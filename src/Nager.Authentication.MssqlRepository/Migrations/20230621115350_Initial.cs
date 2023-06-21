using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nager.Authentication.MssqlRepository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Firstname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Lastname = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RolesData = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(16)", maxLength: 16, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmailAddress",
                table: "Users",
                column: "EmailAddress");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
