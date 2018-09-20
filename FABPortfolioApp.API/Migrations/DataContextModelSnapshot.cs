﻿// <auto-generated />
using FABPortfolioApp.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FABPortfolioApp.API.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846");

            modelBuilder.Entity("FABPortfolioApp.API.Models.Portfolio", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Company");

                    b.Property<string>("From");

                    b.Property<string>("Location");

                    b.Property<string>("Project");

                    b.Property<string>("To");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("Portfolios");
                });

            modelBuilder.Entity("FABPortfolioApp.API.Models.PortfolioFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<string>("FileName");

                    b.Property<int>("OrderId");

                    b.Property<int>("PortfolioId");

                    b.HasKey("Id");

                    b.HasIndex("PortfolioId");

                    b.ToTable("PortfolioFiles");
                });

            modelBuilder.Entity("FABPortfolioApp.API.Models.PortfolioFile", b =>
                {
                    b.HasOne("FABPortfolioApp.API.Models.Portfolio", "Portfolio")
                        .WithMany("PortfolioFiles")
                        .HasForeignKey("PortfolioId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
