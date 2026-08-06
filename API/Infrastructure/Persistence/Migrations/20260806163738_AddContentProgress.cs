using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContentProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "coins",
                table: "students");

            migrationBuilder.CreateTable(
                name: "content_progress",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_id = table.Column<Guid>(type: "uuid", nullable: false),
                    course_id = table.Column<Guid>(type: "uuid", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_content_progress", x => x.id);
                    table.ForeignKey(
                        name: "fk_content_progress_contents_content_id",
                        column: x => x.content_id,
                        principalTable: "contents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_progress_courses_course_id",
                        column: x => x.course_id,
                        principalTable: "courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_content_progress_students_student_id",
                        column: x => x.student_id,
                        principalTable: "students",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_content_progress_content_id",
                table: "content_progress",
                column: "content_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_progress_course_id",
                table: "content_progress",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "ix_content_progress_student_id_content_id",
                table: "content_progress",
                columns: new[] { "student_id", "content_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_content_progress_student_id_course_id",
                table: "content_progress",
                columns: new[] { "student_id", "course_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_progress");

            migrationBuilder.AddColumn<int>(
                name: "coins",
                table: "students",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
