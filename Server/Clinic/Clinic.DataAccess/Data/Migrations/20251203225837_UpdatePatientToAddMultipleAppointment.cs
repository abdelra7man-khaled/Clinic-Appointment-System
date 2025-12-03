using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatientToAddMultipleAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Patients",
                newName: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Patients",
                newName: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientId",
                table: "Appointments",
                column: "PatientId",
                unique: true);
        }
    }
}
