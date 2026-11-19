using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentViewer.Infrastructure.Sql.Migrations
{
    /// <inheritdoc />
    public partial class ContentManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "ComponentIds",
                schema: "dbo");

            migrationBuilder.CreateSequence(
                name: "DocumentIds",
                schema: "dbo");

            migrationBuilder.CreateSequence(
                name: "NavigationIds",
                schema: "dbo");

            migrationBuilder.CreateSequence(
                name: "VersionIds",
                schema: "dbo");

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PublishedVersionNumber = table.Column<int>(type: "int", nullable: false),
                    HasUnpublishedChanges = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentComponents",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentComponents_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentVersions",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    VersionNumber = table.Column<int>(type: "int", nullable: false),
                    PublishedBy = table.Column<long>(type: "bigint", nullable: false),
                    PublishedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentVersions_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NavigationItems",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true),
                    DocumentId = table.Column<long>(type: "bigint", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ExternalUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsVisible = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavigationItems_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "dbo",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationItems_NavigationItems_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "NavigationItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentComponents_DocumentId_Order",
                schema: "dbo",
                table: "DocumentComponents",
                columns: new[] { "DocumentId", "Order" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_Slug",
                schema: "dbo",
                table: "Documents",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentVersions_DocumentId_VersionNumber",
                schema: "dbo",
                table: "DocumentVersions",
                columns: new[] { "DocumentId", "VersionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_DocumentId",
                schema: "dbo",
                table: "NavigationItems",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationItems_ParentId_Order",
                schema: "dbo",
                table: "NavigationItems",
                columns: new[] { "ParentId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentComponents",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DocumentVersions",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NavigationItems",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "ComponentIds",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "DocumentIds",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "NavigationIds",
                schema: "dbo");

            migrationBuilder.DropSequence(
                name: "VersionIds",
                schema: "dbo");
        }
    }
}
