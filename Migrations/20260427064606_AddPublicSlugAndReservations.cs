using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace wisheo_backend_v2.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicSlugAndReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicSlug",
                table: "Wishlists",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WishItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    GuestName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GuestEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CancelToken = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reservations_WishItems_WishItemId",
                        column: x => x.WishItemId,
                        principalTable: "WishItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservations_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_PublicSlug",
                table: "Wishlists",
                column: "PublicSlug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_CancelToken",
                table: "reservations",
                column: "CancelToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_UserId",
                table: "reservations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_WishItemId",
                table: "reservations",
                column: "WishItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_Wishlists_PublicSlug",
                table: "Wishlists");

            migrationBuilder.DropColumn(
                name: "PublicSlug",
                table: "Wishlists");
        }
    }
}
