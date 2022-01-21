using Microsoft.EntityFrameworkCore.Migrations;

namespace BDD.Migrations
{
    public partial class BDDcreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Provincia",
                columns: table => new
                {
                    Provincia = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincia", x => x.Provincia);
                });

            migrationBuilder.CreateTable(
                name: "ZonaRegion",
                columns: table => new
                {
                    Zona = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonaRegion", x => x.Zona);
                });

            migrationBuilder.CreateTable(
                name: "Localidad",
                columns: table => new
                {
                    Localidad = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Latitud = table.Column<double>(type: "float", nullable: false),
                    Longitud = table.Column<double>(type: "float", nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zona = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZonaRegionFKZona = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProvinciaFKProvincia = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Localidad", x => x.Localidad);
                    table.ForeignKey(
                        name: "FK_Localidad_Provincia_ProvinciaFKProvincia",
                        column: x => x.ProvinciaFKProvincia,
                        principalTable: "Provincia",
                        principalColumn: "Provincia",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Localidad_ZonaRegion_ZonaRegionFKZona",
                        column: x => x.ZonaRegionFKZona,
                        principalTable: "ZonaRegion",
                        principalColumn: "Zona",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TemporalLocalidad",
                columns: table => new
                {
                    Localidad = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Temperatura = table.Column<double>(type: "float", nullable: false),
                    VelViento = table.Column<double>(type: "float", nullable: false),
                    Precipitaciones = table.Column<double>(type: "float", nullable: false),
                    Humedad = table.Column<double>(type: "float", nullable: false),
                    LocalidadFKLocalidad = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemporalLocalidad", x => x.Localidad);
                    table.ForeignKey(
                        name: "FK_TemporalLocalidad_Localidad_LocalidadFKLocalidad",
                        column: x => x.LocalidadFKLocalidad,
                        principalTable: "Localidad",
                        principalColumn: "Localidad",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_ProvinciaFKProvincia",
                table: "Localidad",
                column: "ProvinciaFKProvincia");

            migrationBuilder.CreateIndex(
                name: "IX_Localidad_ZonaRegionFKZona",
                table: "Localidad",
                column: "ZonaRegionFKZona");

            migrationBuilder.CreateIndex(
                name: "IX_TemporalLocalidad_LocalidadFKLocalidad",
                table: "TemporalLocalidad",
                column: "LocalidadFKLocalidad");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemporalLocalidad");

            migrationBuilder.DropTable(
                name: "Localidad");

            migrationBuilder.DropTable(
                name: "Provincia");

            migrationBuilder.DropTable(
                name: "ZonaRegion");
        }
    }
}
