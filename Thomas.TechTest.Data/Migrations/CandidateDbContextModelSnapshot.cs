﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thomas.TechTest.Data;

namespace Thomas.TechTest.Data.Migrations
{
    [DbContext(typeof(CandidateDbContext))]
    partial class CandidateDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2");

            modelBuilder.Entity("Thomas.TechTest.Data.Assessment", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AssessmentType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AssignedCandidateId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CompletedOn")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SentOn")
                        .HasColumnType("TEXT");

                    b.Property<int?>("TrainabilityIndex")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AssignedCandidateId");

                    b.ToTable("Assessments");
                });

            modelBuilder.Entity("Thomas.TechTest.Data.Candidate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Firstname")
                        .HasColumnType("TEXT");

                    b.Property<string>("Surname")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Candidates");
                });

            modelBuilder.Entity("Thomas.TechTest.Data.Assessment", b =>
                {
                    b.HasOne("Thomas.TechTest.Data.Candidate", "AssignedCandidate")
                        .WithMany("Assessments")
                        .HasForeignKey("AssignedCandidateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
