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

    private const int PaladinPostId = 1;
    private const int WizardPostId = 2;
    private const int TieflingPostId = 3;
    private const int ShadowCloakPostId = 4;
    private const int StrongPartyPostId = 5;

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(Categories());
        modelBuilder.Entity<Post>().HasData(Posts());
        modelBuilder.Entity<Comment>().HasData(Comments());
    }

    private static Category[] Categories() =>
    [
        new() { Id = ClassesCategoryId, Name = "Klasės", Description = "Žaidėjų klasių aprašymai, subklasės ir patarimai, kaip jas žaisti." },
        new() { Id = RacesCategoryId, Name = "Rasės", Description = "Žaidžiamų rasių savybės, istorija ir vaidmens patarimai." },
        new() { Id = HomebrewCategoryId, Name = "Homebrew kūryba", Description = "Vartotojų sukurti burtai, magiški daiktai, monstrai ir nuotykiai." },
        new() { Id = DungeonMasterTipsCategoryId, Name = "DM patarimai", Description = "Patarimai žaidimų vedėjams: sesijų planavimas, improvizacija ir balansas." }
    ];

    private static Post[] Posts() =>
    [
        new()
        {
            Id = PaladinPostId,
            CategoryId = ClassesCategoryId,
            Title = "Paladino klasės apžvalga",
            Content = "Paladinas – šventas karys, derinantis kovą artimu atstumu su dieviška magija. Svarbiausios savybės yra Strength ir Charisma. Divine Smite leidžia paversti burtų vietas papildoma radiant žala.",
            CreatedAt = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = WizardPostId,
            CategoryId = ClassesCategoryId,
            Title = "Kaip išgyventi su Wizard pirmuose lygiuose",
            Content = "Pirmuose lygiuose vedlys turi mažai gyvybių, todėl laikykitės atokiau nuo priešų. Rinkitės Shield ir Mage Armor, o Find Familiar padės žvalgyti nerizikuojant savimi.",
            CreatedAt = new DateTime(2026, 9, 3, 18, 30, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = TieflingPostId,
            CategoryId = RacesCategoryId,
            Title = "Tiefling rasė: istorija ir savybės",
            Content = "Tieflingai turi pragariškos kilmės kraujo. Jie atsparūs ugnies žalai ir paveldi įgimtus burtus, pavyzdžiui, Hellish Rebuke. Puikiai tinka Warlock ar Sorcerer klasėms.",
            CreatedAt = new DateTime(2026, 9, 5, 14, 15, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = ShadowCloakPostId,
            CategoryId = HomebrewCategoryId,
            Title = "Homebrew daiktas: Šešėlių apsiaustas",
            Content = "Retas magiškas daiktas. Kartą per ilgą poilsį nešiotojas gali tapti nematomas iki 1 minutės, jei stovi silpnoje šviesoje arba tamsoje. Puikiai tinka Rogue personažams.",
            CreatedAt = new DateTime(2026, 9, 10, 20, 0, 0, DateTimeKind.Utc)
        },
        new()
        {
            Id = StrongPartyPostId,
            CategoryId = DungeonMasterTipsCategoryId,
            Title = "Kaip suvaldyti per stiprią žaidėjų grupę",
            Content = "Jei grupė lengvai įveikia kovas, didinkite ne priešų gyvybes, o jų skaičių ir taktiką. Naudokite aplinką, laiko spaudimą ir tikslus, kurie nėra vien priešų nugalėjimas.",
            CreatedAt = new DateTime(2026, 9, 12, 9, 45, 0, DateTimeKind.Utc)
        }
    ];

    private static Comment[] Comments() =>
    [
        new() { Id = 1, PostId = PaladinPostId, Content = "Oath of Vengeance man atrodo geriausias pasirinkimas pradedantiesiems.", CreatedAt = new DateTime(2026, 9, 1, 12, 5, 0, DateTimeKind.Utc) },
        new() { Id = 2, PostId = PaladinPostId, Content = "Verta pridėti, kad Divine Smite su kritiniu smūgiu dvigubina ir smite kauliukus.", CreatedAt = new DateTime(2026, 9, 2, 8, 40, 0, DateTimeKind.Utc) },
        new() { Id = 3, PostId = WizardPostId, Content = "Shield burtas mano vedlį išgelbėjo ne vieną kartą.", CreatedAt = new DateTime(2026, 9, 4, 19, 10, 0, DateTimeKind.Utc) },
        new() { Id = 4, PostId = ShadowCloakPostId, Content = "Ar šiam apsiaustui reikia attunement?", CreatedAt = new DateTime(2026, 9, 11, 7, 20, 0, DateTimeKind.Utc) },
        new() { Id = 5, PostId = ShadowCloakPostId, Content = "Taip, reikia. Pridėsiu tai į aprašymą.", CreatedAt = new DateTime(2026, 9, 11, 9, 0, 0, DateTimeKind.Utc) },
        new() { Id = 6, PostId = StrongPartyPostId, Content = "Mes pridedame priešų bangomis, ir kovos vėl tampa įdomios.", CreatedAt = new DateTime(2026, 9, 13, 21, 30, 0, DateTimeKind.Utc) }
    ];
}
