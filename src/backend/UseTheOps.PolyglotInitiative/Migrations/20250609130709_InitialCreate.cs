using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UseTheOps.PolyglotInitiative.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    IsAdministrator = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalIdentifiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceType = table.Column<string>(type: "text", nullable: false),
                    FieldName = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    AddedOrModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalIdentifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalIdentifiers_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Solutions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    PresentationUrl = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solutions_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeyValue = table.Column<string>(type: "text", nullable: false),
                    Scope = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Origin = table.Column<string>(type: "text", nullable: true),
                    OriginUrl = table.Column<string>(type: "text", nullable: true),
                    ExternalIdentifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ExternalIdentifiers_ExternalIdentifierId",
                        column: x => x.ExternalIdentifierId,
                        principalTable: "ExternalIdentifiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationNeeds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationNeeds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationNeeds_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSolutionAccesses",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AccessLevel = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSolutionAccesses", x => new { x.UserId, x.SolutionId });
                    table.ForeignKey(
                        name: "FK_UserSolutionAccesses_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSolutionAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Components",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Components", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Components_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComponentId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceFiles_Components_ComponentId",
                        column: x => x.ComponentId,
                        principalTable: "Components",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceFiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslatableResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    SourceValue = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ResourceFileId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatableResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslatableResources_ResourceFiles_ResourceFileId",
                        column: x => x.ResourceFileId,
                        principalTable: "ResourceFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceTranslations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TranslatableResourceId = table.Column<Guid>(type: "uuid", nullable: false),
                    TranslationNeedId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValidatedValue = table.Column<string>(type: "text", nullable: true),
                    SuggestedValue = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedById = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceTranslations_TranslatableResources_TranslatableReso~",
                        column: x => x.TranslatableResourceId,
                        principalTable: "TranslatableResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceTranslations_TranslationNeeds_TranslationNeedId",
                        column: x => x.TranslationNeedId,
                        principalTable: "TranslationNeeds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceTranslations_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_SolutionId",
                table: "ApiKeys",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Components_ProjectId",
                table: "Components",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalIdentifiers_ModifiedById",
                table: "ExternalIdentifiers",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ExternalIdentifierId",
                table: "Projects",
                column: "ExternalIdentifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_SolutionId",
                table: "Projects",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceFiles_ComponentId",
                table: "ResourceFiles",
                column: "ComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceFiles_ProjectId",
                table: "ResourceFiles",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTranslations_ModifiedById",
                table: "ResourceTranslations",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTranslations_TranslatableResourceId",
                table: "ResourceTranslations",
                column: "TranslatableResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceTranslations_TranslationNeedId",
                table: "ResourceTranslations",
                column: "TranslationNeedId");

            migrationBuilder.CreateIndex(
                name: "IX_Solutions_OwnerId",
                table: "Solutions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslatableResources_ResourceFileId",
                table: "TranslatableResources",
                column: "ResourceFileId");

            migrationBuilder.CreateIndex(
                name: "IX_TranslationNeeds_SolutionId",
                table: "TranslationNeeds",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSolutionAccesses_SolutionId",
                table: "UserSolutionAccesses",
                column: "SolutionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "ResourceTranslations");

            migrationBuilder.DropTable(
                name: "UserSolutionAccesses");

            migrationBuilder.DropTable(
                name: "TranslatableResources");

            migrationBuilder.DropTable(
                name: "TranslationNeeds");

            migrationBuilder.DropTable(
                name: "ResourceFiles");

            migrationBuilder.DropTable(
                name: "Components");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "ExternalIdentifiers");

            migrationBuilder.DropTable(
                name: "Solutions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
