using Microsoft.EntityFrameworkCore.Migrations;

namespace BDD.Migrations
{
    public partial class quadragesimanovenamigracion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "VelVientoMax",
                table: "TemporalLocalidades",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VelVientoMax",
                table: "TemporalLocalidades");
        }
    }
}
