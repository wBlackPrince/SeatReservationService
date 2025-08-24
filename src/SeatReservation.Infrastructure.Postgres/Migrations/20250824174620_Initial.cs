using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeatReservation.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resevations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    details = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "venues",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    max_seats_count = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    prefix = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_venues", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    venue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    event_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    info = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_events_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "seats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    venue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    row_number = table.Column<int>(type: "integer", nullable: false),
                    seat_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seats", x => x.id);
                    table.ForeignKey(
                        name: "FK_seats_venues_venue_id",
                        column: x => x.venue_id,
                        principalTable: "venues",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_details",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    LastReservationUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_details", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_details_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reservation_seats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    reservation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservationAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reservation_seats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reservation_seats_reservations_reservation_id",
                        column: x => x.reservation_id,
                        principalTable: "reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reservation_seats_seats_seat_id",
                        column: x => x.seat_id,
                        principalTable: "seats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_details_event_id",
                table: "event_details",
                column: "event_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_venue_id",
                table: "events",
                column: "venue_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_seats_event_id_seat_id",
                table: "reservation_seats",
                columns: new[] { "event_id", "seat_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservation_seats_reservation_id",
                table: "reservation_seats",
                column: "reservation_id");

            migrationBuilder.CreateIndex(
                name: "IX_reservation_seats_seat_id",
                table: "reservation_seats",
                column: "seat_id");

            migrationBuilder.CreateIndex(
                name: "IX_seats_venue_id",
                table: "seats",
                column: "venue_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_details");

            migrationBuilder.DropTable(
                name: "reservation_seats");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "seats");

            migrationBuilder.DropTable(
                name: "venues");
        }
    }
}
