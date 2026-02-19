using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HBKPlatform.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "HbkBaseEntitySequence");

            migrationBuilder.CreateTable(
                name: "application_roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenancies",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    org_name = table.Column<string>(type: "text", nullable: false),
                    org_tagline = table.Column<string>(type: "text", nullable: true),
                    contact_email = table.Column<string>(type: "text", nullable: true),
                    licence_status = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    registration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenancies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "timeslots",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    day = table.Column<int>(type: "integer", nullable: false),
                    time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    duration = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_timeslots", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "application_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "attributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_attributes_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    value2 = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.id);
                    table.ForeignKey(
                        name: "FK_settings_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "treatments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    treatment_requestability = table.Column<int>(type: "integer", nullable: false),
                    cost = table.Column<double>(type: "double precision", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    img = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_treatments", x => x.id);
                    table.ForeignKey(
                        name: "FK_treatments_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    login_count = table.Column<int>(type: "integer", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_users_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "clinics",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    street_address = table.Column<string>(type: "text", nullable: true),
                    telephone = table.Column<string>(type: "text", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    manager_user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinics", x => x.id);
                    table.ForeignKey(
                        name: "FK_clinics_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_clinics_asp_net_users_manager_user_id",
                        column: x => x.manager_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "application_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rooms",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    img = table.Column<string>(type: "text", nullable: true),
                    clinic_id = table.Column<int>(type: "integer", nullable: false),
                    price_per_use = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rooms", x => x.id);
                    table.ForeignKey(
                        name: "FK_rooms_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rooms_clinics_clinic_id",
                        column: x => x.clinic_id,
                        principalTable: "clinics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_attributes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    room_id = table.Column<int>(type: "integer", nullable: false),
                    attribute_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_attributes", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_attributes_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_attributes_attributes_attribute_id",
                        column: x => x.attribute_id,
                        principalTable: "attributes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_attributes_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    practitioner_id = table.Column<int>(type: "integer", nullable: false),
                    treatment_id = table.Column<int>(type: "integer", nullable: false),
                    timeslot_id = table.Column<int>(type: "integer", nullable: false),
                    room_id = table.Column<int>(type: "integer", nullable: true),
                    room_reservation_id = table.Column<int>(type: "integer", nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    week_num = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    cancellation_reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                    table.ForeignKey(
                        name: "FK_appointments_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_appointments_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_appointments_timeslots_timeslot_id",
                        column: x => x.timeslot_id,
                        principalTable: "timeslots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_appointments_treatments_treatment_id",
                        column: x => x.treatment_id,
                        principalTable: "treatments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    previous_message_id = table.Column<int>(type: "integer", nullable: true),
                    practitioner_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    message_body = table.Column<string>(type: "text", nullable: false),
                    date_opened = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    message_status_practitioner = table.Column<int>(type: "integer", nullable: false),
                    message_status_client = table.Column<int>(type: "integer", nullable: false),
                    message_origin = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_messages", x => x.id);
                    table.ForeignKey(
                        name: "FK_client_messages_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_client_messages_client_messages_previous_message_id",
                        column: x => x.previous_message_id,
                        principalTable: "client_messages",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "client_practitioners",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    practitioner_id = table.Column<int>(type: "integer", nullable: false),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client_practitioners", x => new { x.client_id, x.practitioner_id });
                    table.ForeignKey(
                        name: "fk_client_practitioners_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "client_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    practitioner_id = table.Column<int>(type: "integer", nullable: false),
                    client_id = table.Column<int>(type: "integer", nullable: false),
                    appointment_id = table.Column<int>(type: "integer", nullable: true),
                    record_visibility = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    note_body = table.Column<string>(type: "text", nullable: false),
                    is_priority = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_client_records", x => x.id);
                    table.ForeignKey(
                        name: "FK_client_records_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_client_records_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "clients",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    title = table.Column<int>(type: "integer", nullable: false),
                    sex = table.Column<int>(type: "integer", nullable: false),
                    forename = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    img = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    telephone = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: true),
                    practice_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clients", x => x.id);
                    table.ForeignKey(
                        name: "FK_clients_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_clients_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "practices",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: false),
                    street_address = table.Column<string>(type: "text", nullable: true),
                    telephone = table.Column<string>(type: "text", nullable: false),
                    email_address = table.Column<string>(type: "text", nullable: false),
                    lead_practitioner_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_practices", x => x.id);
                    table.ForeignKey(
                        name: "FK_practices_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "practitioners",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    forename = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<int>(type: "integer", nullable: false),
                    location = table.Column<string>(type: "text", nullable: true),
                    client_bio = table.Column<string>(type: "text", nullable: true),
                    clinic_bio = table.Column<string>(type: "text", nullable: true),
                    gmc_number = table.Column<string>(type: "text", nullable: true),
                    credentials = table.Column<string>(type: "text", nullable: true),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    img = table.Column<string>(type: "text", nullable: true),
                    sex = table.Column<int>(type: "integer", nullable: false),
                    practice_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_practitioners", x => x.id);
                    table.ForeignKey(
                        name: "FK_practitioners_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_practitioners_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_practitioners_practices_practice_id",
                        column: x => x.practice_id,
                        principalTable: "practices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "room_reservations",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    room_id = table.Column<int>(type: "integer", nullable: false),
                    clinic_id = table.Column<int>(type: "integer", nullable: false),
                    practitioner_id = table.Column<int>(type: "integer", nullable: false),
                    timeslot_id = table.Column<int>(type: "integer", nullable: false),
                    practice_note = table.Column<string>(type: "text", nullable: true),
                    clinic_note = table.Column<string>(type: "text", nullable: true),
                    cancellation_reason = table.Column<string>(type: "text", nullable: true),
                    week_num = table.Column<int>(type: "integer", nullable: false),
                    reservation_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_room_reservations", x => x.id);
                    table.ForeignKey(
                        name: "FK_room_reservations_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_reservations_clinics_clinic_id",
                        column: x => x.clinic_id,
                        principalTable: "clinics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_reservations_practitioners_practitioner_id",
                        column: x => x.practitioner_id,
                        principalTable: "practitioners",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_reservations_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_room_reservations_timeslots_timeslot_id",
                        column: x => x.timeslot_id,
                        principalTable: "timeslots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "timeslot_availability",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "nextval('\"HbkBaseEntitySequence\"')"),
                    tenancy_id = table.Column<int>(type: "integer", nullable: false),
                    date_created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    date_modified = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    create_actioner = table.Column<string>(type: "text", nullable: true),
                    modify_actioner = table.Column<string>(type: "text", nullable: true),
                    timeslot_id = table.Column<int>(type: "integer", nullable: false),
                    practitioner_id = table.Column<int>(type: "integer", nullable: true),
                    room_id = table.Column<int>(type: "integer", nullable: true),
                    week_num = table.Column<int>(type: "integer", nullable: false),
                    is_indefinite = table.Column<bool>(type: "boolean", nullable: false),
                    entity = table.Column<int>(type: "integer", nullable: false),
                    availability = table.Column<int>(type: "integer", nullable: false),
                    interlude = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timeslot_availability", x => x.id);
                    table.ForeignKey(
                        name: "FK_timeslot_availability_tenancies_tenancy_id",
                        column: x => x.tenancy_id,
                        principalTable: "tenancies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_timeslot_availability_practitioners_practitioner_id",
                        column: x => x.practitioner_id,
                        principalTable: "practitioners",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_timeslot_availability_rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "rooms",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_timeslot_availability_timeslots_timeslot_id",
                        column: x => x.timeslot_id,
                        principalTable: "timeslots",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "role_name_index",
                table: "application_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_appointments_client_id",
                table: "appointments",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_practitioner_id",
                table: "appointments",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_room_id",
                table: "appointments",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_room_reservation_id",
                table: "appointments",
                column: "room_reservation_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_tenancy_id",
                table: "appointments",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_timeslot_id",
                table: "appointments",
                column: "timeslot_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_treatment_id",
                table: "appointments",
                column: "treatment_id");

            migrationBuilder.CreateIndex(
                name: "IX_attributes_tenancy_id",
                table: "attributes",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_messages_client_id",
                table: "client_messages",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_messages_practitioner_id",
                table: "client_messages",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_messages_previous_message_id",
                table: "client_messages",
                column: "previous_message_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_messages_tenancy_id",
                table: "client_messages",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_practitioners_practitioner_id",
                table: "client_practitioners",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_practitioners_tenancy_id",
                table: "client_practitioners",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_records_appointment_id",
                table: "client_records",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_records_client_id",
                table: "client_records",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_client_records_practitioner_id",
                table: "client_records",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "IX_client_records_tenancy_id",
                table: "client_records",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_clients_practice_id",
                table: "clients",
                column: "practice_id");

            migrationBuilder.CreateIndex(
                name: "IX_clients_tenancy_id",
                table: "clients",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_clients_user_id",
                table: "clients",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_clinics_manager_user_id",
                table: "clinics",
                column: "manager_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_clinics_tenancy_id",
                table: "clinics",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_practices_lead_practitioner_id",
                table: "practices",
                column: "lead_practitioner_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_practices_tenancy_id",
                table: "practices",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_practitioners_practice_id",
                table: "practitioners",
                column: "practice_id");

            migrationBuilder.CreateIndex(
                name: "IX_practitioners_tenancy_id",
                table: "practitioners",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_practitioners_user_id",
                table: "practitioners",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_attributes_attribute_id",
                table: "room_attributes",
                column: "attribute_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_attributes_room_id",
                table: "room_attributes",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_attributes_tenancy_id",
                table: "room_attributes",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_reservations_clinic_id",
                table: "room_reservations",
                column: "clinic_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_reservations_practitioner_id",
                table: "room_reservations",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_reservations_room_id",
                table: "room_reservations",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_room_reservations_tenancy_id",
                table: "room_reservations",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_room_reservations_timeslot_id",
                table: "room_reservations",
                column: "timeslot_id");

            migrationBuilder.CreateIndex(
                name: "ix_rooms_clinic_id",
                table: "rooms",
                column: "clinic_id");

            migrationBuilder.CreateIndex(
                name: "IX_rooms_tenancy_id",
                table: "rooms",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "IX_settings_tenancy_id",
                table: "settings",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_timeslot_availability_practitioner_id",
                table: "timeslot_availability",
                column: "practitioner_id");

            migrationBuilder.CreateIndex(
                name: "ix_timeslot_availability_room_id",
                table: "timeslot_availability",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_timeslot_availability_tenancy_id",
                table: "timeslot_availability",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_timeslot_availability_timeslot_id",
                table: "timeslot_availability",
                column: "timeslot_id");

            migrationBuilder.CreateIndex(
                name: "IX_treatments_tenancy_id",
                table: "treatments",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "email_index",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_tenancy_id",
                table: "users",
                column: "tenancy_id");

            migrationBuilder.CreateIndex(
                name: "user_name_index",
                table: "users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_appointments_clients_client_id",
                table: "appointments",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_appointments_practitioners_practitioner_id",
                table: "appointments",
                column: "practitioner_id",
                principalTable: "practitioners",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_appointments_room_reservations_room_reservation_id",
                table: "appointments",
                column: "room_reservation_id",
                principalTable: "room_reservations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_client_messages_clients_client_id",
                table: "client_messages",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_messages_practitioners_practitioner_id",
                table: "client_messages",
                column: "practitioner_id",
                principalTable: "practitioners",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_practitioners_hbk_base_entity_client_id",
                table: "client_practitioners",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_practitioners_hbk_base_entity_practitioner_id",
                table: "client_practitioners",
                column: "practitioner_id",
                principalTable: "practitioners",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_records_clients_client_id",
                table: "client_records",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_client_records_practitioners_practitioner_id",
                table: "client_records",
                column: "practitioner_id",
                principalTable: "practitioners",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_clients_practices_practice_id",
                table: "clients",
                column: "practice_id",
                principalTable: "practices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_practices_practitioners_lead_practitioner_id",
                table: "practices",
                column: "lead_practitioner_id",
                principalTable: "practitioners",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_practices_tenancies_tenancy_id",
                table: "practices");

            migrationBuilder.DropForeignKey(
                name: "FK_practitioners_tenancies_tenancy_id",
                table: "practitioners");

            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_tenancies_tenancy_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_practices_practitioners_lead_practitioner_id",
                table: "practices");

            migrationBuilder.DropTable(
                name: "client_messages");

            migrationBuilder.DropTable(
                name: "client_practitioners");

            migrationBuilder.DropTable(
                name: "client_records");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "room_attributes");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "timeslot_availability");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "attributes");

            migrationBuilder.DropTable(
                name: "application_roles");

            migrationBuilder.DropTable(
                name: "clients");

            migrationBuilder.DropTable(
                name: "room_reservations");

            migrationBuilder.DropTable(
                name: "treatments");

            migrationBuilder.DropTable(
                name: "rooms");

            migrationBuilder.DropTable(
                name: "timeslots");

            migrationBuilder.DropTable(
                name: "clinics");

            migrationBuilder.DropTable(
                name: "tenancies");

            migrationBuilder.DropTable(
                name: "practitioners");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "practices");

            migrationBuilder.DropSequence(
                name: "HbkBaseEntitySequence");
        }
    }
}
