using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class makeCourseGroupIdNullableInEnrollments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId",
                table: "Enrollments");

            migrationBuilder.AlterColumn<int>(
                name: "CourseGroupId",
                table: "Enrollments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CourseGroupId1",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CourseGroupId1",
                table: "Enrollments",
                column: "CourseGroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId",
                table: "Enrollments",
                column: "CourseGroupId",
                principalTable: "CourseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId1",
                table: "Enrollments",
                column: "CourseGroupId1",
                principalTable: "CourseGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId1",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CourseGroupId1",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CourseGroupId1",
                table: "Enrollments");

            migrationBuilder.AlterColumn<int>(
                name: "CourseGroupId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_CourseGroups_CourseGroupId",
                table: "Enrollments",
                column: "CourseGroupId",
                principalTable: "CourseGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
