using Microsoft.EntityFrameworkCore.Migrations;

namespace webapp.API.Migrations
{
    public partial class addedAddEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adds",
                columns: table => new
                {
                    AdderId = table.Column<int>(nullable: false),
                    AddedId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adds", x => new { x.AdderId, x.AddedId });
                    table.ForeignKey(
                        name: "FK_Adds_Users_AddedId",
                        column: x => x.AddedId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Adds_Users_AdderId",
                        column: x => x.AdderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adds_AddedId",
                table: "Adds",
                column: "AddedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adds");
        }
    }
}
