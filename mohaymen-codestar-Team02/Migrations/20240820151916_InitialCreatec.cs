using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace mohaymen_codestar_Team02.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    LastName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    Salt = table.Column<byte[]>(type: "bytea", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "DataSets",
                columns: table => new
                {
                    DataGroupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdateAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSets", x => x.DataGroupId);
                    table.ForeignKey(
                        name: "FK_DataSets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EdgeEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DataGroupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgeEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdgeEntities_DataSets_DataGroupId",
                        column: x => x.DataGroupId,
                        principalTable: "DataSets",
                        principalColumn: "DataGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VertexEntities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DataGroupId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VertexEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VertexEntities_DataSets_DataGroupId",
                        column: x => x.DataGroupId,
                        principalTable: "DataSets",
                        principalColumn: "DataGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EdgeAttributes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    EdgeEntityId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgeAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdgeAttributes_EdgeEntities_EdgeEntityId",
                        column: x => x.EdgeEntityId,
                        principalTable: "EdgeEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VertexAttributes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    VertexEntityId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VertexAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VertexAttributes_VertexEntities_VertexEntityId",
                        column: x => x.VertexEntityId,
                        principalTable: "VertexEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EdgeValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StringValue = table.Column<string>(type: "text", nullable: false),
                    EdgeAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EdgeValues_EdgeAttributes_EdgeAttributeId",
                        column: x => x.EdgeAttributeId,
                        principalTable: "EdgeAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VertexValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StringValue = table.Column<string>(type: "text", nullable: false),
                    VertexAttributeId = table.Column<long>(type: "bigint", nullable: false),
                    ObjectId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VertexValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VertexValues_VertexAttributes_VertexAttributeId",
                        column: x => x.VertexAttributeId,
                        principalTable: "VertexAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSets_UserId",
                table: "DataSets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EdgeAttributes_EdgeEntityId",
                table: "EdgeAttributes",
                column: "EdgeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EdgeEntities_DataGroupId",
                table: "EdgeEntities",
                column: "DataGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EdgeValues_EdgeAttributeId",
                table: "EdgeValues",
                column: "EdgeAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VertexAttributes_VertexEntityId",
                table: "VertexAttributes",
                column: "VertexEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VertexEntities_DataGroupId",
                table: "VertexEntities",
                column: "DataGroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VertexValues_VertexAttributeId",
                table: "VertexValues",
                column: "VertexAttributeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EdgeValues");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VertexValues");

            migrationBuilder.DropTable(
                name: "EdgeAttributes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "VertexAttributes");

            migrationBuilder.DropTable(
                name: "EdgeEntities");

            migrationBuilder.DropTable(
                name: "VertexEntities");

            migrationBuilder.DropTable(
                name: "DataSets");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
