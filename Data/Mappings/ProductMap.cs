using Microsoft.EntityFrameworkCore;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Data.Mappings
{
    public class ProductMap
    {
        public static void RegisterMapping(ModelBuilder builder)
        {
            var entityBuilder = builder.Entity<Product>();

            entityBuilder.ToTable("Products");

            // Primary Key
            entityBuilder.HasKey(e => e.ShardId).HasName("ShardId");
            entityBuilder.Property(e => e.ShardId).HasColumnName("ShardId").ValueGeneratedOnAdd();
            entityBuilder.Property(e => e.Id).HasColumnName("GloballyUniqueId").IsRequired();
            entityBuilder.Property(e => e.Model).HasColumnName("Model");
            entityBuilder.Property(e => e.Description).HasColumnName("Description");
            entityBuilder.Property(e => e.Price).HasColumnName("Price");

            // Relationships
            entityBuilder
                .HasMany(e => e.ProductProperties)
                .WithOne(e => e.Product)
                .HasForeignKey(e => e.ProductId);

            // Indexes
            entityBuilder
                .HasIndex(e => e.Id)
                .IsUnique()
                .IsClustered(false);

            entityBuilder
                .HasIndex(e => e.Model)
                .IsUnique(false)
                .IsClustered(false);

            entityBuilder
                .HasIndex(e => e.Description)
                .IsUnique(false)
                .IsClustered(false);

            entityBuilder
                .HasIndex(e => e.Price)
                .IsUnique(false)
                .IsClustered(false);

            // If ordering by descending order is required the descending indexes have to be added to the migration, since code first doesn't support them
            // Please add the following inside the Up method of the migration
            
            /*
            migrationBuilder.Sql(
                sql: "CREATE NONCLUSTERED INDEX [IX_Products_Model_Desc] ON[Products]( [Model] DESC)",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE NONCLUSTERED INDEX [IX_Products_Description_Desc] ON[Products]( [Description] DESC)",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE NONCLUSTERED INDEX [IX_Products_Price_Desc] ON[Products]( [Price] DESC)",
                suppressTransaction: true);

            // If full text search is required add this to the migration aswell
            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;",
                suppressTransaction: true);

            migrationBuilder.Sql(
                sql: "CREATE FULLTEXT INDEX ON Products(Model) KEY INDEX ShardId;",
                suppressTransaction: true);
            */
        }
    }
}
