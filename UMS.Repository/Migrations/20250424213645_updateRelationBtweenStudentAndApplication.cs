using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateRelationBtweenStudentAndApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Students_StudentId",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_StudentId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "Students",
                type: "int",
                nullable: false
                
                );

            migrationBuilder.AddColumn<string>(
                name: "StudentIdentifier",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ApplicationId",
                table: "Students",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Applications_ApplicationId",
                table: "Students",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Applications_ApplicationId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ApplicationId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "StudentIdentifier",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Applications_StudentId",
                table: "Applications",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Students_StudentId",
                table: "Applications",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
