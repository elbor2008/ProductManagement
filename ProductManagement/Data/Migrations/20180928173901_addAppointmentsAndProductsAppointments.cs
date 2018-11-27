using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductManagement.Data.Migrations
{
    public partial class addAppointmentsAndProductsAppointments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AppointmentDate = table.Column<DateTime>(nullable: false),
                    CustomerName = table.Column<string>(nullable: true),
                    CustomerPhoneNumber = table.Column<string>(nullable: true),
                    CustomerEmail = table.Column<string>(nullable: true),
                    IsConfirmed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductsAppointments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProductId = table.Column<int>(nullable: false),
                    AppointmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductsAppointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductsAppointments_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductsAppointments_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductsAppointments_AppointmentId",
                table: "ProductsAppointments",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductsAppointments_ProductId",
                table: "ProductsAppointments",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductsAppointments");

            migrationBuilder.DropTable(
                name: "Appointments");
        }
    }
}
