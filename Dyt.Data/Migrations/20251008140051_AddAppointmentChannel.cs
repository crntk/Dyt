using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dyt.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ensure Channel column exists on Appointments (older DBs won't have it)
            migrationBuilder.Sql(@"IF COL_LENGTH('Appointments','Channel') IS NULL 
ALTER TABLE [Appointments] ADD [Channel] nvarchar(max) NOT NULL CONSTRAINT DF_Appointments_Channel DEFAULT('');
IF COL_LENGTH('Appointments','Channel') IS NOT NULL 
    ALTER TABLE [Appointments] DROP CONSTRAINT DF_Appointments_Channel;", suppressTransaction: false);

            migrationBuilder.DropTable(
                name: "WorkingHourTemplates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SmsTemplates",
                table: "SmsTemplates");

            migrationBuilder.RenameTable(
                name: "SmsTemplates",
                newName: "SmsTemplate");

            migrationBuilder.RenameIndex(
                name: "IX_SmsTemplates_TemplateKey",
                table: "SmsTemplate",
                newName: "IX_SmsTemplate_TemplateKey");

            // Keep column as nvarchar(max); if it already existed with nvarchar(20), widen it
            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SmsTemplate",
                table: "SmsTemplate",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SmsTemplate",
                table: "SmsTemplate");

            migrationBuilder.RenameTable(
                name: "SmsTemplate",
                newName: "SmsTemplates");

            migrationBuilder.RenameIndex(
                name: "IX_SmsTemplate_TemplateKey",
                table: "SmsTemplates",
                newName: "IX_SmsTemplates_TemplateKey");

            // If you actually revert, try to narrow type back. If column doesn't exist, skip via raw SQL.
            migrationBuilder.Sql(@"IF COL_LENGTH('Appointments','Channel') IS NOT NULL 
BEGIN
    ALTER TABLE [Appointments] ALTER COLUMN [Channel] nvarchar(20) NOT NULL;
END");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SmsTemplates",
                table: "SmsTemplates",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "WorkingHourTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    SlotMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingHourTemplates", x => x.Id);
                });
        }
    }
}
