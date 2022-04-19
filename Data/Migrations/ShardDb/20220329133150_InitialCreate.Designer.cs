﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShardCoreTest.Data.EntityFramework;

namespace ShardCoreTest.Data.Migrations.ShardDb
{
    [DbContext(typeof(ShardDbContext))]
    [Migration("20220329133150_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.15");

            modelBuilder.Entity("ShardCoreTest.DomainModels.Product", b =>
                {
                    b.Property<long>("ShardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("ShardId")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Description");

                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("GloballyUniqueId");

                    b.Property<string>("Model")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Model");

                    b.Property<float>("Price")
                        .HasColumnType("real")
                        .HasColumnName("Price");

                    b.HasKey("ShardId")
                        .HasName("ShardId");

                    b.HasIndex("Description")
                        .IsClustered(false);

                    b.HasIndex("Id")
                        .IsUnique()
                        .IsClustered(false);

                    b.HasIndex("Model")
                        .IsClustered(false);

                    b.HasIndex("Price")
                        .IsClustered(false);

                    b.ToTable("Products");
                });

            modelBuilder.Entity("ShardCoreTest.DomainModels.ProductProperty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnName("ProductPropertyId")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Description");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ProductId");

                    b.HasKey("Id")
                        .HasName("ProductPropertyId");

                    b.HasIndex("ProductId")
                        .IsClustered(false);

                    b.ToTable("ProductProperties");
                });

            modelBuilder.Entity("ShardCoreTest.DomainModels.ProductProperty", b =>
                {
                    b.HasOne("ShardCoreTest.DomainModels.Product", "Product")
                        .WithMany("ProductProperties")
                        .HasForeignKey("ProductId")
                        .HasPrincipalKey("Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("ShardCoreTest.DomainModels.Product", b =>
                {
                    b.Navigation("ProductProperties");
                });
#pragma warning restore 612, 618
        }
    }
}
