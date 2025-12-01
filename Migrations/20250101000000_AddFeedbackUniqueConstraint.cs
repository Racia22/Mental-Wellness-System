using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MentalWellnessSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add unique constraint to prevent duplicate feedback per appointment
            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_AppointmentId_Unique",
                table: "Feedbacks",
                column: "AppointmentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_AppointmentId_Unique",
                table: "Feedbacks");
        }
    }
}
