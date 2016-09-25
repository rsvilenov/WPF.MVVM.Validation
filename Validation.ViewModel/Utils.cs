using System;
using System.Linq.Expressions;

namespace Validation.ViewModel
{
    public static class Utils
    {
        public static string GetPropertyNameFromLambda(Expression<Func<object>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            if (body == null)
                return null;

            return body.Member.Name;
        }

        public static void CallPropertySetter(object instance, string propName)
        {
            var t = instance.GetType();
            var mi = t.GetProperty(propName);
            bool isString = mi.PropertyType == Type.GetType("System.String");
            mi.SetValue(instance, mi.GetValue(instance) ?? (isString ? "" : null));
        }
    }
}
