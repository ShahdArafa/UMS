using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateproperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attendance",
                table: "AssignmentQuizzes");

            migrationBuilder.RenameColumn(
                name: "Tasks",
                table: "AssignmentQuizzes",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Quizzes",
                table: "AssignmentQuizzes",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Students",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "AssignmentQuizzes",
                newName: "Tasks");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "AssignmentQuizzes",
                newName: "Quizzes");

            migrationBuilder.AddColumn<int>(
                name: "Attendance",
                table: "AssignmentQuizzes",
                type: "int",
                nullable: true);
        }
    }
}
