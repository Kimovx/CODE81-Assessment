using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CODE81_Assessment.Migrations
{
    /// <inheritdoc />
    public partial class Init_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransaction_AspNetUsers_CreatedById",
                table: "BorrowingTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransaction_AspNetUsers_ReturnedById",
                table: "BorrowingTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransaction_Books_BookId",
                table: "BorrowingTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransaction_LibraryMember_MemberId",
                table: "BorrowingTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LibraryMember",
                table: "LibraryMember");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowingTransaction",
                table: "BorrowingTransaction");

            migrationBuilder.RenameTable(
                name: "LibraryMember",
                newName: "LibraryMembers");

            migrationBuilder.RenameTable(
                name: "BorrowingTransaction",
                newName: "BorrowingTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransaction_ReturnedById",
                table: "BorrowingTransactions",
                newName: "IX_BorrowingTransactions_ReturnedById");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransaction_MemberId",
                table: "BorrowingTransactions",
                newName: "IX_BorrowingTransactions_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransaction_CreatedById",
                table: "BorrowingTransactions",
                newName: "IX_BorrowingTransactions_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransaction_BookId",
                table: "BorrowingTransactions",
                newName: "IX_BorrowingTransactions_BookId");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Books",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibraryMembers",
                table: "LibraryMembers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowingTransactions",
                table: "BorrowingTransactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserLoginLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLoginLogs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLoginLogs_UserId",
                table: "UserLoginLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransactions_AspNetUsers_CreatedById",
                table: "BorrowingTransactions",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransactions_AspNetUsers_ReturnedById",
                table: "BorrowingTransactions",
                column: "ReturnedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransactions_Books_BookId",
                table: "BorrowingTransactions",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransactions_LibraryMembers_MemberId",
                table: "BorrowingTransactions",
                column: "MemberId",
                principalTable: "LibraryMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransactions_AspNetUsers_CreatedById",
                table: "BorrowingTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransactions_AspNetUsers_ReturnedById",
                table: "BorrowingTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransactions_Books_BookId",
                table: "BorrowingTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowingTransactions_LibraryMembers_MemberId",
                table: "BorrowingTransactions");

            migrationBuilder.DropTable(
                name: "UserLoginLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LibraryMembers",
                table: "LibraryMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BorrowingTransactions",
                table: "BorrowingTransactions");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Books");

            migrationBuilder.RenameTable(
                name: "LibraryMembers",
                newName: "LibraryMember");

            migrationBuilder.RenameTable(
                name: "BorrowingTransactions",
                newName: "BorrowingTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransactions_ReturnedById",
                table: "BorrowingTransaction",
                newName: "IX_BorrowingTransaction_ReturnedById");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransactions_MemberId",
                table: "BorrowingTransaction",
                newName: "IX_BorrowingTransaction_MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransactions_CreatedById",
                table: "BorrowingTransaction",
                newName: "IX_BorrowingTransaction_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_BorrowingTransactions_BookId",
                table: "BorrowingTransaction",
                newName: "IX_BorrowingTransaction_BookId");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "AspNetUsers",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(256)",
                oldMaxLength: 256);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LibraryMember",
                table: "LibraryMember",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BorrowingTransaction",
                table: "BorrowingTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransaction_AspNetUsers_CreatedById",
                table: "BorrowingTransaction",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransaction_AspNetUsers_ReturnedById",
                table: "BorrowingTransaction",
                column: "ReturnedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransaction_Books_BookId",
                table: "BorrowingTransaction",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowingTransaction_LibraryMember_MemberId",
                table: "BorrowingTransaction",
                column: "MemberId",
                principalTable: "LibraryMember",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
