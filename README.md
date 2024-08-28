# Todo App MVC

Jednostavna Todo aplikacija s korisničkom autentikacijom, izrađena u ASP.NET Core MVC i SQL Lite.

## Uvod

Ovo je Todo aplikacija izrađena korištenjem ASP.NET Core MVC arhitekture. Projekt je razvijen kao dio procesa učenja C# i MVC frameworka, bez prethodnog iskustva s ovim tehnologijama.

## Iskustvo

- **Prethodno iskustvo**: C++ (osnove), TypeScript, JavaScript, SQL (MSSQL, IBM DB2, Postgres), React, Node, Next.js
- **Novo naučeno za ovaj projekt**: C#, ASP.NET Core MVC, Razor views

## Tehnologije

- .NET 8
- ASP.NET Core MVC
- Entity Framework Core
- SQL Lite (u razvoju se koristio MSSQL, kasnije prebačeno na Lite)
- Google OAuth za autentikaciju

### Preduvjeti

- .NET Core SDK (verzija 8)
- SQL Server: SQL Lite
- Visual Studio 2019 ili noviji (preporučeno v2022)

## Google OAuth postavke

- Potrebni su za Google OAuth Client ID i Client Secret, koji se mogu registrirati na adresi: https://console.cloud.google.com/apis/credentials

## Funkcionalnosti

- Početni ekran s opcijama za prijavu, registraciju i Google autentikaciju
- CRUD operacije za Todo stavke
- Osnovno filtriranje po statusu
- Custom Error i Not-found stranice

## Struktura projekta

- `Controllers/`: Sadrži MVC kontrolere
- `Models/`: Definicije modela podataka
- `Views/`: Razor view templates
- `Data/`: Konfiguracija baze podataka
- `Migrations/`: Sadrži migracijske file-ove
- `Services/`: Sadrži servise za postavljanje veze na bazu i autentifikaciju

## Poznata ograničenja

- Projekt je razvijen u svrhu učenja, moguće je da sam odstupio od best practices
- Razor views sintaksa može biti suboptimalna zbog nedostatka prethodnog iskustva
- CSS datoteka nije optimalno strukturirana

## Budući planovi

- Poboljšanje korisničkog sučelja
- Implementacija dodatnih funkcionalnosti (filtriranje, paginacija, editiranje, korisnički profil, itd.)
- Optimizacija koda i strukture projekta

## Kontakt

- **Email**: vjekoslav.vacenovski@gmail.com

