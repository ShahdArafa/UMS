using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseGroupclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroup_Courses_CourseId",
                table: "CourseGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseGroup",
                table: "CourseGroup");

            migrationBuilder.RenameTable(
                name: "CourseGroup",
                newName: "CourseGroups");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGroup_CourseId",
                table: "CourseGroups",
                newName: "IX_CourseGroups_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseGroups",
                table: "CourseGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_Courses_CourseId",
                table: "CourseGroups",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_Courses_CourseId",
                table: "CourseGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseGroups",
                table: "CourseGroups");

            migrationBuilder.RenameTable(
                name: "CourseGroups",
                newName: "CourseGroup");

            migrationBuilder.RenameIndex(
                name: "IX_CourseGroups_CourseId",
                table: "CourseGroup",
                newName: "IX_CourseGroup_CourseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseGroup",
                table: "CourseGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroup_Courses_CourseId",
                table: "CourseGroup",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
