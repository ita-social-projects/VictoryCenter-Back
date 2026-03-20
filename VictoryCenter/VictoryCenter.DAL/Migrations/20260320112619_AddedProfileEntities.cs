using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedProfileEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfileContacts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CorrespondenceEmail = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Motto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfileContacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfileContacts_CompanyProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "CompanyProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfileRequisites",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Edrpou = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfileRequisites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfileRequisites_CompanyProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "CompanyProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfileSocialLinks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfileId = table.Column<long>(type: "bigint", nullable: false),
                    SocialPlatform = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfileSocialLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyProfileSocialLinks_CompanyProfiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "CompanyProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfileContactLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Motto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfileContactLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_CompanyProfileContactLocalizations_CompanyProfileContacts_EntityId",
                        column: x => x.EntityId,
                        principalTable: "CompanyProfileContacts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProfileContactLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfileRequisiteLocalizations",
                columns: table => new
                {
                    EntityId = table.Column<long>(type: "bigint", nullable: false),
                    LanguageId = table.Column<long>(type: "bigint", nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TranslationStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfileRequisiteLocalizations", x => new { x.EntityId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_CompanyProfileRequisiteLocalizations_CompanyProfileRequisites_EntityId",
                        column: x => x.EntityId,
                        principalTable: "CompanyProfileRequisites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProfileRequisiteLocalizations_LocalizationLanguages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "LocalizationLanguages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            
            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileContactLocalizations_LanguageId",
                table: "CompanyProfileContactLocalizations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileContacts_ProfileId",
                table: "CompanyProfileContacts",
                column: "ProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileRequisiteLocalizations_LanguageId",
                table: "CompanyProfileRequisiteLocalizations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileRequisites_ProfileId",
                table: "CompanyProfileRequisites",
                column: "ProfileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfileSocialLinks_ProfileId_SocialPlatform",
                table: "CompanyProfileSocialLinks",
                columns: new[] { "ProfileId", "SocialPlatform" },
                unique: true);

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyProfileContactLocalizations");

            migrationBuilder.DropTable(
                name: "CompanyProfileRequisiteLocalizations");

            migrationBuilder.DropTable(
                name: "CompanyProfileSocialLinks");

            migrationBuilder.DropTable(
                name: "CompanyProfileContacts");

            migrationBuilder.DropTable(
                name: "CompanyProfileRequisites");

            migrationBuilder.DropTable(
                name: "CompanyProfiles");
        }
    }
}
