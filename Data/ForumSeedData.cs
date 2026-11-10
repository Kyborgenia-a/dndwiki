using DndForum.Auth;
using DndForum.Models;
using Microsoft.EntityFrameworkCore;

namespace DndForum.Data;

// Seed values must be fixed: dynamic values such as DateTime.UtcNow would make
// EF Core generate a new migration every time the model is compared.
public static class ForumSeedData
{
    private const int ClassesCategoryId = 1;
    private const int RacesCategoryId = 2;
    private const int HomebrewCategoryId = 3;
    private const int DungeonMasterTipsCategoryId = 4;
    private const int SpellsCategoryId = 5;

    private const int AdminUserId = 1;
    private const int ModeratorUserId = 2;
    private const int DragonHunterUserId = 3;
    private const int WizardIevaUserId = 4;
    private const int DwarfTomasUserId = 5;

    private const int PaladinPostId = 1;
    private const int WizardPostId = 2;
    private const int TieflingPostId = 3;
    private const int ShadowCloakPostId = 4;
    private const int StrongPartyPostId = 5;
    private const int DwarfPostId = 6;
    private const int SwampWitchPostId = 7;
    private const int FirstSessionPostId = 8;
    private const int FireballPostId = 9;
    private const int ConcentrationPostId = 10;

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(Users());
        modelBuilder.Entity<Category>().HasData(Categories());
        modelBuilder.Entity<Post>().HasData(Posts());
        modelBuilder.Entity<Comment>().HasData(Comments());
    }

    private static DateTime Utc(int month, int day, int hour, int minute) =>
        new(2026, month, day, hour, minute, 0, DateTimeKind.Utc);

    // Password hashes are precomputed in ASP.NET Identity V3 format (PBKDF2-HMAC-SHA512, 100 000 iterations)
    // with fixed salts, because a hash computed at startup would change the model on every migration.
    // Demo passwords: admin / Admin123!, moderatorius / Moder123!, other users / Narys123!
    private static User[] Users() =>
    [
        new() { Id = AdminUserId, UserName = "admin", Email = "admin@dndforum.test", Role = Roles.Admin, CreatedAt = Utc(8, 15, 9, 0), PasswordHash = "AQAAAAIAAYagAAAAEA+4iFavXvb672kYZBCwgBzfIUfDPQDfu9w6ckdTVwl8uwqJ+rJBAJlPWkl+AUvX8g==" },
        new() { Id = ModeratorUserId, UserName = "moderatorius", Email = "moderatorius@dndforum.test", Role = Roles.Moderator, CreatedAt = Utc(8, 16, 10, 30), PasswordHash = "AQAAAAIAAYagAAAAEGdO0WcmKpKS3YWHj64eKnnB1eaMnnmDVhD7PLwWayzRZo3P+2Guithu33/QSGMAmQ==" },
        new() { Id = DragonHunterUserId, UserName = "drakonu_medziotojas", Email = "drakonai@dndforum.test", Role = Roles.Member, CreatedAt = Utc(8, 18, 19, 45), PasswordHash = "AQAAAAIAAYagAAAAENEOy6xjtBp7C1rcvp0pyq330fFX6YcGjeSZjtGNbOlbGon9hyFfZluy+5kDVKz15w==" },
        new() { Id = WizardIevaUserId, UserName = "burtininke_ieva", Email = "ieva@dndforum.test", Role = Roles.Member, CreatedAt = Utc(8, 19, 12, 10), PasswordHash = "AQAAAAIAAYagAAAAEFLG4qlepnkFjYNlHPRozvdSNdTwe6hD78P6pHCS4XgQnD7r4Gg7Zdjss4XXWUJKCw==" },
        new() { Id = DwarfTomasUserId, UserName = "nykstukas_tomas", Email = "tomas@dndforum.test", Role = Roles.Member, CreatedAt = Utc(8, 20, 8, 25), PasswordHash = "AQAAAAIAAYagAAAAEAF2GSVnPfVbIhFnHFEKUNedG+ePu+2FpTzdWE7TzEDicZRU+WlprDREJS6Xs83Pog==" }
    ];

    private static Category[] Categories() =>
    [
        new() { Id = ClassesCategoryId, Name = "Klasės", Description = "Žaidėjų klasių aprašymai, subklasės ir patarimai, kaip jas žaisti.", CreatedAt = Utc(8, 20, 9, 0) },
        new() { Id = RacesCategoryId, Name = "Rasės", Description = "Žaidžiamų rasių savybės, istorija ir vaidmens patarimai.", CreatedAt = Utc(8, 20, 9, 5) },
        new() { Id = HomebrewCategoryId, Name = "Homebrew kūryba", Description = "Vartotojų sukurti burtai, magiški daiktai, monstrai ir nuotykiai.", CreatedAt = Utc(8, 21, 14, 30) },
        new() { Id = DungeonMasterTipsCategoryId, Name = "DM patarimai", Description = "Patarimai žaidimų vedėjams: sesijų planavimas, improvizacija ir balansas.", CreatedAt = Utc(8, 22, 18, 0) },
        new() { Id = SpellsCategoryId, Name = "Burtai", Description = "Burtų aprašymai, jų derinimas ir taisyklių paaiškinimai.", CreatedAt = Utc(8, 25, 11, 15) }
    ];

    private static Post[] Posts() =>
    [
        new()
        {
            Id = PaladinPostId,
            AuthorId = DragonHunterUserId,
            CategoryId = ClassesCategoryId,
            Title = "Paladino klasės apžvalga",
            Content = "Paladinas – šventas karys, derinantis kovą artimu atstumu su dieviška magija. Svarbiausios savybės yra Strength ir Charisma. Divine Smite leidžia paversti burtų vietas papildoma radiant žala.",
            CreatedAt = Utc(9, 1, 10, 0)
        },
        new()
        {
            Id = WizardPostId,
            AuthorId = WizardIevaUserId,
            CategoryId = ClassesCategoryId,
            Title = "Kaip išgyventi su Wizard pirmuose lygiuose",
            Content = "Pirmuose lygiuose vedlys turi mažai gyvybių, todėl laikykitės atokiau nuo priešų. Rinkitės Shield ir Mage Armor, o Find Familiar padės žvalgyti nerizikuojant savimi.",
            CreatedAt = Utc(9, 3, 18, 30)
        },
        new()
        {
            Id = TieflingPostId,
            AuthorId = WizardIevaUserId,
            CategoryId = RacesCategoryId,
            Title = "Tiefling rasė: istorija ir savybės",
            Content = "Tieflingai turi pragariškos kilmės kraujo. Jie atsparūs ugnies žalai ir paveldi įgimtus burtus, pavyzdžiui, Hellish Rebuke. Puikiai tinka Warlock ar Sorcerer klasėms.",
            CreatedAt = Utc(9, 5, 14, 15)
        },
        new()
        {
            Id = ShadowCloakPostId,
            AuthorId = DwarfTomasUserId,
            CategoryId = HomebrewCategoryId,
            Title = "Homebrew daiktas: Šešėlių apsiaustas",
            Content = "Retas magiškas daiktas. Kartą per ilgą poilsį nešiotojas gali tapti nematomas iki 1 minutės, jei stovi silpnoje šviesoje arba tamsoje. Puikiai tinka Rogue personažams.",
            CreatedAt = Utc(9, 10, 20, 0)
        },
        new()
        {
            Id = StrongPartyPostId,
            AuthorId = ModeratorUserId,
            CategoryId = DungeonMasterTipsCategoryId,
            Title = "Kaip suvaldyti per stiprią žaidėjų grupę",
            Content = "Jei grupė lengvai įveikia kovas, didinkite ne priešų gyvybes, o jų skaičių ir taktiką. Naudokite aplinką, laiko spaudimą ir tikslus, kurie nėra vien priešų nugalėjimas.",
            CreatedAt = Utc(9, 12, 9, 45)
        },
        new()
        {
            Id = DwarfPostId,
            AuthorId = DwarfTomasUserId,
            CategoryId = RacesCategoryId,
            Title = "Nykštukai (Dwarf): atsparumas ir tradicijos",
            Content = "Nykštukai mato tamsoje (Darkvision), yra atsparūs nuodams ir puikiai išmano akmens darbus. Dėl tvirtumo jie natūraliai tinka Fighter, Cleric ir Paladin klasėms.",
            CreatedAt = Utc(9, 15, 16, 20)
        },
        new()
        {
            Id = SwampWitchPostId,
            AuthorId = DragonHunterUserId,
            CategoryId = HomebrewCategoryId,
            Title = "Homebrew monstras: Pelkių ragana",
            Content = "Challenge Rating 4 priešininkė, gyvenanti pelkėse. Ji gali sukurti rūką, kuris riboja matomumą, ir prisikviesti dvi pelkių gyvates. Tinka 3–4 lygio grupei kaip sesijos finalas.",
            CreatedAt = Utc(9, 18, 21, 10)
        },
        new()
        {
            Id = FirstSessionPostId,
            AuthorId = AdminUserId,
            CategoryId = DungeonMasterTipsCategoryId,
            Title = "Kaip pasiruošti pirmajai sesijai per valandą",
            Content = "Užtenka trijų dalykų: aiškaus nuotykio kabliuko, vienos kovos ir vieno socialinio susitikimo. Nerašykite viso scenarijaus – leiskite žaidėjų sprendimams nulemti eigą.",
            CreatedAt = Utc(9, 22, 8, 0)
        },
        new()
        {
            Id = FireballPostId,
            AuthorId = WizardIevaUserId,
            CategoryId = SpellsCategoryId,
            Title = "Fireball: kada verta, o kada ne",
            Content = "Fireball yra 3 lygio burtas, darantis 8d6 ugnies žalos 20 pėdų spinduliu. Jis puikus prieš priešų grupes, bet uždarose patalpose lengva užkabinti ir savo sąjungininkus.",
            CreatedAt = Utc(9, 25, 19, 40)
        },
        new()
        {
            Id = ConcentrationPostId,
            AuthorId = ModeratorUserId,
            CategoryId = SpellsCategoryId,
            Title = "Concentration taisyklė paprastai",
            Content = "Vienu metu galima palaikyti tik vieną Concentration burtą. Gavęs žalos, burtininkas meta Constitution gelbėjimo metimą: sudėtingumas yra 10 arba pusė gautos žalos, jei ji didesnė.",
            CreatedAt = Utc(9, 28, 12, 5)
        }
    ];

    private static Comment[] Comments() =>
    [
        new() { Id = 1, AuthorId = WizardIevaUserId, PostId = PaladinPostId, Content = "Oath of Vengeance man atrodo geriausias pasirinkimas pradedantiesiems.", CreatedAt = Utc(9, 1, 12, 5) },
        new() { Id = 2, AuthorId = DwarfTomasUserId, PostId = PaladinPostId, Content = "Verta pridėti, kad Divine Smite su kritiniu smūgiu dvigubina ir smite kauliukus.", CreatedAt = Utc(9, 2, 8, 40) },
        new() { Id = 3, AuthorId = DragonHunterUserId, PostId = WizardPostId, Content = "Shield burtas mano vedlį išgelbėjo ne vieną kartą.", CreatedAt = Utc(9, 4, 19, 10) },
        new() { Id = 4, AuthorId = DragonHunterUserId, PostId = ShadowCloakPostId, Content = "Ar šiam apsiaustui reikia attunement?", CreatedAt = Utc(9, 11, 7, 20) },
        new() { Id = 5, AuthorId = DwarfTomasUserId, PostId = ShadowCloakPostId, Content = "Taip, reikia. Pridėsiu tai į aprašymą.", CreatedAt = Utc(9, 11, 9, 0) },
        new() { Id = 6, AuthorId = WizardIevaUserId, PostId = StrongPartyPostId, Content = "Mes pridedame priešų bangomis, ir kovos vėl tampa įdomios.", CreatedAt = Utc(9, 13, 21, 30) },
        new() { Id = 7, AuthorId = DwarfTomasUserId, PostId = WizardPostId, Content = "Find Familiar pelėda dar gali duoti Help veiksmą ir nusiskristi neišprovokuodama atakos.", CreatedAt = Utc(9, 5, 10, 15) },
        new() { Id = 8, AuthorId = DragonHunterUserId, PostId = TieflingPostId, Content = "Mano tiefling warlock su Hellish Rebuke jau ne kartą nustebino DM.", CreatedAt = Utc(9, 6, 17, 45) },
        new() { Id = 9, AuthorId = WizardIevaUserId, PostId = DwarfPostId, Content = "Mountain dwarf gauna vidutinių šarvų įgūdį, todėl tinka net wizard klasei.", CreatedAt = Utc(9, 16, 11, 0) },
        new() { Id = 10, AuthorId = DwarfTomasUserId, PostId = SwampWitchPostId, Content = "Išbandėme vakar: rūkas tikrai pakeitė kovos taktiką, labai patiko.", CreatedAt = Utc(9, 20, 22, 5) },
        new() { Id = 11, AuthorId = WizardIevaUserId, PostId = SwampWitchPostId, Content = "Siūlyčiau gyvatėms sumažinti gyvybes, kitaip kova užsitęsia.", CreatedAt = Utc(9, 21, 8, 30) },
        new() { Id = 12, AuthorId = DragonHunterUserId, PostId = FirstSessionPostId, Content = "Socialinis susitikimas taverne – klasika, kuri visada veikia.", CreatedAt = Utc(9, 23, 13, 50) },
        new() { Id = 13, AuthorId = DwarfTomasUserId, PostId = FireballPostId, Content = "Prieš kiekvieną Fireball klausiu grupės, ar visi stovi už spindulio ribų.", CreatedAt = Utc(9, 26, 9, 25) },
        new() { Id = 14, AuthorId = ModeratorUserId, PostId = FireballPostId, Content = "Verta paminėti, kad aukštesnio lygio vieta prideda po 1d6 žalos.", CreatedAt = Utc(9, 27, 15, 0) },
        new() { Id = 15, AuthorId = WizardIevaUserId, PostId = ConcentrationPostId, Content = "War Caster feat labai padeda išlaikyti koncentraciją kovoje.", CreatedAt = Utc(9, 29, 18, 35) }
    ];
}
