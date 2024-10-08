﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using mohaymen_codestar_Team02.Data;

#nullable disable

namespace mohaymen_codestar_Team02.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.DataGroup", b =>
                {
                    b.Property<long>("DataGroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("DataGroupId"));

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdateAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("DataGroupId");

                    b.HasIndex("UserId");

                    b.ToTable("DataSets");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeAttribute", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("EdgeEntityId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EdgeEntityId");

                    b.ToTable("EdgeAttributes");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeEntity", b =>
                {
                    b.Property<long>("EdgeEntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("EdgeEntityId"));

                    b.Property<long>("DataGroupId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("EdgeEntityId");

                    b.HasIndex("DataGroupId")
                        .IsUnique();

                    b.ToTable("EdgeEntities");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("EdgeAttributeId")
                        .HasColumnType("bigint");

                    b.Property<string>("ObjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StringValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EdgeAttributeId");

                    b.ToTable("EdgeValues");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.Role", b =>
                {
                    b.Property<long>("RoleId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("RoleId"));

                    b.Property<int[]>("Permissions")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<string>("RoleType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("RoleId");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.User", b =>
                {
                    b.Property<long>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("UserId"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("bytea");

                    b.Property<byte[]>("Salt")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("bytea");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.UserRole", b =>
                {
                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexAttribute", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("VertexEntityId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("VertexEntityId");

                    b.ToTable("VertexAttributes");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexEntity", b =>
                {
                    b.Property<long>("VertexEntityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("VertexEntityId"));

                    b.Property<long>("DataGroupId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("VertexEntityId");

                    b.HasIndex("DataGroupId")
                        .IsUnique();

                    b.ToTable("VertexEntities");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ObjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StringValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("VertexAttributeId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("VertexAttributeId");

                    b.ToTable("VertexValues");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.DataGroup", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.User", "User")
                        .WithMany("DataSets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeAttribute", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeEntity", "EdgeEntity")
                        .WithMany("EdgeAttributes")
                        .HasForeignKey("EdgeEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EdgeEntity");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeEntity", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.DataGroup", "DataGroup")
                        .WithOne("EdgeEntity")
                        .HasForeignKey("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeEntity", "DataGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataGroup");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeValue", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeAttribute", "EdgeAttribute")
                        .WithMany("EdgeValues")
                        .HasForeignKey("EdgeAttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EdgeAttribute");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.UserRole", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mohaymen_codestar_Team02.Models.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexAttribute", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.VertexEAV.VertexEntity", "VertexEntity")
                        .WithMany("VertexAttributes")
                        .HasForeignKey("VertexEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VertexEntity");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexEntity", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.DataGroup", "DataGroup")
                        .WithOne("VertexEntity")
                        .HasForeignKey("mohaymen_codestar_Team02.Models.VertexEAV.VertexEntity", "DataGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataGroup");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexValue", b =>
                {
                    b.HasOne("mohaymen_codestar_Team02.Models.VertexEAV.VertexAttribute", "VertexAttribute")
                        .WithMany("VertexValues")
                        .HasForeignKey("VertexAttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("VertexAttribute");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.DataGroup", b =>
                {
                    b.Navigation("EdgeEntity")
                        .IsRequired();

                    b.Navigation("VertexEntity")
                        .IsRequired();
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeAttribute", b =>
                {
                    b.Navigation("EdgeValues");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.EdgeEAV.EdgeEntity", b =>
                {
                    b.Navigation("EdgeAttributes");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.Role", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.User", b =>
                {
                    b.Navigation("DataSets");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexAttribute", b =>
                {
                    b.Navigation("VertexValues");
                });

            modelBuilder.Entity("mohaymen_codestar_Team02.Models.VertexEAV.VertexEntity", b =>
                {
                    b.Navigation("VertexAttributes");
                });
#pragma warning restore 612, 618
        }
    }
}
