# D&D Forum API

REST API forumui ir wiki apie stalo vaidmenų žaidimą „Dungeons & Dragons“. Sistemoje vartotojai dalijasi savo kūryba, diskutuoja ir randa informaciją apie žaidimą: klases, rases, homebrew turinį ir patarimus žaidimų vedėjams.

## Technologijos

| Sluoksnis | Technologija |
|---|---|
| Back end | ASP.NET Core Web API (.NET 10) |
| Duomenų bazė | MySQL 8 |
| ORM | Entity Framework Core + Pomelo MySQL provider |
| API specifikacija | OpenAPI 3.1 (Microsoft.AspNetCore.OpenApi) |
| Testavimas | Postman kolekcija su testais |
| Front end | React.js (vėlesniuose etapuose) |

## Duomenų modelis

API sudaro trys hierarchiškai susiję resursai:

```
Kategorija (Category)
└── Įrašas (Post)
    └── Komentaras (Comment)
```

Kategorija turi daug įrašų, įrašas turi daug komentarų. Ištrynus kategoriją, kartu ištrinami visi jos įrašai ir jų komentarai.

| Resursas | Laukai |
|---|---|
| Category | `id`, `name` (unikalus, 2–100 simb.), `description` (iki 500 simb.) |
| Post | `id`, `title` (3–200 simb.), `content` (10–10000 simb.), `createdAt`, `categoryId` |
| Comment | `id`, `content` (1–2000 simb.), `createdAt`, `postId` |

## Paleidimas

### Reikalavimai

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL Server 8 (veikiantis `localhost:3306`)
- Visual Studio 2022/2026 arba `dotnet` komandinė eilutė

### Žingsniai

1. Nusiklonuokite repozitoriją:

   ```bash
   git clone https://github.com/Kyborgenia-a/dndwiki
   ```

2. Projekto aplanke nustatykite MySQL prisijungimo eilutę. Slaptažodis saugomas User Secrets, todėl į repozitoriją nepatenka:

   ```bash
   dotnet user-secrets set "ConnectionStrings:ForumDatabase" "server=localhost;port=3306;database=dnd_forum;user=root;password=JUSU_SLAPTAZODIS"
   ```

3. Paleiskite programą: Visual Studio spauskite F5 (profilis `https`) arba komandinėje eilutėje vykdykite:

   ```bash
   dotnet run --launch-profile https
   ```

Paleidimo metu programa automatiškai pritaiko EF Core migracijas. Jei `dnd_forum` duomenų bazės nėra, ji sukuriama ir užpildoma pradiniais duomenimis: 4 kategorijomis, 5 įrašais ir 6 komentarais.

API pasiekiamas adresu `https://localhost:7135`.

## API metodai

Visi metodai priima ir grąžina JSON.

### Kategorijos

| Metodas | Kelias | Aprašymas | Sėkmės kodas |
|---|---|---|---|
| GET | `/api/categories` | Visų kategorijų sąrašas | 200 |
| GET | `/api/categories/{categoryId}` | Viena kategorija | 200 |
| POST | `/api/categories` | Sukurti kategoriją | 201 |
| PUT | `/api/categories/{categoryId}` | Atnaujinti kategoriją | 200 |
| DELETE | `/api/categories/{categoryId}` | Ištrinti kategoriją su įrašais ir komentarais | 204 |

### Įrašai

| Metodas | Kelias | Aprašymas | Sėkmės kodas |
|---|---|---|---|
| GET | `/api/categories/{categoryId}/posts` | Kategorijos įrašų sąrašas | 200 |
| GET | `/api/categories/{categoryId}/posts/{postId}` | Vienas įrašas | 200 |
| POST | `/api/categories/{categoryId}/posts` | Sukurti įrašą | 201 |
| PUT | `/api/categories/{categoryId}/posts/{postId}` | Atnaujinti įrašą | 200 |
| DELETE | `/api/categories/{categoryId}/posts/{postId}` | Ištrinti įrašą su komentarais | 204 |

### Komentarai

