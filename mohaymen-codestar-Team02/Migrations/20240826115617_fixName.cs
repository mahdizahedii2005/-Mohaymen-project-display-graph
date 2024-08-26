using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mohaymen_codestar_Team02.Migrations
{
    /// <inheritdoc />
    public partial class fixName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VertexEntities",
                newName: "VertexEntityId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "EdgeEntities",
                newName: "EdgeEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VertexEntityId",
                table: "VertexEntities",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "EdgeEntityId",
                table: "EdgeEntities",
                newName: "Id");
        }
    }
}
