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

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Appointments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
