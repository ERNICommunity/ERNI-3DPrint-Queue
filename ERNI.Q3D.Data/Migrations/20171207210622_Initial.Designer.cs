﻿// <auto-generated />
using ERNI.Q3D.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace ERNI.Q3D.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20171207210622_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ERNI.Q3D.Data.PrintJob", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedAt");

                    b.Property<long?>("DataId");

                    b.Property<double>("FilamentLength");

                    b.Property<string>("FileName");

                    b.Property<string>("Name")
                        .HasMaxLength(60);

                    b.Property<long?>("OwnerId");

                    b.Property<DateTime?>("PrintStartedAt");

                    b.Property<TimeSpan>("PrintTime");

                    b.Property<long>("Size");

                    b.HasKey("Id");

                    b.HasIndex("DataId");

                    b.HasIndex("OwnerId");

                    b.ToTable("PrintJobs");
                });

            modelBuilder.Entity("ERNI.Q3D.Data.PrintJobData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("Data");

                    b.HasKey("Id");

                    b.ToTable("PrintJobData");
                });

            modelBuilder.Entity("ERNI.Q3D.Data.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasFilter("[Name] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ERNI.Q3D.Data.PrintJob", b =>
                {
                    b.HasOne("ERNI.Q3D.Data.PrintJobData", "Data")
                        .WithMany()
                        .HasForeignKey("DataId");

                    b.HasOne("ERNI.Q3D.Data.User", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });
#pragma warning restore 612, 618
        }
    }
}
