using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateAssignmentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseGroupId",
                table: "Assignments",
                type: "int",
                nullable: false
                );

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseGroupId",
                table: "Assignments",
                column: "CourseGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_CourseGroups_CourseGroupId",
                table: "Assignments",
                column: "CourseGroupId",
                principalTable: "CourseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_CourseGroups_CourseGroupId",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CourseGroupId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CourseGroupId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Assignments");
        }
    }
}
