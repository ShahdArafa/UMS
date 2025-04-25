using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class initialFacultySystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FacultyId",
                table: "CourseGroups",
                type: "int",
                nullable: false
                );

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_FacultyId",
                table: "CourseGroups",
                column: "FacultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_Faculties_FacultyId",
                table: "CourseGroups",
                column: "FacultyId",
                principalTable: "Faculties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_Faculties_FacultyId",
                table: "CourseGroups");

            migrationBuilder.DropIndex(
                name: "IX_CourseGroups_FacultyId",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "FacultyId",
                table: "CourseGroups");
        }
    }
}
