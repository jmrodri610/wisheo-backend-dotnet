using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisheo_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddWishlistCollaborators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "wishlist_collaborators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WishlistId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wishlist_collaborators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_wishlist_collaborators_Wishlists_WishlistId",
                        column: x => x.WishlistId,
                        principalTable: "Wishlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_wishlist_collaborators_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_collaborators_UserId",
                table: "wishlist_collaborators",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_wishlist_collaborators_WishlistId_UserId",
                table: "wishlist_collaborators",
                columns: new[] { "WishlistId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "wishlist_collaborators");
        }
    }
}
