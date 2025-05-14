using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraVerse.Migrations
{
    /// <inheritdoc />
    public partial class FurtherDbChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Books_BookId",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_CreatedBy",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_DeletedBy",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_UpdatedBy",
                table: "Cart");

            migrationBuilder.DropForeignKey(
                name: "FK_Cart_Users_UserId",
                table: "Cart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cart",
                table: "Cart");

            migrationBuilder.RenameTable(
                name: "Cart",
                newName: "Carts");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_UserId",
                table: "Carts",
                newName: "IX_Carts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_UpdatedBy",
                table: "Carts",
                newName: "IX_Carts_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_DeletedBy",
                table: "Carts",
                newName: "IX_Carts_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_CreatedBy",
                table: "Carts",
                newName: "IX_Carts_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Cart_BookId",
                table: "Carts",
                newName: "IX_Carts_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Carts",
                table: "Carts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Books_BookId",
                table: "Carts",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_CreatedBy",
                table: "Carts",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_DeletedBy",
                table: "Carts",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UpdatedBy",
                table: "Carts",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Books_BookId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_CreatedBy",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_DeletedBy",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UpdatedBy",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_UserId",
                table: "Carts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Carts",
                table: "Carts");

            migrationBuilder.RenameTable(
                name: "Carts",
                newName: "Cart");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_UserId",
                table: "Cart",
                newName: "IX_Cart_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_UpdatedBy",
                table: "Cart",
                newName: "IX_Cart_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_DeletedBy",
                table: "Cart",
                newName: "IX_Cart_DeletedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_CreatedBy",
                table: "Cart",
                newName: "IX_Cart_CreatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Carts_BookId",
                table: "Cart",
                newName: "IX_Cart_BookId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cart",
                table: "Cart",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Books_BookId",
                table: "Cart",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_CreatedBy",
                table: "Cart",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_DeletedBy",
                table: "Cart",
                column: "DeletedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_UpdatedBy",
                table: "Cart",
                column: "UpdatedBy",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cart_Users_UserId",
                table: "Cart",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
