using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoleOfSamyInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Role",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                column: "Role",
                value: 1);
        }
    }
}
