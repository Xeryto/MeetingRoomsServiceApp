using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccessLayer.Migrations
{
    public partial class ChangedIdNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_MeetingRooms_MeetingRoomID",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserID",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Reservations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "MeetingRoomID",
                table: "Reservations",
                newName: "MeetingRoomId");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "Reservations",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserID",
                table: "Reservations",
                newName: "IX_Reservations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_MeetingRoomID",
                table: "Reservations",
                newName: "IX_Reservations_MeetingRoomId");

            migrationBuilder.RenameColumn(
                name: "MeetingRoomID",
                table: "MeetingRooms",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_MeetingRooms_MeetingRoomId",
                table: "Reservations",
                column: "MeetingRoomId",
                principalTable: "MeetingRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_MeetingRooms_MeetingRoomId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Users_UserId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Reservations",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "MeetingRoomId",
                table: "Reservations",
                newName: "MeetingRoomID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Reservations",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_UserId",
                table: "Reservations",
                newName: "IX_Reservations_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Reservations_MeetingRoomId",
                table: "Reservations",
                newName: "IX_Reservations_MeetingRoomID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MeetingRooms",
                newName: "MeetingRoomID");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_MeetingRooms_MeetingRoomID",
                table: "Reservations",
                column: "MeetingRoomID",
                principalTable: "MeetingRooms",
                principalColumn: "MeetingRoomID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Users_UserID",
                table: "Reservations",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
