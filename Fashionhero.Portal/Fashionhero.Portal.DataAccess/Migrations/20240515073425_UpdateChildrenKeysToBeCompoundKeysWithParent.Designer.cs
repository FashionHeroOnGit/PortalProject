﻿// <auto-generated />
using System;
using Fashionhero.Portal.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Fashionhero.Portal.DataAccess.Migrations
{
    [DbContext(typeof(PortalDatabaseContext))]
    [Migration("20240515073425_UpdateChildrenKeysToBeCompoundKeysWithParent")]
    partial class UpdateChildrenKeysToBeCompoundKeysWithParent
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Image", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("ImageId");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("Images");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.LocaleProduct", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("LocaleProductId");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Colour")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CountryOrigin")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Gender")
                        .HasColumnType("TEXT");

                    b.Property<string>("IsoName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ItemGroupId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LocalType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Material")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ReferenceId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "ProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("ReferenceId", "IsoName")
                        .IsUnique();

                    b.ToTable("LocaleProducts");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Price", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("PriceId");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<int>("Currency")
                        .HasColumnType("INTEGER");

                    b.Property<float?>("Discount")
                        .HasColumnType("REAL");

                    b.Property<float>("NormalSell")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "ProductId");

                    b.HasIndex("ProductId");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnName("ProductId");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("LinkBase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Manufacturer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ReferenceId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Size", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("SizeId");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<long>("Ean")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LinkBase")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LinkPostFix")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ModelProductNumber")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Primary")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReferenceId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Secondary")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "ProductId");

                    b.HasIndex("Ean")
                        .IsUnique();

                    b.HasIndex("ReferenceId")
                        .IsUnique();

                    b.HasIndex("ProductId", "Primary", "Secondary")
                        .IsUnique();

                    b.ToTable("Sizes");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Tag", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER")
                        .HasColumnName("TagId");

                    b.Property<int>("ProductId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedDateTime")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id", "ProductId");

                    b.HasIndex("ProductId");

                    b.HasIndex("Name", "ProductId")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Image", b =>
                {
                    b.HasOne("Fashionhero.Portal.Shared.Model.Entity.Product", "Product")
                        .WithMany("Images")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.LocaleProduct", b =>
                {
                    b.HasOne("Fashionhero.Portal.Shared.Model.Entity.Product", "Product")
                        .WithMany("Locales")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Price", b =>
                {
                    b.HasOne("Fashionhero.Portal.Shared.Model.Entity.Product", "Product")
                        .WithMany("Prices")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Size", b =>
                {
                    b.HasOne("Fashionhero.Portal.Shared.Model.Entity.Product", "Product")
                        .WithMany("Sizes")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Tag", b =>
                {
                    b.HasOne("Fashionhero.Portal.Shared.Model.Entity.Product", "Product")
                        .WithMany("ExtraTags")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Fashionhero.Portal.Shared.Model.Entity.Product", b =>
                {
                    b.Navigation("ExtraTags");

                    b.Navigation("Images");

                    b.Navigation("Locales");

                    b.Navigation("Prices");

                    b.Navigation("Sizes");
                });
#pragma warning restore 612, 618
        }
    }
}
