using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace WebApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "quartz");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AufyRefreshTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AufyRefreshTokens", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Keywords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Keywords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "qrtz_calendars",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    calendar_name = table.Column<string>(type: "text", nullable: false),
                    calendar = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_calendars", x => new { x.sched_name, x.calendar_name });
                });

            migrationBuilder.CreateTable(
                name: "qrtz_fired_triggers",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    entry_id = table.Column<string>(type: "text", nullable: false),
                    trigger_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false),
                    instance_name = table.Column<string>(type: "text", nullable: false),
                    fired_time = table.Column<long>(type: "bigint", nullable: false),
                    sched_time = table.Column<long>(type: "bigint", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<string>(type: "text", nullable: false),
                    job_name = table.Column<string>(type: "text", nullable: true),
                    job_group = table.Column<string>(type: "text", nullable: true),
                    is_nonconcurrent = table.Column<bool>(type: "bool", nullable: false),
                    requests_recovery = table.Column<bool>(type: "bool", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_fired_triggers", x => new { x.sched_name, x.entry_id });
                });

            migrationBuilder.CreateTable(
                name: "qrtz_job_details",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    job_name = table.Column<string>(type: "text", nullable: false),
                    job_group = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    job_class_name = table.Column<string>(type: "text", nullable: false),
                    is_durable = table.Column<bool>(type: "bool", nullable: false),
                    is_nonconcurrent = table.Column<bool>(type: "bool", nullable: false),
                    is_update_data = table.Column<bool>(type: "bool", nullable: false),
                    requests_recovery = table.Column<bool>(type: "bool", nullable: false),
                    job_data = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_job_details", x => new { x.sched_name, x.job_name, x.job_group });
                });

            migrationBuilder.CreateTable(
                name: "qrtz_locks",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    lock_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_locks", x => new { x.sched_name, x.lock_name });
                });

            migrationBuilder.CreateTable(
                name: "qrtz_paused_trigger_grps",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_paused_trigger_grps", x => new { x.sched_name, x.trigger_group });
                });

            migrationBuilder.CreateTable(
                name: "qrtz_scheduler_state",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    instance_name = table.Column<string>(type: "text", nullable: false),
                    last_checkin_time = table.Column<long>(type: "bigint", nullable: false),
                    checkin_interval = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_scheduler_state", x => new { x.sched_name, x.instance_name });
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qrtz_triggers",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    trigger_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false),
                    job_name = table.Column<string>(type: "text", nullable: false),
                    job_group = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    next_fire_time = table.Column<long>(type: "bigint", nullable: true),
                    prev_fire_time = table.Column<long>(type: "bigint", nullable: true),
                    priority = table.Column<int>(type: "integer", nullable: true),
                    trigger_state = table.Column<string>(type: "text", nullable: false),
                    trigger_type = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<long>(type: "bigint", nullable: false),
                    end_time = table.Column<long>(type: "bigint", nullable: true),
                    calendar_name = table.Column<string>(type: "text", nullable: true),
                    misfire_instr = table.Column<short>(type: "smallint", nullable: true),
                    job_data = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                    table.ForeignKey(
                        name: "FK_qrtz_triggers_qrtz_job_details_sched_name_job_name_job_group",
                        columns: x => new { x.sched_name, x.job_name, x.job_group },
                        principalSchema: "quartz",
                        principalTable: "qrtz_job_details",
                        principalColumns: new[] { "sched_name", "job_name", "job_group" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    UserDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Folders_UserData_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "UserData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "qrtz_blob_triggers",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    trigger_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false),
                    blob_data = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_blob_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                    table.ForeignKey(
                        name: "FK_qrtz_blob_triggers_qrtz_triggers_sched_name_trigger_name_tr~",
                        columns: x => new { x.sched_name, x.trigger_name, x.trigger_group },
                        principalSchema: "quartz",
                        principalTable: "qrtz_triggers",
                        principalColumns: new[] { "sched_name", "trigger_name", "trigger_group" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qrtz_cron_triggers",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    trigger_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false),
                    cron_expression = table.Column<string>(type: "text", nullable: false),
                    time_zone_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_cron_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                    table.ForeignKey(
                        name: "FK_qrtz_cron_triggers_qrtz_triggers_sched_name_trigger_name_tr~",
                        columns: x => new { x.sched_name, x.trigger_name, x.trigger_group },
                        principalSchema: "quartz",
                        principalTable: "qrtz_triggers",
                        principalColumns: new[] { "sched_name", "trigger_name", "trigger_group" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qrtz_simple_triggers",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    trigger_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false),
                    repeat_count = table.Column<long>(type: "bigint", nullable: false),
                    repeat_interval = table.Column<long>(type: "bigint", nullable: false),
                    times_triggered = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_simple_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                    table.ForeignKey(
                        name: "FK_qrtz_simple_triggers_qrtz_triggers_sched_name_trigger_name_~",
                        columns: x => new { x.sched_name, x.trigger_name, x.trigger_group },
                        principalSchema: "quartz",
                        principalTable: "qrtz_triggers",
                        principalColumns: new[] { "sched_name", "trigger_name", "trigger_group" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "qrtz_simprop_triggers",
                schema: "quartz",
                columns: table => new
                {
                    sched_name = table.Column<string>(type: "text", nullable: false),
                    trigger_name = table.Column<string>(type: "text", nullable: false),
                    trigger_group = table.Column<string>(type: "text", nullable: false),
                    str_prop_1 = table.Column<string>(type: "text", nullable: true),
                    str_prop_2 = table.Column<string>(type: "text", nullable: true),
                    str_prop_3 = table.Column<string>(type: "text", nullable: true),
                    int_prop_1 = table.Column<int>(type: "integer", nullable: true),
                    int_prop_2 = table.Column<int>(type: "integer", nullable: true),
                    long_prop_1 = table.Column<long>(type: "bigint", nullable: true),
                    long_prop_2 = table.Column<long>(type: "bigint", nullable: true),
                    dec_prop_1 = table.Column<decimal>(type: "numeric", nullable: true),
                    dec_prop_2 = table.Column<decimal>(type: "numeric", nullable: true),
                    bool_prop_1 = table.Column<bool>(type: "bool", nullable: true),
                    bool_prop_2 = table.Column<bool>(type: "bool", nullable: true),
                    time_zone_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_qrtz_simprop_triggers", x => new { x.sched_name, x.trigger_name, x.trigger_group });
                    table.ForeignKey(
                        name: "FK_qrtz_simprop_triggers_qrtz_triggers_sched_name_trigger_name~",
                        columns: x => new { x.sched_name, x.trigger_name, x.trigger_group },
                        principalSchema: "quartz",
                        principalTable: "qrtz_triggers",
                        principalColumns: new[] { "sched_name", "trigger_name", "trigger_group" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderKeyword",
                columns: table => new
                {
                    FoldersId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeywordsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderKeyword", x => new { x.FoldersId, x.KeywordsId });
                    table.ForeignKey(
                        name: "FK_FolderKeyword_Folders_FoldersId",
                        column: x => x.FoldersId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderKeyword_Keywords_KeywordsId",
                        column: x => x.KeywordsId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    FolderId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserDataId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notes_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Notes_UserData_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "UserData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "KeywordNotes",
                columns: table => new
                {
                    NoteId = table.Column<Guid>(type: "uuid", nullable: false),
                    KeywordId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeywordNotes", x => new { x.KeywordId, x.NoteId });
                    table.ForeignKey(
                        name: "FK_KeywordNotes_Keywords_KeywordId",
                        column: x => x.KeywordId,
                        principalTable: "Keywords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KeywordNotes_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FolderKeyword_KeywordsId",
                table: "FolderKeyword",
                column: "KeywordsId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UserDataId",
                table: "Folders",
                column: "UserDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UserId",
                table: "Folders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_KeywordNotes_NoteId",
                table: "KeywordNotes",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Keywords_Name",
                table: "Keywords",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notes_FolderId",
                table: "Notes",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserDataId",
                table: "Notes",
                column: "UserDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_UserId",
                table: "Notes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_job_group",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                column: "job_group");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_job_name",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                column: "job_name");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_job_req_recovery",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                column: "requests_recovery");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_trig_group",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                column: "trigger_group");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_trig_inst_name",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                column: "instance_name");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_trig_name",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                column: "trigger_name");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_ft_trig_nm_gp",
                schema: "quartz",
                table: "qrtz_fired_triggers",
                columns: new[] { "sched_name", "trigger_name", "trigger_group" });

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_j_req_recovery",
                schema: "quartz",
                table: "qrtz_job_details",
                column: "requests_recovery");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_t_next_fire_time",
                schema: "quartz",
                table: "qrtz_triggers",
                column: "next_fire_time");

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_t_nft_st",
                schema: "quartz",
                table: "qrtz_triggers",
                columns: new[] { "next_fire_time", "trigger_state" });

            migrationBuilder.CreateIndex(
                name: "idx_qrtz_t_state",
                schema: "quartz",
                table: "qrtz_triggers",
                column: "trigger_state");

            migrationBuilder.CreateIndex(
                name: "IX_qrtz_triggers_sched_name_job_name_job_group",
                schema: "quartz",
                table: "qrtz_triggers",
                columns: new[] { "sched_name", "job_name", "job_group" });

            migrationBuilder.CreateIndex(
                name: "IX_UserData_UserId",
                table: "UserData",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AufyRefreshTokens");

            migrationBuilder.DropTable(
                name: "FolderKeyword");

            migrationBuilder.DropTable(
                name: "KeywordNotes");

            migrationBuilder.DropTable(
                name: "qrtz_blob_triggers",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_calendars",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_cron_triggers",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_fired_triggers",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_locks",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_paused_trigger_grps",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_scheduler_state",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_simple_triggers",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "qrtz_simprop_triggers",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Keywords");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "qrtz_triggers",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "Folders");

            migrationBuilder.DropTable(
                name: "qrtz_job_details",
                schema: "quartz");

            migrationBuilder.DropTable(
                name: "UserData");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
