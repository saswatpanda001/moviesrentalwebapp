using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesRental.Migrations
{
    /// <inheritdoc />
    public partial class validations1_new1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Users_Email_Valid",
                table: "Users",
                sql: "[Email] LIKE '%_@__%.__%'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Users_Email_Valid",
                table: "Users");
        }
    }
}
