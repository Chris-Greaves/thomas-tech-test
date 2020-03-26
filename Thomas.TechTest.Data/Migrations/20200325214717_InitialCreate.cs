using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Thomas.TechTest.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AptitudeAssessments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SentOn = table.Column<DateTime>(nullable: false),
                    CompletedOn = table.Column<DateTime>(nullable: true),
                    TrainabilityIndex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AptitudeAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BehaviourAssessments",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SentOn = table.Column<DateTime>(nullable: false),
                    CompletedOn = table.Column<DateTime>(nullable: true),
                    WorkingStrengths = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BehaviourAssessments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    Firstname = table.Column<string>(nullable: true),
                    Lastname = table.Column<string>(nullable: true),
                    BehaviourAssessmentId = table.Column<long>(nullable: false),
                    AptitudeAssessmentId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Candidates_AptitudeAssessments_AptitudeAssessmentId",
                        column: x => x.AptitudeAssessmentId,
                        principalTable: "AptitudeAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Candidates_BehaviourAssessments_BehaviourAssessmentId",
                        column: x => x.BehaviourAssessmentId,
                        principalTable: "BehaviourAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_AptitudeAssessmentId",
                table: "Candidates",
                column: "AptitudeAssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Candidates_BehaviourAssessmentId",
                table: "Candidates",
                column: "BehaviourAssessmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Candidates");

            migrationBuilder.DropTable(
                name: "AptitudeAssessments");

            migrationBuilder.DropTable(
                name: "BehaviourAssessments");
        }
    }
}
