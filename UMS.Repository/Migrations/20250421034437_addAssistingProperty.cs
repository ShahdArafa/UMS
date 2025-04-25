using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class addAssistingProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeachingAssistantId",
                table: "CourseGroups",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseGroups_TeachingAssistantId",
                table: "CourseGroups",
                column: "TeachingAssistantId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseGroups_Faculties_TeachingAssistantId",
                table: "CourseGroups",
                column: "TeachingAssistantId",
                principalTable: "Faculties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseGroups_Faculties_TeachingAssistantId",
                table: "CourseGroups");

            migrationBuilder.DropIndex(
                name: "IX_CourseGroups_TeachingAssistantId",
                table: "CourseGroups");

            migrationBuilder.DropColumn(
                name: "TeachingAssistantId",
                table: "CourseGroups");
        }
    }
}
