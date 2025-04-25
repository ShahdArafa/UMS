using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class testmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignmentQuizzes_Enrollments_EnrollmentId",
                table: "AssignmentQuizzes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignmentQuizzes",
                table: "AssignmentQuizzes");

            migrationBuilder.RenameTable(
                name: "AssignmentQuizzes",
                newName: "Assignment");

            migrationBuilder.RenameIndex(
                name: "IX_AssignmentQuizzes_EnrollmentId",
                table: "Assignment",
                newName: "IX_Assignment_EnrollmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assignment",
                table: "Assignment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Quiz",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quiz", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Quiz_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quiz_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_CourseId",
                table: "Quiz",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Quiz_EnrollmentId",
                table: "Quiz",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Enrollments_EnrollmentId",
                table: "Assignment",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Enrollments_EnrollmentId",
                table: "Assignment");

            migrationBuilder.DropTable(
                name: "Quiz");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assignment",
                table: "Assignment");

            migrationBuilder.RenameTable(
                name: "Assignment",
                newName: "AssignmentQuizzes");

            migrationBuilder.RenameIndex(
                name: "IX_Assignment_EnrollmentId",
                table: "AssignmentQuizzes",
                newName: "IX_AssignmentQuizzes_EnrollmentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignmentQuizzes",
                table: "AssignmentQuizzes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignmentQuizzes_Enrollments_EnrollmentId",
                table: "AssignmentQuizzes",
                column: "EnrollmentId",
                principalTable: "Enrollments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
