using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Telerik.Windows.Data;

namespace Telerik.Expressions.Runtime
{
    internal partial class LinqExpressionTranslator : ExpressionNodeTranslator<Expression>
    {
        private readonly ClrExpressionRuntime runtime;
        private readonly ExpressionExecutionContext context;
        private IMethodResolver methodResolver;
        private IMemberAccessResolver memberAccessResolver;

        public ClrExpressionRuntime Runtime
        {
            get
            {
                return this.runtime;
            }
        }

        public ExpressionExecutionScope Scope
        {
            get
            {
                return this.context.Scope;
            }
        }

        protected IMethodResolver MethodResolver
        {
            get
            {
                if (this.methodResolver == null)
                {
                    this.methodResolver = this.CreateMethodResolver();
                }

                return this.methodResolver;
            }
        }

        protected IMemberAccessResolver MemberAccessResolver
        {
            get
            {
                if (this.memberAccessResolver == null)
                {
                    this.memberAccessResolver = this.CreateMemberAccessResolver();
                }

                return this.memberAccessResolver;
            }
        }

        internal LinqExpressionTranslator(ClrExpressionRuntime runtime, ExpressionExecutionContext context)
        {
            this.runtime = runtime;
            this.context = context;
        }

        protected virtual IMethodResolver CreateMethodResolver()
        {
            return new ClrMethodResolver(this);
        }

        protected virtual IMemberAccessResolver CreateMemberAccessResolver()
        {
            return new ClrMemberAccessResolver(this);
        }

        public override Expression Translate(ExpressionNode node)
        {
            Expression expression = null;

            try
            {
                expression = base.Translate(node);
            }
            catch (Exception ex)
            {
                this.context.ErrorListener.ReportError(node, ex.Message);
            }

            return expression;
        }

        protected internal override Expression TranslateConstant(ConstantExpressionNode node)
        {
            return Expression.Constant(node.Value);
        }

        protected internal override Expression TranslateBinary(BinaryExpressionNode node)
        {
            var left = this.Translate(node.Left);
            var right = this.Translate(node.Right);

            return MakeBinary(node.Operator.ToExpressionType(), left, right);
        }

        private static Expression MakeBinary(ExpressionType type, Expression left, Expression right)
        {
            Expression specialCaseBinary;
            if (TryCreateBinarySpecialCase(type, left, right, out specialCaseBinary))
            {
                return specialCaseBinary;
            }

            return Expression.MakeBinary(type, left, right);
        }

        protected internal override Expression TranslateUnary(UnaryExpressionNode node)
        {
            var operand = this.Translate(node.Operand);

            return Expression.MakeUnary(node.Operator.ToExpressionType(), operand, null);
        }

        protected internal override Expression TranslateMember(MemberExpressionNode node)
        {
            return this.MemberAccessResolver.ResolveMemberAccess(node);
        }

        protected internal override Expression TranslateIndex(IndexExpressionNode node)
        {
            var instance = this.Translate(node.Instance);
            var args = this.TranslateArguments(node.Arguments);

            var indexer = instance.Type.GetIndexerPropertyInfo(args.Select(a => a.Type));

            return Expression.Call(instance, indexer.GetGetMethod(), args);
        }

        protected internal override Expression TranslateFunction(FunctionExpressionNode node)
        {
            var expression = this.MethodResolver.ResolveMethodCall(node);
            if (expression == null)
            {
                this.context.ErrorListener.ReportError(node, "Unknown function: " + node.Name);
            }

            return expression;
        }

        internal List<Expression> TranslateArguments(IEnumerable<ExpressionNode> arguments)
        {
            return arguments.Select(this.Translate).ToList();
        }

        public LambdaExpression CreateLambdaExpression()
        {
            var body = this.Translate(this.context.Expression);
            var parameters = this.MemberAccessResolver.ResolvedParameters;

            if (body != null)
            {
                return Expression.Lambda(body, parameters.ToArray());
            }

            return null;
        }
    }
}
