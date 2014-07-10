using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.Expressions
{
	internal class ExpressionExecutionScope
	{
		private static readonly ICollection<FunctionDefinition> EmptyFunctions = Enumerable.Empty<FunctionDefinition>().ToArray();

		private readonly ExpressionExecutionScope parent;
		private readonly IDictionary<string, VariableDefinition> variables;
		private readonly IDictionary<string, ICollection<FunctionDefinition>> functions;

		public ExpressionExecutionScope Parent
		{
			get
			{
				return this.parent;
			}
		}

		public virtual IEnumerable<VariableDefinition> Variables
		{
			get
			{
				return this.variables.Values;
			}
		}

		public virtual IEnumerable<FunctionDefinition> Functions
		{
			get
			{
				return this.functions.Values.SelectMany(v => v);
			}
		}

		public ExpressionExecutionScope()
			: this(null)
		{
		}

		public ExpressionExecutionScope(ExpressionExecutionScope parent)
			: this(parent, Enumerable.Empty<VariableDefinition>(), Enumerable.Empty<FunctionDefinition>())
		{
		}

		public ExpressionExecutionScope(
			ExpressionExecutionScope parent,
			IEnumerable<VariableDefinition> variables,
			IEnumerable<FunctionDefinition> functions)
			: this(
				parent,
				variables.ToDictionary(VariableNameSelector, ExpressionHelper.IdentifierComparer),
				functions
				.GroupBy(FunctionNameSelector, ExpressionHelper.IdentifierComparer)
				.ToDictionary(g => g.Key, g => (ICollection<FunctionDefinition>)g.ToList()))
		{
		}

		private ExpressionExecutionScope(
			ExpressionExecutionScope parent,
			IDictionary<string, VariableDefinition> variables,
			IDictionary<string, ICollection<FunctionDefinition>> functions)
		{
			this.parent = parent;
			this.variables = variables;
			this.functions = functions;
		}

		public virtual void AddVariable(VariableDefinition variable)
		{
			var variableName = VariableNameSelector(variable);
			if (!this.variables.ContainsKey(variableName))
			{
				this.variables.Add(variableName, variable);
			}
		}

		public virtual void AddFunction(FunctionDefinition function)
		{
			var name = FunctionNameSelector(function);

			ICollection<FunctionDefinition> matchingFunctions;
			if (!this.functions.TryGetValue(name, out matchingFunctions))
			{
				matchingFunctions = new List<FunctionDefinition>();
				this.functions.Add(name, matchingFunctions);
			}

			matchingFunctions.Add(function);
		}

		public virtual bool TryGetVariable(string name, out VariableDefinition variable)
		{
			if (!this.variables.TryGetValue(name, out variable))
			{
				if (this.parent != null)
				{
					return this.parent.TryGetVariable(name, out variable);
				}

				return false;
			}

			return true;
		}

		public virtual IEnumerable<FunctionDefinition> GetFunctions(string name)
		{
			ICollection<FunctionDefinition> functionsToReturn;
			if (!this.functions.TryGetValue(name, out functionsToReturn))
			{
				functionsToReturn = EmptyFunctions;
			}

			if (this.parent != null)
			{
				return functionsToReturn.Concat(this.parent.GetFunctions(name));
			}

			return functionsToReturn;
		}

		public virtual ExpressionExecutionScope CreateChildScope()
		{
			return new ExpressionExecutionScope(this);
		}

		private static string VariableNameSelector(VariableDefinition variable)
		{
			return variable.Name;
		}

		private static string FunctionNameSelector(FunctionDefinition function)
		{
			return function.Name;
		}

#if DEBUG
		private int Level
		{
			get
			{
				if (this.parent == null)
				{
					return 0;
				}
				else 
				{
					return parent.Level + 1;
				}
			}
		}
		
		public override string ToString()
		{
			return "Level " + this.Level;
		}
#endif
	}
}
