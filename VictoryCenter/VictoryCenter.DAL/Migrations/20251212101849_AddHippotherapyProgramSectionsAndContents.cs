using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHippotherapyProgramSectionsAndContents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HippotherapyProgramSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramId = table.Column<long>(type: "bigint", nullable: false),
                    Template = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyProgramSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyProgramSections_HippotherapyPrograms_ProgramId",
                        column: x => x.ProgramId,
                        principalTable: "HippotherapyPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramSectionContents",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SectionId = table.Column<long>(type: "bigint", nullable: false),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramSectionContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramSectionContents_HippotherapyProgramSections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "HippotherapyProgramSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramSectionContents_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyProgramSections_ProgramId",
                table: "HippotherapyProgramSections",
                column: "ProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSectionContents_ImageId",
                table: "ProgramSectionContents",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSectionContents_SectionId",
                table: "ProgramSectionContents",
                column: "SectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramSectionContents");

            migrationBuilder.DropTable(
                name: "HippotherapyProgramSections");
        }
    }
}
