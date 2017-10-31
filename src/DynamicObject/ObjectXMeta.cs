using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicObject
{
    internal class ObjectXMeta : DynamicMetaObject
    {
        private static readonly Type typeXObject = typeof(ObjectX);
        private static readonly MethodInfo Getter = typeXObject.GetMethod("GetValue"); 
        private static readonly MethodInfo Setter = typeXObject.GetMethod("SetValue");


        public ObjectXMeta(Expression expression, object value)
            : base(expression, BindingRestrictions.Empty, value)
        {
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            var propertyName = binder.Name;
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
            var expPropName = Expression.Constant(propertyName);
            var xobj = Expression.Convert(Expression, LimitType);
            var exp = Expression.Call(xobj, Getter, expPropName);
            return new DynamicMetaObject(exp, restrictions, Value);
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
        {
            var propertyName = binder.Name;
            var restrictions = BindingRestrictions.GetTypeRestriction(Expression, LimitType);
            var expPropName = Expression.Constant(propertyName);

            var xobj = Expression.Convert(Expression, LimitType);
            var exp = Expression.Call(xobj, Setter, expPropName, value.Expression);
            return new DynamicMetaObject(exp, restrictions);
        }
    }
}