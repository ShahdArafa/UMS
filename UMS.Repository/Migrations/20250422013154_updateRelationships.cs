using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments");

            migrationBuilder.AddColumn<int>(
                name: "CourseGroupId",
                table: "Enrollments",
                type: "int",
                nullable: false
                );

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseGroupId",
                table: "Enrollments",
                column: "CourseGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId",
                table: "Enrollments",
                column: "CourseGroupId",
                principalTable: "CourseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseGroupId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CourseGroupId",
                table: "Enrollments");

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_EnrollmentId",
                table: "Assignments",
                column: "EnrollmentId",
                unique: true,
                filter: "[EnrollmentId] IS NOT NULL");
        }
    }
}
