using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCourseAndContentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "intro_video_url",
                table: "courses",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "language",
                table: "courses",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "English");

            migrationBuilder.AddColumn<List<string>>(
                name: "requirements",
                table: "courses",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");

            migrationBuilder.AddColumn<List<string>>(
                name: "what_you_will_learn",
                table: "courses",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "intro_video_url",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "language",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "requirements",
                table: "courses");

            migrationBuilder.DropColumn(
                name: "what_you_will_learn",
                table: "courses");
        }
    }
}
