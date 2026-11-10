# Loomade varjupaik

Varjupaiga süsteem loomade ja adopteerimise haldamiseks.

## Projektid

- **Varjupaik.Api** – Web API (kontrollerid, autentimine, veebileht `wwwroot`)
- **Varjupaik.Services** – andmemudelid, `AppDbContext` ja äriloogika teenused
- **Varjupaik.Worker** – taustateenus (kasutab API-t `HttpClient`-iga, ei viita `DbContext`-ile)

Viited: `Api -> Services`. Worker ei viita teistele projektidele.

## Käivitamine

```bash
dotnet build
dotnet run --project Varjupaik.Api
```

Veebileht avaneb aadressil http://localhost:5156

## Testkasutajad (appsettings.Development.json)

| Roll | Email | Parool |
|------|-------|--------|
| Admin | admin@varjupaik.ee | Admin123! |
| Staff | staff@varjupaik.ee | Staff123! |
| Service | service@varjupaik.ee | Service123! |

Külastaja (Visitor) konto saab teha lehel "Logi sisse" → "Registreeru".

## API (hinne 3)

| Meetod | Aadress | Kes |
|--------|---------|-----|
| POST | `/api/auth/register` | kõik |
| POST | `/api/auth/login` | kõik |
| GET | `/api/animals?species=&status=` | kõik (`[AllowAnonymous]`) |
| GET | `/api/animals/{id}` | kõik |
| POST / PUT / DELETE | `/api/animals` | Admin, Staff |
| POST | `/api/animals/{id}/applications` | Visitor |
