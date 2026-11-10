# D&D Forum API

REST API forumui ir wiki apie stalo vaidmenų žaidimą „Dungeons & Dragons“. Sistemoje vartotojai dalijasi savo kūryba, diskutuoja ir randa informaciją apie žaidimą: klases, rases, homebrew turinį ir patarimus žaidimų vedėjams.

## Technologijos

| Sluoksnis | Technologija |
|---|---|
| Back end | ASP.NET Core Web API (.NET 10) |
| Duomenų bazė | MySQL 8 |
| ORM | Entity Framework Core + Pomelo MySQL provider |
| Autentifikacija | JWT (Microsoft.AspNetCore.Authentication.JwtBearer), refresh token su rotacija |
| Slaptažodžiai | ASP.NET Core `PasswordHasher` (PBKDF2-HMAC-SHA512, 100 000 iteracijų) |
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
| Category | `id`, `name` (unikalus, 2–100 simb.), `description` (iki 500 simb.), `createdAt` |
| Post | `id`, `title` (3–200 simb.), `content` (10–10000 simb.), `createdAt`, `categoryId`, `authorId` |
| Comment | `id`, `content` (1–2000 simb.), `createdAt`, `postId`, `authorId` |

Naudotojai (`User`) ir jų prisijungimo sesijos (`UserSession`) nėra taikomosios srities hierarchijos dalis. Įrašas ir komentaras tik nurodo savo autorių.

## Paleidimas

### Reikalavimai

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- MySQL Server 8 (veikiantis `localhost:3306`)
- Visual Studio 2022/2026 arba `dotnet` komandinė eilutė

### Žingsniai

1. Nusiklonuokite repozitoriją:

   ```bash
   git clone <repozitorijos-adresas>
   ```

2. Projekto aplanke nustatykite MySQL prisijungimo eilutę. Slaptažodis saugomas User Secrets, todėl į repozitoriją nepatenka:

   ```bash
   dotnet user-secrets set "ConnectionStrings:ForumDatabase" "server=localhost;port=3306;database=dnd_forum;user=root;password=JUSU_SLAPTAZODIS"
   ```

   Ten pat nustatykite JWT pasirašymo raktą (bent 32 simboliai, atsitiktinė eilutė):

   ```bash
   dotnet user-secrets set "Jwt:SigningKey" "IRASYKITE_ATSITIKTINE_BENT_32_SIMBOLIU_EILUTE"
   ```

   Jei raktas nenustatytas arba per trumpas, programa nepasileidžia ir parašo, ko trūksta.

3. Paleiskite programą: Visual Studio spauskite F5 (profilis `https`) arba komandinėje eilutėje vykdykite:

   ```bash
   dotnet run --launch-profile https
   ```

Paleidimo metu programa automatiškai pritaiko EF Core migracijas. Jei `dnd_forum` duomenų bazės nėra, ji sukuriama ir užpildoma pradiniais duomenimis: 5 naudotojais, 5 kategorijomis, 10 įrašų ir 15 komentarų.

### Demonstraciniai naudotojai

| Vardas | Slaptažodis | Rolė |
|---|---|---|
| `admin` | `Admin123!` | Admin |
| `moderatorius` | `Moder123!` | Moderator |
| `drakonu_medziotojas` | `Narys123!` | Member |
| `burtininke_ieva` | `Narys123!` | Member |
| `nykstukas_tomas` | `Narys123!` | Member |

API pasiekiamas adresu `https://localhost:7135/api`. Šis adresas grąžina nuorodas į visus pagrindinius resursus.

## API metodai

Visi metodai priima ir grąžina JSON. Stulpelyje „Prieiga“: **Viešas** – be prisijungimo, **Prisijungęs** – bet kuri rolė, **Autorius/Mod.** – turinio autorius, Moderator arba Admin.

### Autentifikacija

