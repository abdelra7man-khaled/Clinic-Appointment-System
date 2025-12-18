using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinic.DataAccess.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Specialties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Cardiology" },
                    { 2, "Dermatology" },
                    { 3, "Neurology" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role", "Username" },
                values: new object[,]
                {
                    { 1, "admin@clinic.com", "admin-hash", 2, "admin" },
                    { 2, "abdelrahman@clinic.com", "123456", 1, "abdelrahman" },
                    { 3, "ahmed@clinic.com", "123456", 0, "ahmed" },
                    { 4, "amr@clinic.com", "123456", 1, "amr" },
                    { 5, "samy@clinic.com", "123456", 1, "samy" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "Biography", "FullName", "UserId" },
                values: new object[,]
                {
                    { 1, "Senior Cardiologist", "Abdelrahman Khaled", 2 },
                    { 2, "Senior Dermatologist", "Amr Sobhy", 4 }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "Id", "Balance", "FullName", "PhoneNumber", "UserId" },
                values: new object[,]
                {
                    { 1, 3700m, "Samy Mohamed", "01123456789", 3 },
                    { 2, 2500m, "Ahmed Ali", "01123856109", 5 }
                });

            migrationBuilder.InsertData(
                table: "Appointments",
                columns: new[] { "Id", "AppointmentType", "DoctorId", "EndTime", "Fee", "Notes", "PatientId", "StartTime", "Status" },
                values: new object[,]
                {
                    { 1, 0, 1, new DateTime(2025, 1, 20, 10, 30, 0, 0, DateTimeKind.Unspecified), 300m, "Initial checkup", 1, new DateTime(2025, 1, 20, 10, 0, 0, 0, DateTimeKind.Unspecified), 0 },
                    { 2, 2, 2, new DateTime(2025, 5, 17, 8, 30, 0, 0, DateTimeKind.Unspecified), 100m, "Follow Up Appointment", 2, new DateTime(2025, 5, 17, 8, 0, 0, 0, DateTimeKind.Unspecified), 0 }
                });

            migrationBuilder.InsertData(
                table: "DoctorSpecialties",
                columns: new[] { "DoctorId", "SpecialtyId", "SpecialtyId1" },
                values: new object[,]
                {
                    { 1, 1, null },
                    { 2, 2, null }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "AppointmentId", "PaidAt", "PatientId", "PaymentMethod" },
                values: new object[,]
                {
                    { 1, 300m, 1, new DateTime(2025, 1, 20, 9, 55, 0, 0, DateTimeKind.Unspecified), null, 0 },
                    { 2, 100m, 2, new DateTime(2025, 5, 15, 6, 40, 0, 0, DateTimeKind.Unspecified), null, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DoctorSpecialties",
                keyColumns: new[] { "DoctorId", "SpecialtyId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "DoctorSpecialties",
                keyColumns: new[] { "DoctorId", "SpecialtyId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Appointments",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Specialties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
