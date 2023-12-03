using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NegotiationsApi.Migrations
{
    /// <inheritdoc />
    public partial class NStatusAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NegotiationModel",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "NegotiationModel");
        }
    }
}
