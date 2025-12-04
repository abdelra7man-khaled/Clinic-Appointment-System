using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatePatientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Patients");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Patients",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Patients");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: true);
        }
    }
}
