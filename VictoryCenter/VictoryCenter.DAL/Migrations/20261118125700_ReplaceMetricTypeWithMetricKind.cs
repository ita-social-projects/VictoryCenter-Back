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
            // MetricTypeBackup is not part of the EF model. It keeps the original business types
            // so that Down can restore them exactly. It must stay in its own batch because it reads
            // the Type column before the rename.
            migrationBuilder.Sql(
                """
                CREATE TABLE MetricTypeBackup (
                    MetricId bigint NOT NULL PRIMARY KEY,
                    Type int NOT NULL
                );

                INSERT INTO MetricTypeBackup (MetricId, Type)
                SELECT Id, Type
                FROM Metrics;
                """);

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
            // RaisedFunds always maps back to Raised (2). Standard metrics are restored from
            // MetricTypeBackup; their original type cannot be derived from the data, so Down fails
            // instead of guessing when a Standard metric has no backup row (for example, metrics
            // created after the upgrade, or databases that applied an earlier version of this migration).
            // To roll back in that case, insert the original types into MetricTypeBackup and retry.
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'MetricTypeBackup', N'U') IS NULL
                BEGIN
                    IF EXISTS (SELECT 1 FROM Metrics WHERE Kind = 0)
                    BEGIN
                        THROW 50000, N'Cannot roll back ReplaceMetricTypeWithMetricKind: the original MetricType of Standard metrics is unknown. Create MetricTypeBackup (MetricId bigint PRIMARY KEY, Type int), insert their original types and retry.', 1;
                    END;

                    UPDATE Metrics
                    SET Kind = 2
                    WHERE Kind = 1;
                END
                ELSE
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM Metrics m
                        WHERE m.Kind = 0
                          AND NOT EXISTS (SELECT 1 FROM MetricTypeBackup b WHERE b.MetricId = m.Id))
                    BEGIN
                        THROW 50000, N'Cannot roll back ReplaceMetricTypeWithMetricKind: some Standard metrics have no row in MetricTypeBackup. Insert their original types and retry.', 1;
                    END;

                    UPDATE m
                    SET m.Kind = CASE WHEN m.Kind = 1 THEN 2 ELSE b.Type END
                    FROM Metrics m
                    LEFT JOIN MetricTypeBackup b ON b.MetricId = m.Id;

                    DROP TABLE MetricTypeBackup;
                END;
                """);

            migrationBuilder.RenameColumn(
                name: "Kind",
                table: "Metrics",
                newName: "Type");
        }
    }
}
