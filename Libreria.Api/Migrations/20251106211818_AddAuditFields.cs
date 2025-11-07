using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Libreria.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Precio",
                table: "Libros",
                type: "decimal(65,30)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Libros",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Libros",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Facturas",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Facturas",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "DetalleFacturas",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "DetalleFacturas",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Clientes",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Clientes",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacion",
                table: "Autores",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Autores",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Libros");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Facturas");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "DetalleFacturas");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "DetalleFacturas");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Clientes");

            migrationBuilder.DropColumn(
                name: "FechaActualizacion",
                table: "Autores");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Autores");

            migrationBuilder.AlterColumn<double>(
                name: "Precio",
                table: "Libros",
                type: "double",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)");
        }
    }
}
