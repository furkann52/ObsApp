﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace OBSApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241229215453_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OBSApp.Models.Ders", b =>
                {
                    b.Property<int>("DersId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DersId"));

                    b.Property<string>("DersAd")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DersKod")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DersId");

                    b.ToTable("Dersler");
                });

            modelBuilder.Entity("OBSApp.Models.Ogrenci", b =>
                {
                    b.Property<int>("OgrenciId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OgrenciId"));

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Numara")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SinifId")
                        .HasColumnType("int");

                    b.Property<string>("Soyad")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OgrenciId");

                    b.HasIndex("SinifId");

                    b.ToTable("Ogrenciler");
                });

            modelBuilder.Entity("OBSApp.Models.OgrenciDers", b =>
                {
                    b.Property<int>("DersId")
                        .HasColumnType("int");

                    b.Property<int>("OgrenciId")
                        .HasColumnType("int");

                    b.HasKey("DersId", "OgrenciId");

                    b.HasIndex("OgrenciId");

                    b.ToTable("OgrenciDersler");
                });

            modelBuilder.Entity("OBSApp.Models.Sinif", b =>
                {
                    b.Property<int>("SinifId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SinifId"));

                    b.Property<int>("Kontenjan")
                        .HasColumnType("int");

                    b.Property<string>("SinifAd")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SinifId");

                    b.ToTable("Siniflar");
                });

            modelBuilder.Entity("OBSApp.Models.Ogrenci", b =>
                {
                    b.HasOne("OBSApp.Models.Sinif", "Sinif")
                        .WithMany("Ogrenciler")
                        .HasForeignKey("SinifId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Sinif");
                });

            modelBuilder.Entity("OBSApp.Models.OgrenciDers", b =>
                {
                    b.HasOne("OBSApp.Models.Ders", "Ders")
                        .WithMany("OgrenciDersler")
                        .HasForeignKey("DersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OBSApp.Models.Ogrenci", "Ogrenci")
                        .WithMany("OgrenciDersler")
                        .HasForeignKey("OgrenciId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ders");

                    b.Navigation("Ogrenci");
                });

            modelBuilder.Entity("OBSApp.Models.Ders", b =>
                {
                    b.Navigation("OgrenciDersler");
                });

            modelBuilder.Entity("OBSApp.Models.Ogrenci", b =>
                {
                    b.Navigation("OgrenciDersler");
                });

            modelBuilder.Entity("OBSApp.Models.Sinif", b =>
                {
                    b.Navigation("Ogrenciler");
                });
#pragma warning restore 612, 618
        }
    }
}