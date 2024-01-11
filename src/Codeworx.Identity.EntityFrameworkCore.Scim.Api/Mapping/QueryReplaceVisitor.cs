using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class QueryReplaceVisitor : ExpressionVisitor
    {
        private static readonly PropertyInfo _providerIdMember;
        private static readonly MethodInfo _setMethod;
        private static readonly PropertyInfo _queryDataProviderId;
        private static readonly PropertyInfo _queryDataDb;
        private static readonly Type _queryType;

        private readonly QueryData _queryData;

        static QueryReplaceVisitor()
        {
            _queryType = typeof(Query);

            Expression<Func<Guid>> exp = () => Query.ProviderId;
            _providerIdMember = (PropertyInfo)((MemberExpression)exp.Body).Member;

            Expression<Func<DbContext, IQueryable<object>>> exp2 = p => p.Set<object>();
            _setMethod = ((MethodCallExpression)exp2.Body).Method.GetGenericMethodDefinition();

            Expression<Func<QueryData, Guid>> exp3 = p => p.ProviderId;
            _queryDataProviderId = (PropertyInfo)((MemberExpression)exp3.Body).Member;

            Expression<Func<QueryData, DbContext>> exp4 = p => p.Db;
            _queryDataDb = (PropertyInfo)((MemberExpression)exp4.Body).Member;
        }

        public QueryReplaceVisitor(QueryData queryData)
        {
            _queryData = queryData;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod && node.Method.DeclaringType == _queryType)
            {
                return Expression.Call(
                    Expression.Property(Expression.Constant(_queryData), _queryDataDb),
                    _setMethod.MakeGenericMethod(node.Method.GetGenericArguments()[0]));
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member == _providerIdMember)
            {
                return Expression.Property(Expression.Constant(_queryData), _queryDataProviderId);
            }

            return base.VisitMember(node);
        }
    }
}
