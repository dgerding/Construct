using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections.Generic;

namespace Construct.Server.Models.Data
{
    [DataContract]
    public class Datum : IDynamicMetaObjectProvider
    {
        [DataMember]
        private IDictionary<string, object> dynamicProperties = new Dictionary<string, object>();

        public Datum()
        {
            Type selfType = this.GetType();
            foreach (PropertyInfo propertyInfo in selfType.GetProperties(BindingFlags.Public))
            {
                object obj = this;
                object value = propertyInfo.GetValue(obj, null);
                dynamicProperties.Add(propertyInfo.Name, value);
            }
        }

        #region IDynamicMetaObjectProvider implementation
        public DynamicMetaObject GetMetaObject(Expression expression)
        {
            return new SerializableDynamicMetaObject(expression,
                BindingRestrictions.GetInstanceRestriction(expression, this), this);
        }
        #endregion

        #region Helper methods for dynamic meta object support
        internal object setValue(string name, object value)
        {
            dynamicProperties.Add(name, value);
            return value;
        }

        internal object getValue(string name)
        {
            object value;
            if (!dynamicProperties.TryGetValue(name, out value))
            {
                value = null;
            }
            return value;
        }

        internal IEnumerable<string> getDynamicMemberNames()
        {
            return dynamicProperties.Keys;
        }
        #endregion
    }

    public class SerializableDynamicMetaObject : DynamicMetaObject
    {
        Type objType;

        public SerializableDynamicMetaObject(Expression expression, BindingRestrictions restrictions, object value)
            : base(expression, restrictions, value)
        {
            objType = value.GetType();
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var self = this.Expression;
            var dynObj = (Datum)this.Value;
            var keyExpr = Expression.Constant(binder.Name);
            var getMethod = objType.GetMethod("getValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var target = Expression.Call(Expression.Convert(self, objType),
                                         getMethod,
                                         keyExpr);
            return new DynamicMetaObject(target,
                BindingRestrictions.GetTypeRestriction(self, objType));
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var self = this.Expression;
            var keyExpr = Expression.Constant(binder.Name);
            var valueExpr = Expression.Convert(value.Expression, typeof(object));
            var setMethod = objType.GetMethod("setValue", BindingFlags.NonPublic | BindingFlags.Instance);
            var target = Expression.Call(Expression.Convert(self, objType),
            setMethod,
            keyExpr,
            valueExpr);
            return new DynamicMetaObject(target,
                BindingRestrictions.GetTypeRestriction(self, objType));
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var dynObj = (Datum)this.Value;
            return dynObj.getDynamicMemberNames();
        }
    }

}
