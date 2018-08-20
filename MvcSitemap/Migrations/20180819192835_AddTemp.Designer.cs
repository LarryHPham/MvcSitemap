﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MvcSitemap.Models;

namespace MvcSitemap.Migrations
{
    [DbContext(typeof(MvcSitemapContext))]
    [Migration("20180819192835_AddTemp")]
    partial class AddTemp
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MvcSitemap.Models.Sitemap", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChangeFrequency");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<bool>("NoIndex");

                    b.Property<decimal>("Priority");

                    b.Property<string>("Status");

                    b.Property<string>("TempChangeFrequency");

                    b.Property<DateTime>("TempModifiedDate");

                    b.Property<decimal>("TempPriority");

                    b.Property<string>("Url");

                    b.HasKey("ID");

                    b.ToTable("Sitemap");
                });
#pragma warning restore 612, 618
        }
    }
}
