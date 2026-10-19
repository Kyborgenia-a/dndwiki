using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace saitynoi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Žaidėjų klasių aprašymai, subklasės ir patarimai, kaip jas žaisti.", "Klasės" },
                    { 2, "Žaidžiamų rasių savybės, istorija ir vaidmens patarimai.", "Rasės" },
                    { 3, "Vartotojų sukurti burtai, magiški daiktai, monstrai ir nuotykiai.", "Homebrew kūryba" },
                    { 4, "Patarimai žaidimų vedėjams: sesijų planavimas, improvizacija ir balansas.", "DM patarimai" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "CategoryId", "Content", "CreatedAt", "Title" },
                values: new object[,]
                {
                    { 1, 1, "Paladinas – šventas karys, derinantis kovą artimu atstumu su dieviška magija. Svarbiausios savybės yra Strength ir Charisma. Divine Smite leidžia paversti burtų vietas papildoma radiant žala.", new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Paladino klasės apžvalga" },
                    { 2, 1, "Pirmuose lygiuose vedlys turi mažai gyvybių, todėl laikykitės atokiau nuo priešų. Rinkitės Shield ir Mage Armor, o Find Familiar padės žvalgyti nerizikuojant savimi.", new DateTime(2026, 9, 3, 18, 30, 0, 0, DateTimeKind.Utc), "Kaip išgyventi su Wizard pirmuose lygiuose" },
                    { 3, 2, "Tieflingai turi pragariškos kilmės kraujo. Jie atsparūs ugnies žalai ir paveldi įgimtus burtus, pavyzdžiui, Hellish Rebuke. Puikiai tinka Warlock ar Sorcerer klasėms.", new DateTime(2026, 9, 5, 14, 15, 0, 0, DateTimeKind.Utc), "Tiefling rasė: istorija ir savybės" },
                    { 4, 3, "Retas magiškas daiktas. Kartą per ilgą poilsį nešiotojas gali tapti nematomas iki 1 minutės, jei stovi silpnoje šviesoje arba tamsoje. Puikiai tinka Rogue personažams.", new DateTime(2026, 9, 10, 20, 0, 0, 0, DateTimeKind.Utc), "Homebrew daiktas: Šešėlių apsiaustas" },
                    { 5, 4, "Jei grupė lengvai įveikia kovas, didinkite ne priešų gyvybes, o jų skaičių ir taktiką. Naudokite aplinką, laiko spaudimą ir tikslus, kurie nėra vien priešų nugalėjimas.", new DateTime(2026, 9, 12, 9, 45, 0, 0, DateTimeKind.Utc), "Kaip suvaldyti per stiprią žaidėjų grupę" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "PostId" },
                values: new object[,]
                {
                    { 1, "Oath of Vengeance man atrodo geriausias pasirinkimas pradedantiesiems.", new DateTime(2026, 9, 1, 12, 5, 0, 0, DateTimeKind.Utc), 1 },
                    { 2, "Verta pridėti, kad Divine Smite su kritiniu smūgiu dvigubina ir smite kauliukus.", new DateTime(2026, 9, 2, 8, 40, 0, 0, DateTimeKind.Utc), 1 },
                    { 3, "Shield burtas mano vedlį išgelbėjo ne vieną kartą.", new DateTime(2026, 9, 4, 19, 10, 0, 0, DateTimeKind.Utc), 2 },
                    { 4, "Ar šiam apsiaustui reikia attunement?", new DateTime(2026, 9, 11, 7, 20, 0, 0, DateTimeKind.Utc), 4 },
                    { 5, "Taip, reikia. Pridėsiu tai į aprašymą.", new DateTime(2026, 9, 11, 9, 0, 0, 0, DateTimeKind.Utc), 4 },
                    { 6, "Mes pridedame priešų bangomis, ir kovos vėl tampa įdomios.", new DateTime(2026, 9, 13, 21, 30, 0, 0, DateTimeKind.Utc), 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CategoryId",
                table: "Posts",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
