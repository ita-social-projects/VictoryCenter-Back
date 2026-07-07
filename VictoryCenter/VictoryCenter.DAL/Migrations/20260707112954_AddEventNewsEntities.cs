using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddEventNewsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventNews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Resource = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PreviewImageId = table.Column<long>(type: "bigint", nullable: true),
                    BackgroundImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventNews_Images_BackgroundImageId",
                        column: x => x.BackgroundImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EventNews_Images_PreviewImageId",
                        column: x => x.PreviewImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventNewsCategories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNewsCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventNewsLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNewsLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_EventNewsLocalizations_EventNews_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EventNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventNewsLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventNewsEventNewsCategories",
                columns: table => new
                {
                    CategoriesId = table.Column<long>(type: "bigint", nullable: false),
                    EventsNewsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventNewsEventNewsCategories", x => new { x.CategoriesId, x.EventsNewsId });
                    table.ForeignKey(
                        name: "FK_EventNewsEventNewsCategories_EventNewsCategories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "EventNewsCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventNewsEventNewsCategories_EventNews_EventsNewsId",
                        column: x => x.EventsNewsId,
                        principalTable: "EventNews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventNews_BackgroundImageId",
                table: "EventNews",
                column: "BackgroundImageId",
                unique: true,
                filter: "[BackgroundImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventNews_PreviewImageId",
                table: "EventNews",
                column: "PreviewImageId",
                unique: true,
                filter: "[PreviewImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventNews_Slug",
                table: "EventNews",
                column: "Slug",
                unique: true,
                filter: "[Slug] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventNewsEventNewsCategories_EventsNewsId",
                table: "EventNewsEventNewsCategories",
                column: "EventsNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_EventNewsLocalizations_LanguageId",
                table: "EventNewsLocalizations",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventNewsEventNewsCategories");

            migrationBuilder.DropTable(
                name: "EventNewsLocalizations");

            migrationBuilder.DropTable(
                name: "EventNewsCategories");

            migrationBuilder.DropTable(
                name: "EventNews");
        }
    }
}