| Metodas | Kelias | Aprašymas | Prieiga | Sėkmės kodas |
|---|---|---|---|---|
| POST | `/api/auth/register` | Registracija (visada rolė Member) | Viešas | 201 |
| POST | `/api/auth/login` | Prisijungimas: access token atsakyme, refresh token cookie | Viešas | 200 |
| POST | `/api/auth/refresh` | Naujas access token ir naujas refresh token (rotacija) | Refresh cookie | 200 |
| POST | `/api/auth/logout` | Atsijungimas: sesija panaikinama | Prisijungęs | 204 |
| GET | `/api/auth/me` | Prisijungusio naudotojo duomenys | Prisijungęs | 200 |

### Naudotojų valdymas

| Metodas | Kelias | Aprašymas | Prieiga | Sėkmės kodas |
|---|---|---|---|---|
| GET | `/api/users` | Naudotojų sąrašas | Admin | 200 |
| GET | `/api/users/{userId}` | Vienas naudotojas | Admin | 200 |
| PUT | `/api/users/{userId}/role` | Pakeisti rolę (naudotojo sesijos uždaromos) | Admin | 200 |
| DELETE | `/api/users/{userId}` | Ištrinti naudotoją su jo turiniu | Admin | 204 |

### Kategorijos

| Metodas | Kelias | Aprašymas | Prieiga | Sėkmės kodas |
|---|---|---|---|---|
| GET | `/api/categories` | Visų kategorijų sąrašas | Viešas | 200 |
| GET | `/api/categories/{categoryId}` | Viena kategorija | Viešas | 200 |
| POST | `/api/categories` | Sukurti kategoriją | Admin | 201 |
| PUT | `/api/categories/{categoryId}` | Atnaujinti kategoriją | Admin | 200 |
| DELETE | `/api/categories/{categoryId}` | Ištrinti kategoriją su įrašais ir komentarais | Admin | 204 |

### Įrašai

| Metodas | Kelias | Aprašymas | Prieiga | Sėkmės kodas |
|---|---|---|---|---|
| GET | `/api/categories/{categoryId}/posts` | Kategorijos įrašų sąrašas | Viešas | 200 |
| GET | `/api/categories/{categoryId}/posts/{postId}` | Vienas įrašas | Viešas | 200 |
| POST | `/api/categories/{categoryId}/posts` | Sukurti įrašą | Prisijungęs | 201 |
| PUT | `/api/categories/{categoryId}/posts/{postId}` | Atnaujinti įrašą | Autorius/Mod. | 200 |
| DELETE | `/api/categories/{categoryId}/posts/{postId}` | Ištrinti įrašą su komentarais | Autorius/Mod. | 204 |

### Komentarai

| Metodas | Kelias | Aprašymas | Prieiga | Sėkmės kodas |
|---|---|---|---|---|
| GET | `/api/categories/{categoryId}/posts/{postId}/comments` | Įrašo komentarų sąrašas | Viešas | 200 |
| GET | `/api/categories/{categoryId}/posts/{postId}/comments/{commentId}` | Vienas komentaras | Viešas | 200 |
| POST | `/api/categories/{categoryId}/posts/{postId}/comments` | Sukurti komentarą | Prisijungęs | 201 |
| PUT | `/api/categories/{categoryId}/posts/{postId}/comments/{commentId}` | Atnaujinti komentarą | Autorius/Mod. | 200 |
| DELETE | `/api/categories/{categoryId}/posts/{postId}/comments/{commentId}` | Ištrinti komentarą | Autorius/Mod. | 204 |

### Papildomi metodai

| Metodas | Kelias | Aprašymas | Prieiga | Sėkmės kodas |
|---|---|---|---|---|
| GET | `/api` | API pradžios taškas su nuorodomis į resursus | Viešas | 200 |
| GET | `/api/dashboard` | Forumo panelė, sudaryta iš kategorijų, įrašų, komentarų ir naudotojų | Prisijungęs | 200 |

## Autentifikacija ir autorizacija

### Rolės

