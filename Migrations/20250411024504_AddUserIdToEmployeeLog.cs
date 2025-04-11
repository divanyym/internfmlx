using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcMovie.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToEmployeeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "EmployeeLogs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeLogs_UserId",
                table: "EmployeeLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLogs_Users_UserId",
                table: "EmployeeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLogs_Users_UserId",
                table: "EmployeeLogs");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeLogs_UserId",
                table: "EmployeeLogs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "EmployeeLogs");
        }
    }
}
