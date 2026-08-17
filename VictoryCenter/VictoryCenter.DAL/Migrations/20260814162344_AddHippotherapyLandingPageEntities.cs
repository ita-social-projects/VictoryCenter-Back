using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddHippotherapyLandingPageEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPages",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageAdvantagesSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageAdvantagesSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageAdvantagesSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageAnalysisSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageAnalysisSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageAnalysisSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageAnotherQuoteSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    QuoteText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageAnotherQuoteSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageAnotherQuoteSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageAnotherQuoteSections_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageDescriptionSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageDescriptionSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageDescriptionSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageEthicsSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageEthicsSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageEthicsSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageEthicsSections_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageHippoventionCenterSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageHippoventionCenterSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionCenterSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionCenterSections_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageHippoventionSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageHippoventionSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageIntroSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageIntroSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageIntroSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageIntroSections_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageParticipantsSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageParticipantsSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageParticipantsSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageQuoteSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    QuoteText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageQuoteSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageQuoteSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageQuoteSections_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageScientificReferencesSections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippotherapyLandingPageId = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageScientificReferencesSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageScientificReferencesSections_HippotherapyLandingPages_HippotherapyLandingPageId",
                        column: x => x.HippotherapyLandingPageId,
                        principalTable: "HippotherapyLandingPages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageAdvantageCards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvantagesSectionId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageAdvantageCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageAdvantageCards_HippotherapyLandingPageAdvantagesSections_AdvantagesSectionId",
                        column: x => x.AdvantagesSectionId,
                        principalTable: "HippotherapyLandingPageAdvantagesSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageAdvantageCards_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageEthicsPrinciples",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EthicsSectionId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageEthicsPrinciples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageEthicsPrinciples_HippotherapyLandingPageEthicsSections_EthicsSectionId",
                        column: x => x.EthicsSectionId,
                        principalTable: "HippotherapyLandingPageEthicsSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageHippoventionPros",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HippoventionCenterSectionId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageHippoventionPros", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageHippoventionPros_HippotherapyLandingPageHippoventionCenterSections_HippoventionCenterSectionId",
                        column: x => x.HippoventionCenterSectionId,
                        principalTable: "HippotherapyLandingPageHippoventionCenterSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageParticipantCards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParticipantsSectionId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageId = table.Column<long>(type: "bigint", nullable: true),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageParticipantCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageParticipantCards_HippotherapyLandingPageParticipantsSections_ParticipantsSectionId",
                        column: x => x.ParticipantsSectionId,
                        principalTable: "HippotherapyLandingPageParticipantsSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageParticipantCards_Images_ImageId",
                        column: x => x.ImageId,
                        principalSchema: "media",
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "HippotherapyLandingPageScientificReferences",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScientificReferencesSectionId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priority = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HippotherapyLandingPageScientificReferences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HippotherapyLandingPageScientificReferences_HippotherapyLandingPageScientificReferencesSections_ScientificReferencesSectionId",
                        column: x => x.ScientificReferencesSectionId,
                        principalTable: "HippotherapyLandingPageScientificReferencesSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageAdvantageCards_AdvantagesSectionId_Priority",
                table: "HippotherapyLandingPageAdvantageCards",
                columns: new[] { "AdvantagesSectionId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageAdvantageCards_ImageId",
                table: "HippotherapyLandingPageAdvantageCards",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageAdvantagesSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageAdvantagesSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageAnalysisSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageAnalysisSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageAnotherQuoteSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageAnotherQuoteSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageAnotherQuoteSections_ImageId",
                table: "HippotherapyLandingPageAnotherQuoteSections",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageDescriptionSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageDescriptionSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageEthicsPrinciples_EthicsSectionId_Priority",
                table: "HippotherapyLandingPageEthicsPrinciples",
                columns: new[] { "EthicsSectionId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageEthicsSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageEthicsSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageEthicsSections_ImageId",
                table: "HippotherapyLandingPageEthicsSections",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageHippoventionCenterSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageHippoventionCenterSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageHippoventionCenterSections_ImageId",
                table: "HippotherapyLandingPageHippoventionCenterSections",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageHippoventionPros_HippoventionCenterSectionId_Priority",
                table: "HippotherapyLandingPageHippoventionPros",
                columns: new[] { "HippoventionCenterSectionId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageHippoventionSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageHippoventionSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageIntroSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageIntroSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageIntroSections_ImageId",
                table: "HippotherapyLandingPageIntroSections",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageParticipantCards_ImageId",
                table: "HippotherapyLandingPageParticipantCards",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageParticipantCards_ParticipantsSectionId_Priority",
                table: "HippotherapyLandingPageParticipantCards",
                columns: new[] { "ParticipantsSectionId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageParticipantsSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageParticipantsSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageQuoteSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageQuoteSections",
                column: "HippotherapyLandingPageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageQuoteSections_ImageId",
                table: "HippotherapyLandingPageQuoteSections",
                column: "ImageId",
                unique: true,
                filter: "[ImageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageScientificReferences_ScientificReferencesSectionId_Priority",
                table: "HippotherapyLandingPageScientificReferences",
                columns: new[] { "ScientificReferencesSectionId", "Priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HippotherapyLandingPageScientificReferencesSections_HippotherapyLandingPageId",
                table: "HippotherapyLandingPageScientificReferencesSections",
                column: "HippotherapyLandingPageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageAdvantageCards");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageAnalysisSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageAnotherQuoteSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageDescriptionSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageEthicsPrinciples");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageHippoventionPros");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageHippoventionSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageIntroSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageParticipantCards");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageQuoteSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageScientificReferences");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageAdvantagesSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageEthicsSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageHippoventionCenterSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageParticipantsSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPageScientificReferencesSections");

            migrationBuilder.DropTable(
                name: "HippotherapyLandingPages");
        }
    }
}
