using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Hbk.Database.Helpers
{
       public static class ModelBuilderExtension
    {
        public static void ApplyGlobalFilters<TInterface>(this ModelBuilder modelBuilder, Expression<Func<TInterface, bool>> expression)
        {
            var entities = modelBuilder.GetEntitiesFromBaseType<TInterface>();
            foreach (var entity in entities)
            {
                var newParam = Expression.Parameter(entity);
                var newbody = ReplacingExpressionVisitor.Replace(expression.Parameters.Single(), newParam, expression.Body);
                modelBuilder.Entity(entity).HasQueryFilter(Expression.Lambda(newbody, newParam));
            }
        }
        public static void ApplyGlobalInclude<TInterface, TProperty>(this ModelBuilder modelBuilder, Expression<Func<TInterface, TProperty>> expression) where TProperty : class
        {
            var entities = modelBuilder.GetEntitiesFromBaseType<TInterface>();
            foreach (var entity in entities)
            {
                modelBuilder.Entity(entity).Navigation(((MemberExpression)expression.Body).Member.Name).AutoInclude();
            }
        }
        public static void ApplyGlobalDefaultSqlValue<TInterface, TProperty>(this ModelBuilder modelBuilder, Expression<Func<TInterface, TProperty>> expression, string sql)
        {
            var entities = modelBuilder.GetEntitiesFromBaseType<TInterface>();
            foreach (var entity in entities)
            {
                modelBuilder.Entity(entity).Property(((MemberExpression)expression.Body).Member.Name).HasDefaultValueSql(sql);
            }
        }
        private static IEnumerable<Type> GetEntitiesFromBaseType<TBase>(this ModelBuilder modelBuilder) => modelBuilder.Model
                .GetEntityTypes()
                .Where(t => t.BaseType == null)
                .Select(t => t.ClrType)
                .Where(t => typeof(TBase).IsAssignableFrom(t));
    }
}