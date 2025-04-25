using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class makeenrollmentidNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Enrollments_EnrollmentId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "Assignments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments",
                column: "EnrollmentId",
                unique: true,
                filter: "[EnrollmentId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Enrollments_EnrollmentId",
                table: "Assignments",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Enrollments_EnrollmentId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments");

            migrationBuilder.AlterColumn<int>(
                name: "EnrollmentId",
                table: "Assignments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Enrollments_EnrollmentId",
                table: "Assignments",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
