using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiPrototype2.Migrations
{
    public partial class CurrentState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "uq_country_name",
                table: "Countries");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "uq_country_name",
                table: "Countries",
                column: "Name");
        }
    }
}