| Rolė | Teisės |
|---|---|
| Svečias (be tokeno) | Skaito kategorijas, įrašus ir komentarus, registruojasi, jungiasi |
| Member | Kaip svečias, plius: kuria įrašus ir komentarus, redaguoja ir trina **tik savo** turinį, mato panelę |
| Moderator | Kaip Member, plius: redaguoja ir trina **bet kurį** įrašą ar komentarą |
| Admin | Kaip Moderator, plius: valdo kategorijas ir naudotojus (sąrašas, rolės keitimas, šalinimas) |

Rolės tikrinamos dviem lygiais:

- `[Authorize(Roles = Roles.Admin)]` atributai kontroleriuose – pagal rolę tokene;
- resursų lygio politika `AuthorOrModerator` (`Auth/AuthorOrModeratorRequirement.cs`) – įrašą ar komentarą keisti leidžiama, jei tokeno `sub` sutampa su `authorId` arba rolė yra Moderator/Admin.

Registracijos metu rolė visada `Member`, net jei užklausoje nurodyta kita. Administratorius negali pakeisti savo rolės ar ištrinti savęs.

### Tokenai

| Tokenas | Formatas | Galiojimas | Kur perduodamas | Kur saugomas serveryje |
|---|---|---|---|---|
| Access token | JWT, pasirašytas HS256 | 15 min. | `Authorization: Bearer <token>` antraštė | Nesaugomas (tikrinamas parašas) |
| Refresh token | `{sesijosId}.{atsitiktinė 256 bitų paslaptis}` | 7 dienos, pratęsiama kiekvieno atnaujinimo metu | `HttpOnly`, `Secure`, `SameSite=Strict` cookie, `Path=/api/auth` | Lentelėje `UserSessions` tik SHA-256 maiša |

Access token turinys (payload):

```json
{
  "sub": "3",
  "name": "drakonu_medziotojas",
  "role": "Member",
  "sid": "8f6c3c1e-...",
  "jti": "1b2a...",
  "iss": "DndForum.Api",
  "aud": "DndForum.Client",
  "iat": 1762770000,
  "nbf": 1762770000,
  "exp": 1762770900
}
```

Tokenas kuriamas `Auth/AccessTokenFactory.cs`, tikrinamas `Auth/AuthServiceCollectionExtensions.cs`. Priimamas tik HS256 algoritmas, todėl tokenai su `"alg": "none"` ar kitu algoritmu atmetami. Pasirašymo raktas laikomas User Secrets (`Jwt:SigningKey`), produkcijoje – aplinkos kintamajame.

### Atnaujinimo strategija

1. `POST /api/auth/login` sukuria sesiją ir grąžina access token bei refresh token cookie.
2. Kai access token baigia galioti, klientas kviečia `POST /api/auth/refresh`. Serveris patikrina refresh token, **pakeičia jį nauju** (rotacija) ir grąžina naują access token.
3. Jei atsiunčiamas jau pakeistas (senas) refresh token, laikoma, kad jis pavogtas, ir visa sesija uždaroma.
4. `POST /api/auth/logout` pažymi sesiją kaip panaikintą. Po to neveikia nei refresh token, nei tos sesijos access token, nes kiekvienos užklausos metu `SessionValidatingJwtBearerEvents` patikrina, ar tokeno `sid` sesija dar aktyvi.
5. Pakeitus naudotojo rolę ar jį ištrynus, visos jo sesijos uždaromos iškart.

### Apsauga nuo pagrindinių grėsmių

