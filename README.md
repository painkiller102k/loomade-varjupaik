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
