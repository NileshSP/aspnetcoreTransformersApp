using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;

namespace aspnetcoreTransformersApp.Migrations
{
    public partial class InitialDBschema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transformers",
                columns: table => new
                {
                    TransformerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Strength = table.Column<int>(nullable: false),
                    Intelligence = table.Column<int>(nullable: false),
                    Speed = table.Column<int>(nullable: false),
                    Endurance = table.Column<int>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    Courage = table.Column<int>(nullable: false),
                    Firepower = table.Column<int>(nullable: false),
                    Skill = table.Column<int>(nullable: false),
                    AllegianceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transformers", x => x.TransformerId);
                });

            migrationBuilder.CreateTable(
                name: "TransformerAllegiances",
                columns: table => new
                {
                    TransformerAllegianceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllegianceName = table.Column<string>(nullable: true),
                    TransformerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransformerAllegiances", x => x.TransformerAllegianceId);
                    table.ForeignKey(
                        name: "FK_TransformerAllegiances_Transformers_TransformerId",
                        column: x => x.TransformerId,
                        principalTable: "Transformers",
                        principalColumn: "TransformerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransformerAllegiances_TransformerId",
                table: "TransformerAllegiances",
                column: "TransformerId");

            // SP to get transformer score
            var spGetTransformerScore = @"CREATE PROCEDURE dbo.sp_GetTransformerScore 
                                            @TransformerId int = 0
                                        AS
                                        BEGIN
	                                        DECLARE @Score INT = 0

                                            SELECT @Score = (a.Courage + a.Endurance + a.Firepower + a.Intelligence + a.Rank + a.Skill + a.Speed + a.Strength)
	                                        FROM dbo.Transformers a
	                                        WHERE a.TransformerId = @TransformerId 

	                                        SELECT @Score AS Score
                                        END";

            migrationBuilder.Sql(spGetTransformerScore);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransformerAllegiances");

            migrationBuilder.DropTable(
                name: "Transformers");
        }
    }
}
