using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class addMultiplePaymentsToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PatientId",
                table: "Payments",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Patients_PatientId",
                table: "Payments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Patients_PatientId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_PatientId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "Payments");
        }
    }
}
