﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MvcMovie.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250410041446_InitEmployeeLog")]
    partial class InitEmployeeLog
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MvcMovie.Models.EmployeeLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Level")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<DateTime>("TapIn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("TapOut")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("TotalHours")
                        .HasColumnType("double precision");

                    b.Property<double>("TotalSalary")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("EmployeeLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
