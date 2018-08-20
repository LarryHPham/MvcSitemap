using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcSitemap.Migrations
{
    public partial class AddTemp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempChangeFrequency",
                table: "Sitemap",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TempModifiedDate",
                table: "Sitemap",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "TempPriority",
                table: "Sitemap",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempChangeFrequency",
                table: "Sitemap");

            migrationBuilder.DropColumn(
                name: "TempModifiedDate",
                table: "Sitemap");

            migrationBuilder.DropColumn(
                name: "TempPriority",
                table: "Sitemap");
        }
    }
}
