using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buildify.Repository.Identity.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove any user-role associations with the "User" role
            migrationBuilder.Sql(
                @"DELETE FROM AspNetUserRoles 
                  WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'USER')");

            // Delete the "User" role
            migrationBuilder.Sql(
                @"DELETE FROM AspNetRoles WHERE NormalizedName = 'USER'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-create the "User" role if migration is rolled back
            migrationBuilder.Sql(
                @"INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
                  VALUES (NEWID(), 'User', 'USER', NULL)");
        }
    }
}
