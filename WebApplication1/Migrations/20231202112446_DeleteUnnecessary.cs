using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NegotiationsApi.Migrations
{
    /// <inheritdoc />
    public partial class DeleteUnnecessary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accepted",
                table: "NegotiationModel");

            migrationBuilder.DropColumn(
                name: "BasePrice",
                table: "NegotiationModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Accepted",
                table: "NegotiationModel",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "BasePrice",
                table: "NegotiationModel",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
