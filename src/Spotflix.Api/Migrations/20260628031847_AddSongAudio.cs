using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotflix.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSongAudio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "AudioData",
                table: "Songs",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Songs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AudioData",
                table: "Songs");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Songs");
        }
    }
}
