using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisheo_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddEmojiToWishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Emoji",
                table: "Wishlists",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Emoji",
                table: "Wishlists");
        }
    }
}
