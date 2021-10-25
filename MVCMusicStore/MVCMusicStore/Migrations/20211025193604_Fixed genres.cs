using Microsoft.EntityFrameworkCore.Migrations;

namespace MVCMusicStore.Migrations
{
    public partial class Fixedgenres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_Genres_GenreId1",
                table: "Genres");

            migrationBuilder.DropIndex(
                name: "IX_Genres_GenreId1",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "GenreId1",
                table: "Genres");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GenreId1",
                table: "Genres",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Genres_GenreId1",
                table: "Genres",
                column: "GenreId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_Genres_GenreId1",
                table: "Genres",
                column: "GenreId1",
                principalTable: "Genres",
                principalColumn: "GenreId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
