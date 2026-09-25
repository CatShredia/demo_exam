using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DemoExam1.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskEquipmentAndWorkHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "WorkHours",
                table: "Tasks",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskEquipment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    EquipmentId = table.Column<int>(type: "integer", nullable: false),
                    Problem = table.Column<string>(type: "text", nullable: false),
                    TypeOfProblem = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskEquipment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskEquipment_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskEquipment_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskEquipment_EquipmentId",
                table: "TaskEquipment",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskEquipment_TaskId",
                table: "TaskEquipment",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskEquipment");

            migrationBuilder.DropColumn(
                name: "WorkHours",
                table: "Tasks");
        }
    }
}
