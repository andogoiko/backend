using Microsoft.EntityFrameworkCore.Migrations;

namespace BDD.Migrations
{
    public partial class SeptuagesimaCreacionDeLaBDD : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Provincia = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Provincia);
                });

            migrationBuilder.CreateTable(
                name: "Localidades",
                columns: table => new
                {
                    Localidad = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Baliza = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitud = table.Column<double>(type: "float", nullable: true),
                    Longitud = table.Column<double>(type: "float", nullable: true),
                    Provincia = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidades", x => x.Localidad);
                    table.ForeignKey(
                        name: "FK_Localidades_Provincias_Provincia",
                        column: x => x.Provincia,
                        principalTable: "Provincias",
                        principalColumn: "Provincia",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemporalLocalidades",
                columns: table => new
                {
                    Localidad = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Temperatura = table.Column<double>(type: "float", nullable: true),
                    VelViento = table.Column<double>(type: "float", nullable: true),
                    Precipitaciones = table.Column<double>(type: "float", nullable: true),
                    Humedad = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalLocalidades", x => x.Localidad);
                    table.ForeignKey(
                        name: "FK_TemporalLocalidades_Localidades_Localidad",
                        column: x => x.Localidad,
                        principalTable: "Localidades",
                        principalColumn: "Localidad",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Localidades_Provincia",
                table: "Localidades",
                column: "Provincia");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporalLocalidades");

            migrationBuilder.DropTable(
                name: "Localidades");

            migrationBuilder.DropTable(
                name: "Provincias");
        }
    }
}
