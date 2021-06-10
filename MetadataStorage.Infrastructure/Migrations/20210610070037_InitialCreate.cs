using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MetadataStorage.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityMetadataDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityMetadataDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetadataFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FieldType = table.Column<int>(type: "int", nullable: false),
                    PublicName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsMandatory = table.Column<bool>(type: "bit", nullable: false),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MetadataSchemaDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataSchemaDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetadataSchemaDetails_EntityMetadataDetails_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EntityMetadataDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityMetadataFieldValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TenantId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityMetadataFieldValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityMetadataFieldValues_EntityMetadataDetails_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityMetadataDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntityMetadataFieldValues_MetadataFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "MetadataFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityMetadataSchemas",
                columns: table => new
                {
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityMetadataSchemas", x => new { x.SubjectId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_EntityMetadataSchemas_MetadataSchemaDetails_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "MetadataSchemaDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetadataSchemaFields",
                columns: table => new
                {
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetadataSchemaFields", x => new { x.FieldId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_MetadataSchemaFields_MetadataFields_FieldId",
                        column: x => x.FieldId,
                        principalTable: "MetadataFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MetadataSchemaFields_MetadataSchemaDetails_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "MetadataSchemaDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataDetails_EntityName",
                table: "EntityMetadataDetails",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataDetails_Id",
                table: "EntityMetadataDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataDetails_TenantId",
                table: "EntityMetadataDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataFieldValues_EntityTypeId",
                table: "EntityMetadataFieldValues",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataFieldValues_FieldId",
                table: "EntityMetadataFieldValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataFieldValues_SubjectId",
                table: "EntityMetadataFieldValues",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataFieldValues_TenantId",
                table: "EntityMetadataFieldValues",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataSchemas_SchemaId",
                table: "EntityMetadataSchemas",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityMetadataSchemas_SubjectId",
                table: "EntityMetadataSchemas",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataFields_FieldName",
                table: "MetadataFields",
                column: "FieldName");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataFields_Id",
                table: "MetadataFields",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataFields_PublicName",
                table: "MetadataFields",
                column: "PublicName");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataFields_TenantId",
                table: "MetadataFields",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataSchemaDetails_EntityId",
                table: "MetadataSchemaDetails",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataSchemaDetails_Id",
                table: "MetadataSchemaDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataSchemaDetails_SchemaName",
                table: "MetadataSchemaDetails",
                column: "SchemaName");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataSchemaDetails_TenantId",
                table: "MetadataSchemaDetails",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataSchemaFields_FieldId",
                table: "MetadataSchemaFields",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_MetadataSchemaFields_SchemaId",
                table: "MetadataSchemaFields",
                column: "SchemaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityMetadataFieldValues");

            migrationBuilder.DropTable(
                name: "EntityMetadataSchemas");

            migrationBuilder.DropTable(
                name: "MetadataSchemaFields");

            migrationBuilder.DropTable(
                name: "MetadataFields");

            migrationBuilder.DropTable(
                name: "MetadataSchemaDetails");

            migrationBuilder.DropTable(
                name: "EntityMetadataDetails");
        }
    }
}
