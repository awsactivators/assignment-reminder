using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssignmentReminder.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Assignments",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Assignments");
        }
    }
}