| Grėsmė | Apsauga | Kur realizuota |
|---|---|---|
| SQL injekcija | Visos užklausos per EF Core LINQ, reikšmės perduodamos parametrais | Kontroleriai |
| Slaptažodžių nutekėjimas | Saugomi tik PBKDF2 maišos su druska | `PasswordHasher<User>`, `AuthController` |
| Teisių eskalacija | Rolė nustatoma tik serveryje; registracijos DTO neturi rolės lauko; autoriaus ID imamas iš tokeno | `RegisterRequest`, `PostsController.Create` |
| Svetimo turinio keitimas | Resursų lygio autorizacija | `AuthorOrModeratorHandler` |
| Suklastotas tokenas | Parašo, išdavėjo, auditorijos, galiojimo ir algoritmo tikrinimas | `AuthServiceCollectionExtensions` |
| XSS (tokeno vagystė per JavaScript) | Refresh token `HttpOnly` cookie; access token trumpalaikis | `RefreshTokenCookie` |
| CSRF | `SameSite=Strict`; API veiksmams reikia `Authorization` antraštės, kurios naršyklė automatiškai neprideda | `RefreshTokenCookie` |
| Pavogtas refresh token | Rotacija ir pakartotinio panaudojimo aptikimas | `SessionTokenService.RefreshAsync` |
| Vardų spėliojimas | Tas pats 401 atsakymas neteisingam vardui ir slaptažodžiui | `AuthController.Login` |

## Puslapiavimas ir filtravimas

Visi sąrašo metodai (`GET .../categories`, `.../posts`, `.../comments`) puslapiuojami:

| Parametras | Numatyta | Ribos |
|---|---|---|
| `page` | 1 | 1–10000 |
| `pageSize` | 10 | 1–50 |

Filtrai:

| Resursas | Parametrai |
|---|---|
| Kategorijos | `search` – ieško pavadinime ir aprašyme |
| Įrašai | `search` – ieško pavadinime ir turinyje; `createdFrom`, `createdTo` – sukūrimo laiko intervalas (imtinai) |
| Komentarai | `search` – ieško komentaro tekste |

Pavyzdys: `GET /api/categories/1/posts?search=burt&createdFrom=2026-09-01&page=1&pageSize=5`

Sąrašo atsakymas:

```json
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 5,
  "totalCount": 12,
  "totalPages": 3,
  "links": [
    { "rel": "self", "href": "/api/categories/1/posts?search=burt&page=1&pageSize=5", "method": "GET" },
    { "rel": "first", "href": "...", "method": "GET" },
    { "rel": "last", "href": "...", "method": "GET" },
    { "rel": "next", "href": "...", "method": "GET" }
  ]
}
```

Netinkamos reikšmės (`pageSize=0`, `createdFrom` vėlesnė už `createdTo`) grąžina 422.

## Hypermedia (HATEOAS)

Kiekvienas resursas turi `links` masyvą su galimais veiksmais ir susijusiais resursais, todėl klientui nereikia pačiam konstruoti adresų:

| Resursas | Nuorodos (`rel`) |
|---|---|
| Kategorija | `self`, `update`, `delete`, `posts`, `create-post` |
| Įrašas | `self`, `update`, `delete`, `category`, `comments`, `create-comment` |
| Komentaras | `self`, `update`, `delete`, `post` |
| Sąrašo puslapis | `self`, `first`, `last`, `previous`, `next` |

Nuorodos generuojamos iš kontrolerių maršrutų (`Url.Action`), todėl pakeitus maršrutą jos atsinaujina automatiškai.

## Forumo panelė

`GET /api/dashboard` grąžina resursą, sukonstruotą iš visų trijų esybių:

- `totals` – kategorijų, įrašų ir komentarų skaičiai;
- `categories` – kiekvienos kategorijos įrašų ir komentarų skaičius bei paskutinio įrašo data;
- `mostDiscussedPosts` – 5 labiausiai komentuojami įrašai su kategorijos ir autoriaus vardais;
- `latestComments` – 5 naujausi komentarai su autoriaus, įrašo ir kategorijos pavadinimais.

### Užklausų pavyzdžiai

Prisijungti:

```http
POST /api/auth/login
Content-Type: application/json

{
  "userName": "admin",
  "password": "Admin123!"
}
```

Sukurti kategoriją (reikia Admin tokeno):

```http
POST /api/categories
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "name": "Monstrai",
  "description": "Monstrų aprašymai ir kovos su jais patarimai."
}
```

