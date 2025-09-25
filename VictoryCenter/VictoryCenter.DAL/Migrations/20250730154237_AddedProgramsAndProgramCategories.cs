using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedProgramsAndProgramCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Programs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Programs_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProgramProgramCategories",
                columns: table => new
                {
                    CategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    ProgramsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramProgramCategories", x => new { x.CategoriesId, x.ProgramsId });
                    table.ForeignKey(
                        name: "FK_ProgramProgramCategories_ProgramCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "ProgramCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramProgramCategories_Programs_ProgramsId",
                        column: x => x.ProgramsId,
                        principalTable: "Programs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramProgramCategories_ProgramsId",
                table: "ProgramProgramCategories",
                column: "ProgramsId");

            migrationBuilder.CreateIndex(
                name: "IX_Programs_ImageId",
                table: "Programs",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramProgramCategories");

            migrationBuilder.DropTable(
                name: "ProgramCategories");

            migrationBuilder.DropTable(
                name: "Programs");
        }
    }
}