| Metodas | Kelias | Aprašymas | Sėkmės kodas |
|---|---|---|---|
| GET | `/api/categories/{categoryId}/posts/{postId}/comments` | Įrašo komentarų sąrašas | 200 |
| GET | `/api/categories/{categoryId}/posts/{postId}/comments/{commentId}` | Vienas komentaras | 200 |
| POST | `/api/categories/{categoryId}/posts/{postId}/comments` | Sukurti komentarą | 201 |
| PUT | `/api/categories/{categoryId}/posts/{postId}/comments/{commentId}` | Atnaujinti komentarą | 200 |
| DELETE | `/api/categories/{categoryId}/posts/{postId}/comments/{commentId}` | Ištrinti komentarą | 204 |

### Užklausų pavyzdžiai

Sukurti kategoriją:

```http
POST /api/categories
Content-Type: application/json

{
  "name": "Monstrai",
  "description": "Monstrų aprašymai ir kovos su jais patarimai."
}
```

Sukurti įrašą:

```http
POST /api/categories/1/posts
Content-Type: application/json

{
  "title": "Druido klasės apžvalga",
  "content": "Druidai valdo gamtos magiją ir gali virsti gyvūnais (Wild Shape)."
}
```

Sukurti komentarą:

```http
POST /api/categories/1/posts/1/comments
Content-Type: application/json

{
  "content": "Puiki apžvalga!"
}
```

## Atsako kodai

| Kodas | Kada grąžinamas |
|---|---|
| 200 OK | Sėkmingas GET arba PUT. Atsakyme grąžinamas resursas |
| 201 Created | Resursas sukurtas. Atsakyme grąžinamas resursas ir `Location` antraštė su jo adresu |
| 204 No Content | Resursas ištrintas. Atsakymas be turinio |
| 400 Bad Request | Užklausos turinys nėra tinkamas JSON (sintaksės klaida, neteisingas lauko tipas, tuščias turinys) |
| 404 Not Found | Bent vienas kelio resursas neegzistuoja arba nepriklauso nurodytam tėviniam resursui |
| 409 Conflict | Kategorija tokiu pavadinimu jau egzistuoja |
| 422 Unprocessable Entity | JSON tinkamas, bet reikšmės pažeidžia validacijos taisykles (tuščias laukas, netinkamas ilgis) |

Klaidų atsakymai pateikiami [Problem Details](https://www.rfc-editor.org/rfc/rfc9457) formatu. Validacijos klaidų atveju lauke `errors` nurodoma, kurie laukai netinkami.

## OpenAPI specifikacija

Programai veikiant, specifikacija pasiekiama adresu `https://localhost:7135/openapi/v1.json`. Joje aprašyti visi 15 metodų su santraukomis, parametrais, užklausų bei atsakymų schemomis ir galimais atsako kodais.

Statinė specifikacijos kopija saugoma faile [`docs/openapi.json`](docs/openapi.json).

## Testavimas su Postman

Kolekcija [`postman/DndForum.postman_collection.json`](postman/DndForum.postman_collection.json) automatiškai patikrina visus 15 metodų ir klaidų atvejus (400, 404, 409, 422).

1. Postman Desktop programoje spauskite Import ir pasirinkite kolekcijos failą.
2. Settings → General išjunkite „SSL certificate verification“, nes vietinis HTTPS sertifikatas yra savarankiškai pasirašytas.
3. Paleiskite API.
4. Ant kolekcijos spauskite Run collection → Run.

Kolekcija pati susikuria testinius duomenis ir pabaigoje juos ištrina, todėl pradiniai duomenys lieka nepakitę ir testus galima leisti pakartotinai.

## Projekto struktūra

```
├── Controllers/     API kontroleriai (Categories, Posts, Comments)
├── Data/            DbContext ir pradiniai duomenys
├── Dtos/            Užklausų ir atsakymų modeliai
├── Migrations/      EF Core migracijos
├── Models/          Duomenų bazės esybės
├── Validation/      400 ir 422 klaidų atskyrimas
├── docs/            Statinė OpenAPI specifikacija
├── postman/         Postman kolekcija su testais
└── Program.cs       Programos konfigūracija
```