Sukurti įrašą (reikia bet kurio prisijungusio naudotojo tokeno):

```http
POST /api/categories/1/posts
Authorization: Bearer <accessToken>
Content-Type: application/json

{
  "title": "Druido klasės apžvalga",
  "content": "Druidai valdo gamtos magiją ir gali virsti gyvūnais (Wild Shape)."
}
```

Sukurti komentarą:

```http
POST /api/categories/1/posts/1/comments
Authorization: Bearer <accessToken>
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
| 401 Unauthorized | Nėra tokeno, jis suklastotas, pasibaigęs arba jo sesija panaikinta (atsijungus). Taip pat neteisingas vardas ar slaptažodis, netinkamas refresh token |
| 403 Forbidden | Tokenas galioja, bet rolė per žema arba naudotojas nėra turinio autorius. Administratorius bando keisti savo rolę ar ištrinti save |
| 404 Not Found | Bent vienas kelio resursas neegzistuoja, nepriklauso nurodytam tėviniam resursui, arba adresas neegzistuoja |
| 409 Conflict | Kategorija tokiu pavadinimu jau egzistuoja, arba registruojamas jau užimtas vardas ar el. paštas |
| 422 Unprocessable Entity | JSON tinkamas, bet reikšmės pažeidžia validacijos taisykles (tuščias laukas, netinkamas ilgis), arba netinkami puslapiavimo ar filtro parametrai |

Klaidų atsakymai pateikiami [Problem Details](https://www.rfc-editor.org/rfc/rfc9457) formatu. Validacijos klaidų atveju lauke `errors` nurodoma, kurie laukai netinkami.

## OpenAPI specifikacija

Programai veikiant, specifikacija pasiekiama adresu `https://localhost:7135/openapi/v1.json`. Joje aprašyti visi metodai su santraukomis, prieigos aprašymu, parametrais, užklausų bei atsakymų schemomis ir galimais atsako kodais.

Statinė specifikacijos kopija saugoma faile [`docs/openapi.json`](docs/openapi.json).

## Testavimas su Postman

Kolekcija [`postman/DndForum.postman_collection.json`](postman/DndForum.postman_collection.json) automatiškai patikrina registraciją, prisijungimą, tokeno turinį ir cookie vėliavas, refresh rotaciją, atsijungimą, visus 15 CRUD metodų su skirtingomis rolėmis, savininko patikrą, puslapiavimą, filtravimą, hypermedia, forumo panelę ir klaidų atvejus (400, 401, 403, 404, 409, 422).

1. Postman Desktop programoje spauskite Import ir pasirinkite kolekcijos failą.
2. Settings → General išjunkite „SSL certificate verification“, nes vietinis HTTPS sertifikatas yra savarankiškai pasirašytas.
3. Paleiskite API.
4. Ant kolekcijos spauskite Run collection → Run.

Kolekcija pati užregistruoja testinį naudotoją, susikuria testinius duomenis ir pabaigoje juos ištrina, todėl pradiniai duomenys lieka nepakitę ir testus galima leisti pakartotinai.

## Projekto struktūra

```
├── Auth/            JWT, refresh token sesijos, cookie, rolės ir autorizacijos politika
├── Controllers/     API kontroleriai (Auth, Users, Categories, Posts, Comments, Dashboard, ApiRoot)
├── Data/            DbContext ir pradiniai duomenys
├── Dtos/            Užklausų ir atsakymų modeliai
├── Hypermedia/      HATEOAS nuorodų ir puslapių kūrimas
├── Migrations/      EF Core migracijos
├── Models/          Duomenų bazės esybės
├── Queries/         Puslapiavimo ir filtrų parametrai
├── Validation/      400 ir 422 klaidų atskyrimas
├── docs/            Statinė OpenAPI specifikacija
├── postman/         Postman kolekcija su testais
└── Program.cs       Programos konfigūracija
```
