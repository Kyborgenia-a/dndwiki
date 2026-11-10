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
        modelBuilder.Entity<Category>().HasData(Categories());
        modelBuilder.Entity<Post>().HasData(Posts());
        modelBuilder.Entity<Comment>().HasData(Comments());
    }

    private static DateTime Utc(int month, int day, int hour, int minute) =>
        new(2026, month, day, hour, minute, 0, DateTimeKind.Utc);

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
            CategoryId = ClassesCategoryId,
            Title = "Paladino klasės apžvalga",
            Content = "Paladinas – šventas karys, derinantis kovą artimu atstumu su dieviška magija. Svarbiausios savybės yra Strength ir Charisma. Divine Smite leidžia paversti burtų vietas papildoma radiant žala.",
            CreatedAt = Utc(9, 1, 10, 0)
        },
        new()
        {
            Id = WizardPostId,
            CategoryId = ClassesCategoryId,
            Title = "Kaip išgyventi su Wizard pirmuose lygiuose",
            Content = "Pirmuose lygiuose vedlys turi mažai gyvybių, todėl laikykitės atokiau nuo priešų. Rinkitės Shield ir Mage Armor, o Find Familiar padės žvalgyti nerizikuojant savimi.",
            CreatedAt = Utc(9, 3, 18, 30)
        },
        new()
        {
            Id = TieflingPostId,
            CategoryId = RacesCategoryId,
            Title = "Tiefling rasė: istorija ir savybės",
            Content = "Tieflingai turi pragariškos kilmės kraujo. Jie atsparūs ugnies žalai ir paveldi įgimtus burtus, pavyzdžiui, Hellish Rebuke. Puikiai tinka Warlock ar Sorcerer klasėms.",
            CreatedAt = Utc(9, 5, 14, 15)
        },
        new()
        {
            Id = ShadowCloakPostId,
            CategoryId = HomebrewCategoryId,
            Title = "Homebrew daiktas: Šešėlių apsiaustas",
            Content = "Retas magiškas daiktas. Kartą per ilgą poilsį nešiotojas gali tapti nematomas iki 1 minutės, jei stovi silpnoje šviesoje arba tamsoje. Puikiai tinka Rogue personažams.",
            CreatedAt = Utc(9, 10, 20, 0)
        },
        new()
        {
            Id = StrongPartyPostId,
            CategoryId = DungeonMasterTipsCategoryId,
            Title = "Kaip suvaldyti per stiprią žaidėjų grupę",
            Content = "Jei grupė lengvai įveikia kovas, didinkite ne priešų gyvybes, o jų skaičių ir taktiką. Naudokite aplinką, laiko spaudimą ir tikslus, kurie nėra vien priešų nugalėjimas.",
            CreatedAt = Utc(9, 12, 9, 45)
        },
        new()
        {
            Id = DwarfPostId,
            CategoryId = RacesCategoryId,
            Title = "Nykštukai (Dwarf): atsparumas ir tradicijos",
            Content = "Nykštukai mato tamsoje (Darkvision), yra atsparūs nuodams ir puikiai išmano akmens darbus. Dėl tvirtumo jie natūraliai tinka Fighter, Cleric ir Paladin klasėms.",
            CreatedAt = Utc(9, 15, 16, 20)
        },
        new()
        {
            Id = SwampWitchPostId,
            CategoryId = HomebrewCategoryId,
            Title = "Homebrew monstras: Pelkių ragana",
            Content = "Challenge Rating 4 priešininkė, gyvenanti pelkėse. Ji gali sukurti rūką, kuris riboja matomumą, ir prisikviesti dvi pelkių gyvates. Tinka 3–4 lygio grupei kaip sesijos finalas.",
            CreatedAt = Utc(9, 18, 21, 10)
        },
        new()
        {
            Id = FirstSessionPostId,
            CategoryId = DungeonMasterTipsCategoryId,
            Title = "Kaip pasiruošti pirmajai sesijai per valandą",
            Content = "Užtenka trijų dalykų: aiškaus nuotykio kabliuko, vienos kovos ir vieno socialinio susitikimo. Nerašykite viso scenarijaus – leiskite žaidėjų sprendimams nulemti eigą.",
            CreatedAt = Utc(9, 22, 8, 0)
        },
        new()
        {
            Id = FireballPostId,
            CategoryId = SpellsCategoryId,
            Title = "Fireball: kada verta, o kada ne",
            Content = "Fireball yra 3 lygio burtas, darantis 8d6 ugnies žalos 20 pėdų spinduliu. Jis puikus prieš priešų grupes, bet uždarose patalpose lengva užkabinti ir savo sąjungininkus.",
            CreatedAt = Utc(9, 25, 19, 40)
        },
        new()
        {
            Id = ConcentrationPostId,
            CategoryId = SpellsCategoryId,
            Title = "Concentration taisyklė paprastai",
            Content = "Vienu metu galima palaikyti tik vieną Concentration burtą. Gavęs žalos, burtininkas meta Constitution gelbėjimo metimą: sudėtingumas yra 10 arba pusė gautos žalos, jei ji didesnė.",
            CreatedAt = Utc(9, 28, 12, 5)
        }
    ];

    private static Comment[] Comments() =>
    [
        new() { Id = 1, PostId = PaladinPostId, Content = "Oath of Vengeance man atrodo geriausias pasirinkimas pradedantiesiems.", CreatedAt = Utc(9, 1, 12, 5) },
        new() { Id = 2, PostId = PaladinPostId, Content = "Verta pridėti, kad Divine Smite su kritiniu smūgiu dvigubina ir smite kauliukus.", CreatedAt = Utc(9, 2, 8, 40) },
        new() { Id = 3, PostId = WizardPostId, Content = "Shield burtas mano vedlį išgelbėjo ne vieną kartą.", CreatedAt = Utc(9, 4, 19, 10) },
        new() { Id = 4, PostId = ShadowCloakPostId, Content = "Ar šiam apsiaustui reikia attunement?", CreatedAt = Utc(9, 11, 7, 20) },
        new() { Id = 5, PostId = ShadowCloakPostId, Content = "Taip, reikia. Pridėsiu tai į aprašymą.", CreatedAt = Utc(9, 11, 9, 0) },
        new() { Id = 6, PostId = StrongPartyPostId, Content = "Mes pridedame priešų bangomis, ir kovos vėl tampa įdomios.", CreatedAt = Utc(9, 13, 21, 30) },
        new() { Id = 7, PostId = WizardPostId, Content = "Find Familiar pelėda dar gali duoti Help veiksmą ir nusiskristi neišprovokuodama atakos.", CreatedAt = Utc(9, 5, 10, 15) },
        new() { Id = 8, PostId = TieflingPostId, Content = "Mano tiefling warlock su Hellish Rebuke jau ne kartą nustebino DM.", CreatedAt = Utc(9, 6, 17, 45) },
        new() { Id = 9, PostId = DwarfPostId, Content = "Mountain dwarf gauna vidutinių šarvų įgūdį, todėl tinka net wizard klasei.", CreatedAt = Utc(9, 16, 11, 0) },
        new() { Id = 10, PostId = SwampWitchPostId, Content = "Išbandėme vakar: rūkas tikrai pakeitė kovos taktiką, labai patiko.", CreatedAt = Utc(9, 20, 22, 5) },
        new() { Id = 11, PostId = SwampWitchPostId, Content = "Siūlyčiau gyvatėms sumažinti gyvybes, kitaip kova užsitęsia.", CreatedAt = Utc(9, 21, 8, 30) },
        new() { Id = 12, PostId = FirstSessionPostId, Content = "Socialinis susitikimas taverne – klasika, kuri visada veikia.", CreatedAt = Utc(9, 23, 13, 50) },
        new() { Id = 13, PostId = FireballPostId, Content = "Prieš kiekvieną Fireball klausiu grupės, ar visi stovi už spindulio ribų.", CreatedAt = Utc(9, 26, 9, 25) },
        new() { Id = 14, PostId = FireballPostId, Content = "Verta paminėti, kad aukštesnio lygio vieta prideda po 1d6 žalos.", CreatedAt = Utc(9, 27, 15, 0) },
        new() { Id = 15, PostId = ConcentrationPostId, Content = "War Caster feat labai padeda išlaikyti koncentraciją kovoje.", CreatedAt = Utc(9, 29, 18, 35) }
    ];
}
