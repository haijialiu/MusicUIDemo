﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MusicUIDemo.Models.Database;

#nullable disable

namespace MusicUIDemo.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240316182733_AddMusicListTYpe")]
    partial class AddMusicListTYpe
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("MusicUIDemo.Models.Database.MusicMusicList", b =>
                {
                    b.Property<int>("MusicId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MusicListId")
                        .HasColumnType("INTEGER");

                    b.HasKey("MusicId", "MusicListId");

                    b.HasIndex("MusicListId");

                    b.ToTable("MusicMusicLists");
                });

            modelBuilder.Entity("MusicUIDemo.Models.Music", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasColumnOrder(0);

                    b.Property<string>("Album")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(3)
                        .HasComment("专辑名");

                    b.Property<byte[]>("AlbumImage")
                        .HasColumnType("BLOB")
                        .HasColumnOrder(5)
                        .HasComment("专辑图");

                    b.Property<string>("Artist")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(2)
                        .HasComment("歌手");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("TEXT")
                        .HasColumnOrder(6)
                        .HasComment("文件路径");

                    b.Property<double>("Time")
                        .HasColumnType("REAL")
                        .HasColumnOrder(4)
                        .HasComment("时长（秒）");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT")
                        .HasColumnOrder(1)
                        .HasComment("歌名");

                    b.HasKey("Id");

                    b.ToTable("musics", t =>
                        {
                            t.HasComment("音乐库");
                        });
                });

            modelBuilder.Entity("MusicUIDemo.Models.MusicList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValue(new DateTime(2024, 3, 17, 2, 27, 33, 328, DateTimeKind.Local).AddTicks(763));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("MusicList");
                });

            modelBuilder.Entity("MusicUIDemo.Models.Database.MusicMusicList", b =>
                {
                    b.HasOne("MusicUIDemo.Models.Music", null)
                        .WithMany()
                        .HasForeignKey("MusicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MusicUIDemo.Models.MusicList", null)
                        .WithMany()
                        .HasForeignKey("MusicListId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
