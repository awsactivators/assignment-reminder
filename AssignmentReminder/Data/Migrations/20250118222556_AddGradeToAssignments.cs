using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssignmentReminder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGradeToAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Grade",
                table: "Assignments",
                type: "REAL",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Assignments");
        }
    }
}
