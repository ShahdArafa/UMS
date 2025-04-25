using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UMS.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TermResults");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TermResults");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "TermResults");

            migrationBuilder.DropColumn(
                name: "TermDate",
                table: "TermResults");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TermResults");

            migrationBuilder.AddColumn<int>(
                name: "TermId1",
                table: "TermResults",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Units",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateTable(
                name: "CourseResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TermResultId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Units = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseResults_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseResults_TermResults_TermResultId",
                        column: x => x.TermResultId,
                        principalTable: "TermResults",
                        principalColumn: "TermResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TermResults_TermId1",
                table: "TermResults",
                column: "TermId1");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResults_CourseId",
                table: "CourseResults",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseResults_TermResultId",
                table: "CourseResults",
                column: "TermResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_TermResults_Terms_TermId1",
                table: "TermResults",
                column: "TermId1",
                principalTable: "Terms",
                principalColumn: "TermId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TermResults_Terms_TermId1",
                table: "TermResults");

            migrationBuilder.DropTable(
                name: "CourseResults");

            migrationBuilder.DropIndex(
                name: "IX_TermResults_TermId1",
                table: "TermResults");

            migrationBuilder.DropColumn(
                name: "TermId1",
                table: "TermResults");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TermResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TermResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "TermResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TermDate",
                table: "TermResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TermResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "Units",
                table: "Courses",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3);
        }
    }
}
