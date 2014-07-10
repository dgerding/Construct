using System;

namespace Telerik.Expressions
{
	internal class VariableDefinition : DefinitionBase
	{
		// We should support null as value, 
		// so we must introduce our concept of unset value
		private static readonly object UnsetValue = new object();

		private object valueField;

		public virtual object Value
		{
			get
			{
				if (!this.HasValue)
				{
					throw new InvalidOperationException();
				}

				return this.valueField;
			}
			set
			{
				this.valueField = value;
			}
		}

		public virtual bool HasValue
		{
			get
			{
				return !object.Equals(this.valueField, UnsetValue);
			}
		}

		public virtual Type Type
		{
			get
			{
				if (this.HasValue)
				{
					if (this.valueField != null)
					{
						return this.valueField.GetType();
					}
				}

				return typeof(object);
			}
		}

		public VariableDefinition(string name)
			: this(name, UnsetValue)
		{
		}

		public VariableDefinition(string name, object value)
			: base(name)
		{
			this.valueField = value;
		}
	}
}
