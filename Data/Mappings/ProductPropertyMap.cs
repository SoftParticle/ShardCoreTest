using Microsoft.EntityFrameworkCore;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Data.Mappings
{
    public class ProductPropertyMap
    {
        public static void RegisterMapping(ModelBuilder builder)
        {
            var entityBuilder = builder.Entity<ProductProperty>();

            entityBuilder.ToTable("ProductProperties");

            // Primary Key
            entityBuilder.HasKey(e => e.Id).HasName("ProductPropertyId");
            entityBuilder.Property(e => e.Id).HasColumnName("ProductPropertyId").ValueGeneratedOnAdd();
            entityBuilder.Property(e => e.ProductId).IsRequired().HasColumnName("ProductId");
            entityBuilder.Property(e => e.Description).HasColumnName("Description");
            

            // Relationships
            entityBuilder
                .HasOne(e => e.Product)
                .WithMany(e => e.ProductProperties)
                .HasPrincipalKey(e => e.Id);

            // Indexes
            entityBuilder
                .HasIndex(e => e.ProductId)
                .IsUnique(false)
                .IsClustered(false);
        }
    }
}
