using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiPrototype2.Migrations
{
    public partial class CurrentModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "uq_state_name",
                table: "States");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "uq_state_name",
                table: "States",
                column: "Name");
        }
    }
}
