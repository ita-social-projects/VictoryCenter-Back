using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VictoryCenter.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMetricTypeWithMetricKind : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Metrics",
                newName: "Kind");

            // Old MetricType: Partners = 0, Programs = 1, Raised = 2, TherapyHours = 3.
            // New MetricKind: Standard = 0, RaisedFunds = 1.
            migrationBuilder.Sql(
                """
                UPDATE Metrics
                SET Kind = CASE WHEN Kind = 2 THEN 1 ELSE 0 END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // The original business types of Standard metrics cannot be recovered exactly,
            // so they are reassigned Partners, Programs, TherapyHours in priority order.
            // RaisedFunds is converted first so that the reassigned Programs (1) values are not affected.
            migrationBuilder.Sql(
                """
                UPDATE Metrics
                SET Kind = 2
                WHERE Kind = 1;

                UPDATE m
                SET m.Kind = CASE ranked.RowNum WHEN 1 THEN 0 WHEN 2 THEN 1 ELSE 3 END
                FROM Metrics m
                INNER JOIN (
                    SELECT Id, ROW_NUMBER() OVER (PARTITION BY StatisticId ORDER BY Priority, Id) AS RowNum
                    FROM Metrics
                    WHERE Kind = 0
                ) ranked ON m.Id = ranked.Id;
                """);

            migrationBuilder.RenameColumn(
                name: "Kind",
                table: "Metrics",
                newName: "Type");
        }
    }
}
