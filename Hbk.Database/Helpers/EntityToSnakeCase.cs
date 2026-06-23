using Hbk.Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Hbk.Database.Helpers
{
    /// <summary>
    /// Hbk.Platform Model Builder Extensions.
    /// Overrides for database construction.
    /// Author: MJB
    /// Authored: 10/12/2023
    /// </summary>
    public static class ModelBuilderExtensions
    {
        public static void NameModelEntitiesInSnakeCase(this ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (entity.IsOwned() || entity.ClrType.IsAbstract) continue;

                // Skip owned types and abstract base types
                if (entity.IsOwned() || entity.ClrType.IsAbstract)
                    continue;

                entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName()?.ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName()?.ToSnakeCase());
                }

                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.SetConstraintName(fk.GetConstraintName()?.ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetDatabaseName(index.GetDatabaseName()?.ToSnakeCase());
                }
            }
        }
    }
}