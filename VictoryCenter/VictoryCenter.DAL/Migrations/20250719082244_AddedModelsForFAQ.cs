using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedModelsForFAQ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaqQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisitorPages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FaqPlacements",
                columns: table => new
                {
                    PageId = table.Column<long>(type: "bigint", nullable: false),
                    QuestionId = table.Column<long>(type: "bigint", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaqPlacements", x => new { x.PageId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_FaqPlacements_FaqQuestions_PageId",
                        column: x => x.PageId,
                        principalTable: "FaqQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FaqPlacements_VisitorPages_PageId",
                        column: x => x.PageId,
                        principalTable: "VisitorPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FaqPlacements_PageId_Priority",
                table: "FaqPlacements",
                columns: new[] { "PageId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPages_Slug",
                table: "VisitorPages",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VisitorPages_Title",
                table: "VisitorPages",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaqPlacements");

            migrationBuilder.DropTable(
                name: "FaqQuestions");

            migrationBuilder.DropTable(
                name: "VisitorPages");
        }
    }
}
