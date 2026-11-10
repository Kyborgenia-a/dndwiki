using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace saitynoi.Migrations
{
    /// <inheritdoc />
    public partial class CategoryCreatedAtAndMoreSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 9, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 20, 9, 5, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 21, 14, 30, 0, 0, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 22, 18, 0, 0, 0, DateTimeKind.Utc));

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[] { 5, new DateTime(2026, 8, 25, 11, 15, 0, 0, DateTimeKind.Utc), "Burtų aprašymai, jų derinimas ir taisyklių paaiškinimai.", "Burtai" });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "PostId" },
                values: new object[,]
                {
                    { 7, "Find Familiar pelėda dar gali duoti Help veiksmą ir nusiskristi neišprovokuodama atakos.", new DateTime(2026, 9, 5, 10, 15, 0, 0, DateTimeKind.Utc), 2 },
                    { 8, "Mano tiefling warlock su Hellish Rebuke jau ne kartą nustebino DM.", new DateTime(2026, 9, 6, 17, 45, 0, 0, DateTimeKind.Utc), 3 }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "CategoryId", "Content", "CreatedAt", "Title" },
                values: new object[,]
                {
                    { 6, 2, "Nykštukai mato tamsoje (Darkvision), yra atsparūs nuodams ir puikiai išmano akmens darbus. Dėl tvirtumo jie natūraliai tinka Fighter, Cleric ir Paladin klasėms.", new DateTime(2026, 9, 15, 16, 20, 0, 0, DateTimeKind.Utc), "Nykštukai (Dwarf): atsparumas ir tradicijos" },
                    { 7, 3, "Challenge Rating 4 priešininkė, gyvenanti pelkėse. Ji gali sukurti rūką, kuris riboja matomumą, ir prisikviesti dvi pelkių gyvates. Tinka 3–4 lygio grupei kaip sesijos finalas.", new DateTime(2026, 9, 18, 21, 10, 0, 0, DateTimeKind.Utc), "Homebrew monstras: Pelkių ragana" },
                    { 8, 4, "Užtenka trijų dalykų: aiškaus nuotykio kabliuko, vienos kovos ir vieno socialinio susitikimo. Nerašykite viso scenarijaus – leiskite žaidėjų sprendimams nulemti eigą.", new DateTime(2026, 9, 22, 8, 0, 0, 0, DateTimeKind.Utc), "Kaip pasiruošti pirmajai sesijai per valandą" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "PostId" },
                values: new object[,]
                {
                    { 9, "Mountain dwarf gauna vidutinių šarvų įgūdį, todėl tinka net wizard klasei.", new DateTime(2026, 9, 16, 11, 0, 0, 0, DateTimeKind.Utc), 6 },
                    { 10, "Išbandėme vakar: rūkas tikrai pakeitė kovos taktiką, labai patiko.", new DateTime(2026, 9, 20, 22, 5, 0, 0, DateTimeKind.Utc), 7 },
                    { 11, "Siūlyčiau gyvatėms sumažinti gyvybes, kitaip kova užsitęsia.", new DateTime(2026, 9, 21, 8, 30, 0, 0, DateTimeKind.Utc), 7 },
                    { 12, "Socialinis susitikimas taverne – klasika, kuri visada veikia.", new DateTime(2026, 9, 23, 13, 50, 0, 0, DateTimeKind.Utc), 8 }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "CategoryId", "Content", "CreatedAt", "Title" },
                values: new object[,]
                {
                    { 9, 5, "Fireball yra 3 lygio burtas, darantis 8d6 ugnies žalos 20 pėdų spinduliu. Jis puikus prieš priešų grupes, bet uždarose patalpose lengva užkabinti ir savo sąjungininkus.", new DateTime(2026, 9, 25, 19, 40, 0, 0, DateTimeKind.Utc), "Fireball: kada verta, o kada ne" },
                    { 10, 5, "Vienu metu galima palaikyti tik vieną Concentration burtą. Gavęs žalos, burtininkas meta Constitution gelbėjimo metimą: sudėtingumas yra 10 arba pusė gautos žalos, jei ji didesnė.", new DateTime(2026, 9, 28, 12, 5, 0, 0, DateTimeKind.Utc), "Concentration taisyklė paprastai" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "PostId" },
                values: new object[,]
                {
                    { 13, "Prieš kiekvieną Fireball klausiu grupės, ar visi stovi už spindulio ribų.", new DateTime(2026, 9, 26, 9, 25, 0, 0, DateTimeKind.Utc), 9 },
                    { 14, "Verta paminėti, kad aukštesnio lygio vieta prideda po 1d6 žalos.", new DateTime(2026, 9, 27, 15, 0, 0, 0, DateTimeKind.Utc), 9 },
                    { 15, "War Caster feat labai padeda išlaikyti koncentraciją kovoje.", new DateTime(2026, 9, 29, 18, 35, 0, 0, DateTimeKind.Utc), 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Comments",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Categories");
        }
    }
}
