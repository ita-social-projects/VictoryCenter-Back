using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDonationsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForeignBankDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    SwiftCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(29)", maxLength: 29, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignBankDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Currency = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UahBankDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Edrpou = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PaymentPurpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Recipient = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(29)", maxLength: 29, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UahBankDetails", x => x.Id);
                });
            
            migrationBuilder.CreateTable(
                name: "CorrespondentBanks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ForeignBankDetailsId = table.Column<long>(type: "bigint", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Account = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SwiftCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrespondentBanks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrespondentBanks_ForeignBankDetails_ForeignBankDetailsId",
                        column: x => x.ForeignBankDetailsId,
                        principalTable: "ForeignBankDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalFields",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UahBankDetailsId = table.Column<long>(type: "bigint", nullable: true),
                    ForeignBankDetailsId = table.Column<long>(type: "bigint", nullable: true),
                    SupportMethodId = table.Column<long>(type: "bigint", nullable: true),
                    FieldName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalFields_ForeignBankDetails_ForeignBankDetailsId",
                        column: x => x.ForeignBankDetailsId,
                        principalTable: "ForeignBankDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalFields_SupportMethods_SupportMethodId",
                        column: x => x.SupportMethodId,
                        principalTable: "SupportMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalFields_UahBankDetails_UahBankDetailsId",
                        column: x => x.UahBankDetailsId,
                        principalTable: "UahBankDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_AdditionalFields_ForeignBankDetailsId",
                table: "AdditionalFields",
                column: "ForeignBankDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalFields_SupportMethodId",
                table: "AdditionalFields",
                column: "SupportMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalFields_UahBankDetailsId",
                table: "AdditionalFields",
                column: "UahBankDetailsId");
            
            migrationBuilder.CreateIndex(
                name: "IX_CorrespondentBanks_ForeignBankDetailsId",
                table: "CorrespondentBanks",
                column: "ForeignBankDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalFields");
            
            migrationBuilder.DropTable(
                name: "CorrespondentBanks");

            migrationBuilder.DropTable(
                name: "SupportMethods");

            migrationBuilder.DropTable(
                name: "UahBankDetails");

            migrationBuilder.DropTable(
                name: "ForeignBankDetails");
        }
    }
}
