using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorTransactionsAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_transactions_ticker",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "ticker",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "type",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "unit_price_currency",
                table: "transactions",
                newName: "price_currency");

            migrationBuilder.RenameColumn(
                name: "unit_price_amount",
                table: "transactions",
                newName: "price_amount");

            migrationBuilder.RenameColumn(
                name: "executed_at",
                table: "transactions",
                newName: "created_at");

            migrationBuilder.AlterColumn<string>(
                name: "price_currency",
                table: "transactions",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3,
                oldDefaultValue: "BRL");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_asset_id",
                table: "transactions",
                column: "asset_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_assets_asset_id",
                table: "transactions",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_assets_asset_id",
                table: "transactions");

            migrationBuilder.DropIndex(
                name: "ix_transactions_asset_id",
                table: "transactions");

            migrationBuilder.RenameColumn(
                name: "price_currency",
                table: "transactions",
                newName: "unit_price_currency");

            migrationBuilder.RenameColumn(
                name: "price_amount",
                table: "transactions",
                newName: "unit_price_amount");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "transactions",
                newName: "executed_at");

            migrationBuilder.AlterColumn<string>(
                name: "unit_price_currency",
                table: "transactions",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "BRL",
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<string>(
                name: "ticker",
                table: "transactions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "type",
                table: "transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_ticker",
                table: "transactions",
                column: "ticker");
        }
    }
}
