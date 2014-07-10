using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Telerik.Expressions
{
	internal partial class ExpressionParser
	{
		private readonly Lexer lexer;
		private ExpressionNode previousNode;

		private ExpressionParser(Lexer lexer)
		{
			this.lexer = lexer;
		}

		public static ExpressionNode Parse(string input)
		{
			using (var reader = new StringSourceReader(input))
			{
				return Parse(reader);
			}
		}

		public static ExpressionNode Parse(TextReader reader)
		{
			using (var sourceReader = new TextReaderSourceReader(reader))
			{
				return Parse(sourceReader);
			}
		}

		private static ExpressionNode Parse(ISourceReader reader)
		{
			var context = new ParserContext(reader);

			return Parse(context);
		}

		internal static ExpressionNode Parse(ParserContext context)
		{
			var lexer = new Lexer(context);
			var parser = new ExpressionParser(lexer);

			return parser.ParseSingleExpression();
		}

		private ExpressionNode ParseSingleExpression()
		{
			var token = this.PeekToken();

			if (token == null)
			{
				return null;
			}

			ExpressionNode result = null;

			if (token.TokenType == TokenType.Paren)
			{
				result = this.ParseParentheticForm();
			}
			else if (token.TokenType == TokenType.Bracket)
			{
				result = this.ParseIndexer();
			}
			else if (token is NamedToken)
			{
				result = this.ParseNamedToken();
			}
			else if (token is ILiteralToken)
			{
				result = this.ParseConstant();
			}

			if (result != null)
			{
				result = this.ParseAnyNextReservedToken(result);
			}

			return result;
		}

		private ExpressionNode ParseAnyNextReservedToken(ExpressionNode previouslyParsedNode)
		{
			var next = this.PeekToken();
			while (next is ReservedToken)
			{
				// TODO: reconsider removing previousNode member.
				this.previousNode = previouslyParsedNode;
				previouslyParsedNode = this.ParseReservedToken();
				this.previousNode = null;

				next = this.PeekToken();
			}

			// Nedyalko: At this point should appear only Syntax tokens such as EOF
			// fix for the case 3 + 6 _1
			IdentifierToken identToken = next as IdentifierToken;
			if (identToken != null)
			{
				this.ReportError(identToken.Span(), "Missing Operator!");
			}

			return previouslyParsedNode;
		}

		private ExpressionNode ParseConstant()
		{
			var literalToken = this.ReadToken();
            Debug.Assert(literalToken is ILiteralToken, "literalToken is ILiteralToken");

			return CreateConstant(literalToken, t => ((ILiteralToken)t).Value);
		}

		private static ConstantExpressionNode CreateConstant<TToken>(TToken token, Func<TToken, object> valueGetter)
			where TToken : Token
		{
			var node = ExpressionNode.Constant(valueGetter(token));
			node.SetSpan(token.Span());

			return node;
		}

		private ExpressionNode ParseParentheticForm()
		{
			var token = this.ReadToken();
            Debug.Assert(token.TokenType == TokenType.Paren, "token.TokenType == TokenType.Paren");

			var node = this.ParseSingleExpression();
			if (node != null)
			{
				token = this.ReadToken();
				if (token.TokenType != TokenType.CloseParen)
				{
					this.ReportMissingEndToken(token.Span(), TokenType.CloseParen);
				}

				return node;
			}

			// TODO: should we support root object functions
			return null;
		}

		private ExpressionNode ParseNamedToken()
		{
			var token = this.PeekToken();
            Debug.Assert(token is NamedToken, "token is NamedToken");

			if (token is ReservedToken)
			{
				return this.ParseReservedToken();
			}

			return this.ParseMemberAccess();
		}

		private ExpressionNode ParseReservedToken()
		{
			var token = this.PeekToken();
            Debug.Assert(token is ReservedToken, "token is ReservedToken");

			if (token is KeywordToken)
			{
				return this.ParseKeyword();
			}

            Debug.Assert(token is OperatorToken, "token is OperatorToken");

			return this.ParseOperator();
		}

		private ExpressionNode ParseKeyword()
		{
			var keywordToken = this.ReadToken() as KeywordToken;
            Debug.Assert(keywordToken != null, "keywordToken != null");

			if (keywordToken.IsKeywordOperator())
			{
				return this.ParseUnaryOrBinary(keywordToken);
			}

			return ParseKeywordConstant(keywordToken);
		}

		private static ConstantExpressionNode ParseKeywordConstant(KeywordToken keywordToken)
		{
            Debug.Assert(keywordToken.TokenType.IsKeywordConstant(), "keywordToken.TokenType.IsKeywordConstant()");

			return CreateConstant(keywordToken, t => t.TokenType.GetKeywordConstantValue());
		}

		private ExpressionNode ParseOperator()
		{
			var token = this.ReadToken() as OperatorToken;
            Debug.Assert(token != null, "token != null");

			if (token.TokenType == TokenType.Dot)
			{
				return this.ParseMemberAccess();
			}

			return this.ParseUnaryOrBinary(token);
		}

		private ExpressionNode ParseMemberAccess()
		{
			var parsedMembersQueue = new Queue<IMemberInvocationExpressionNode>();

			while (true)
			{
				var token = this.ReadToken();

				var nameToken = token as IdentifierToken;
				if (nameToken == null)
				{
					this.ReportError(token.Span(), "Missing argument.");
					break;
				}

				var node = this.ParseMemberOrFunction(nameToken);

				parsedMembersQueue.Enqueue(node);

				var nextToken = this.PeekToken();
				if (nextToken.TokenType == TokenType.Bracket)
				{
					node = this.ParseIndexer();

					parsedMembersQueue.Enqueue(node);
				}

				nextToken = this.PeekToken();
				if (nextToken.TokenType != TokenType.Dot)
				{
					break;
				}

				// Read the dot.
				this.ReadToken();
			}

			var result = this.previousNode;
			foreach (var memberInvocationNode in parsedMembersQueue)
			{
				result = UpdateMemberInvocationInstance(memberInvocationNode, result);
			}

			return result;
		}

		private IMemberInvocationExpressionNode ParseMemberOrFunction(IdentifierToken nameToken)
		{
			var nextToken = this.PeekToken();
			if (nextToken.TokenType == TokenType.Paren)
			{
				return this.ParseFunction(nameToken);
			}

			return ParseMember(nameToken);
		}

		private static ExpressionNode UpdateMemberInvocationInstance(IMemberInvocationExpressionNode target, ExpressionNode instance)
		{
			var updated = target.Update(instance, target.Arguments);

			updated.SetSpan(target.Span());

			return updated;
		}

		private ExpressionNode ParseUnaryOrBinary(ReservedToken token)
		{
			ExpressionNodeOperator @operator;

			// We are parsing binary.
			if (this.previousNode != null)
			{
				if (this.TryGetApplicableBinaryOperator(token, out @operator))
				{
					return this.ParseBinary(@operator);
				}
				return null;
			}

			if (this.TryGetApplicableUnaryOperator(token, out @operator))
			{
				return this.ParseUnary(@operator);
			}
			return null;
		}

		private bool TryGetApplicableUnaryOperator(ReservedToken token, out ExpressionNodeOperator @operator)
		{
			return this.TryGetApplicableOperator(token, out @operator, /*isUnary:*/ true);
		}

		private bool TryGetApplicableBinaryOperator(ReservedToken token, out ExpressionNodeOperator @operator)
		{
			return this.TryGetApplicableOperator(token, out @operator, /*isUnary:*/ false);
		}

		private bool TryGetApplicableOperator(ReservedToken token, out ExpressionNodeOperator @operator, bool isUnary)
		{
			var applicableOperators = GetApplicableOperators(token, isUnary);

			if (applicableOperators.Length == 1)
			{
				@operator = applicableOperators[0];
				return true;
			}

			this.ReportInvalidApplicableOperatorsError(token, isUnary, applicableOperators.Length);

			@operator = default(ExpressionNodeOperator);
			return false;
		}

		private void ReportInvalidApplicableOperatorsError(ReservedToken token, bool isUnary, int applicableOperatorsCount)
		{
			string errorMessageFormat;
			if (applicableOperatorsCount == 0)
			{
				errorMessageFormat = "No applicable {0} operator at this context.";
			}
			else
			{
                Debug.Assert(applicableOperatorsCount > 1, "applicableOperatorsCount > 1");
				errorMessageFormat = "More than one applicable {0} operator at this context.";
			}
			var operatorType = isUnary ? "unary" : "binary";

			this.ReportError(token.Span(), string.Format(errorMessageFormat, operatorType));
		}

		private static ExpressionNodeOperator[] GetApplicableOperators(ReservedToken token, bool isUnary)
		{
			Func<ExpressionNodeOperator, bool> operatorPredicate;
			if (isUnary)
			{
				operatorPredicate = op => op.IsUnaryOperator();
			}
			else
			{
				operatorPredicate = op => op.IsBinaryOperator();
			}

			return
				token.TokenType
				.GetExpressionNodeOperators()
				.Where(operatorPredicate)
				.ToArray();
		}

		private ExpressionNode ParseUnary(ExpressionNodeOperator parsedOperator)
		{
            Debug.Assert(parsedOperator.IsUnaryOperator(), "parsedOperator.IsUnaryOperator()");

			// Special case for negated numbers
			if (parsedOperator == ExpressionNodeOperator.Negate && this.PeekToken() is INumberToken)
			{
				return this.ParseNegatedNumber();
			}

			if (!this.CheckValidOperand())
			{
				return ExpressionNode.Unary(null, parsedOperator);
			}

			return this.ParseValidUnary(parsedOperator);
		}

		private UnaryExpressionNode ParseValidUnary(ExpressionNodeOperator parsedOperator)
		{
			var operand = this.ParseSingleExpression();
			var unary = ExpressionNode.Unary(operand, parsedOperator);

			return unary;
		}

		private BinaryExpressionNode ParseBinary(ExpressionNodeOperator parsedOperator)
		{
            Debug.Assert(parsedOperator.IsBinaryOperator(), "parsedOperator.IsBinaryOperator()");
			Debug.Assert(this.previousNode != null, "Binary should have previous (left) operand.");

			var left = this.previousNode;
			this.previousNode = null;

			if (!this.CheckValidOperand())
			{
				return ExpressionNode.Binary(left, null, parsedOperator);
			}

			return this.ParseValidBinary(left, parsedOperator);
		}

		private BinaryExpressionNode ParseValidBinary(ExpressionNode left, ExpressionNodeOperator @operator)
		{
			// if the next token is not paren '(' we should apply operator precedence rule
			var shouldApplyOperatorPrecedence = this.PeekToken().TokenType != TokenType.Paren;

			var right = this.ParseSingleExpression();
			var binary = ExpressionNode.Binary(left, right, @operator);

			if (shouldApplyOperatorPrecedence && right != null)
			{
				return RewriteBinaryOperatorPrecedence(binary);
			}

			return binary;
		}

		private bool CheckValidOperand()
		{
			var token = this.PeekToken();
			if (token == null)
			{
				return false;
			}
			
			if (IsTokenInvalidOperand(token))
			{
				this.ReportError(token.Span(), "Missing operand.");
				return false;
			}

			return true;
		}

		private static bool IsTokenInvalidOperand(Token token)
		{
			var tokenType = token.TokenType;
			var keywordToken = token as KeywordToken;

			return
				tokenType == TokenType.Comma ||
				tokenType == TokenType.CloseParen ||
				tokenType == TokenType.CloseBracket ||
				tokenType == TokenType.Eof ||
				token is OperatorToken ||
				(keywordToken != null && keywordToken.IsKeywordOperator());
		}

		private ExpressionNode ParseNegatedNumber()
		{
			var numberToken = this.ReadToken();
            Debug.Assert(numberToken is INumberToken, "numberToken is INumberToken");

			return CreateConstant(numberToken, t => ((INumberToken)t).NegatedValue);
		}

		private static MemberExpressionNode ParseMember(IdentifierToken nameToken)
		{
            Debug.Assert(nameToken != null, "nameToken != null");

			var node = ExpressionNode.Member(nameToken.Name);
			node.SetSpan(nameToken.Span());

			return node;
		}

		private IndexExpressionNode ParseIndexer()
		{
			var args = this.ParseIndexArguments();

			return ExpressionNode.Index(args);
		}

		private FunctionExpressionNode ParseFunction(IdentifierToken nameToken)
		{
            Debug.Assert(nameToken != null, "nameToken != null");

			var name = nameToken.Name;
			var args = this.ParseFunctionArguments();

			var node = ExpressionNode.Function(name, args);
			node.SetSpan(nameToken.Span());

			return node;
		}

		private IEnumerable<ExpressionNode> ParseIndexArguments()
		{
			return this.ParseArguments(TokenType.CloseBracket);
		}

		private IEnumerable<ExpressionNode> ParseFunctionArguments()
		{
			return this.ParseArguments(TokenType.CloseParen);
		}

		private IEnumerable<ExpressionNode> ParseArguments(TokenType end)
		{
			var token = this.ReadToken();

			token = this.PeekToken();
			if (token.TokenType == end)
			{
				this.ReadToken();
				return Enumerable.Empty<ExpressionNode>();
			}

			return this.ParseNotEmptyArguments(end);
		}

		private IEnumerable<ExpressionNode> ParseNotEmptyArguments(TokenType end)
		{
			var args = new List<ExpressionNode>();

			while (true)
			{
				var arg = this.ParseSingleExpression();
				if (arg != null)
				{
					args.Add(arg);
				}
				else
				{
					this.ReportMissingArgument();
				}

				var token = this.ReadToken();
				if (token.TokenType == end)
				{
					break;
				}
				else if (token.TokenType != TokenType.Comma)
				{
                    this.ReportMissingEndToken(token.Span(), end);
					break;
				}
			}

			return args;
		}

		private Token PeekTokenAhead(int tokensToSkip)
		{
            Debug.Assert(tokensToSkip >= 0, "tokensToSkip >= 0");

			var skippedTokens = new Stack<Token>(tokensToSkip);

			while (tokensToSkip >= 0)
			{
				skippedTokens.Push(this.ReadToken());
				tokensToSkip--;
			}

			var token = skippedTokens.Peek();

			foreach (var skippedToken in skippedTokens)
			{
				this.AddToken(skippedToken);
			}

			return token;
		}

		private Token PeekToken()
		{
			return this.PeekTokenAhead(0);
		}

		private void AddToken(Token token)
		{
			this.lexer.AddToken(token);
		}

		private Token ReadToken()
		{
			return this.lexer.ReadToken();
		}

		private void ReportError(SourceSpan errorSpan, string message)
		{
			this.lexer.Context.ErrorListener.ReportError(errorSpan, message);
		}

		private void ReportMissingArgument()
		{
			this.ReportError(this.PeekToken().Span(), "Missing argument.");
		}

		private void ReportMissingEndToken(SourceSpan errorSpan, TokenType end)
		{
			this.ReportError(errorSpan, string.Format("Missing ending '{0}'.", end.ToTokenString()));
		}
	}
}
