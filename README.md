# Project Habakkuk

![Hbk Logo](./assets/HbkLogo1.png)

Clinical practice management application - C# / ASP.NET Core

Project Habakkuk is an open-source clinical practice management application built with ASP.NET Core.

It began as an early prototype of a product for independent healthcare practitioners and predates the later UbiClinic marketplace. The project is now used as a demonstration application for modern .NET architecture and development practices.

## Features

- Role-based workflows for practitioners, clients, clinic managers, and platform administrators.
- Practitioner and client management, including practitioner-client relationships and contact details.
- Appointment booking, treatment management, practitioner availability, and scheduling.
- Clinical records, priority items, and client-practitioner messaging.
- Clinic room management, practitioner room reservations, and approval workflows.
- ASP.NET Core Identity authentication with role-based authorization.
- Server-rendered Razor UI backed by Entity Framework Core, with SQLite, PostgreSQL, and in-memory database options.
- Experimental Vue 3 and Vite frontend work in `Hbk.Platform/JsAppRoot`.

## Current Architecture

The solution is currently organised into focused projects:

- `Hbk.Platform` is the ASP.NET Core host, containing Razor views, MVC controllers, role-specific Areas, repositories, and application services.
- `Hbk.Database` contains the EF Core `ApplicationDbContext`, persistence entities, migrations, and development seed data.
- `Hbk.Models` contains DTOs and view models shared across the application.
- `Hbk.Common` contains common helpers and cross-cutting services.
- `Hbk.Test` contains the unit test suite.

The application uses ASP.NET Core Identity for authentication and routes signed-in users to the appropriate role-specific area. Development uses a seeded, disposable SQLite database by default, while PostgreSQL and EF Core's in-memory provider remain configurable alternatives.

## Run Locally

### Prerequisites

- .NET 8 SDK
- PostgreSQL only when using the PostgreSQL provider
- Node.js 18+ only when working on the experimental Vue frontend

### Start the application

The Development configuration uses a seeded SQLite database by default, so no external database setup is required for a first run.

```bash
dotnet restore
dotnet run --project Hbk.Platform
```

The default SQLite file is created under the operating system's temporary directory. It is intentionally disposable: the application recreates and seeds it whenever the hosting environment removes it.

### Database providers

Select a provider with `Database:Provider`. In Azure App Service or another hosted environment, use the equivalent `Database__Provider` environment variable.

| Provider | Configuration value | Intended use |
| --- | --- | --- |
| SQLite | `Sqlite` | Default local and public-demo option. Supports relational EF Core operations without an external database. |
| PostgreSQL | `PostgreSql` | Persistent relational deployment. Requires `ConnectionStrings:HbkContext`, or `ConnectionStrings__HbkContext` as an environment variable. PostgreSQL migrations are applied at startup. |
| In-memory | `InMemory` | Lightweight fallback for development and tests. Data is lost when the process stops, and relational-only operations such as `ExecuteUpdateAsync` are unavailable. |

SQLite uses `ConnectionStrings:HbkSqlite` when configured (`ConnectionStrings__HbkSqlite` in Azure). If it is omitted, the application creates `hbk-demo.db` in its temporary directory. For example:

```json
{
  "Database": {
    "Provider": "Sqlite"
  },
  "ConnectionStrings": {
    "HbkSqlite": "Data Source=/tmp/project-habakkuk.db"
  }
}
```

To use the in-memory fallback locally, set `Database:Provider` to `InMemory`. Its optional `Database:InMemoryDatabaseName` setting defaults to `HbkInMemory`.

### Run the tests

```bash
dotnet test Hbk.Test/Hbk.Test.csproj -m:1 --no-restore
```

### Sample users

The default seed includes the following accounts for local exploration:

| Email | Password | Role |
| --- | --- | --- |
| `mjb+sudo1@nowdoctor.co.uk` | `changeme123` | SuperAdmin |
| `drwallace@lawrencestreetpractice.com` | `trustmeiamadoctor` | Practitioner |
| `another@hillvalley.com` | `trustmeiamadoctor` | Practitioner |
| `edward@fsmail.net` | `eddie_metal` | Client |
| `laura@hotmail.com` | `ihatemanure` | Client |
| `wolseley@btinternet.com` | `vip_pass_mode` | ClinicManager |
| `mrg@sphigh.com` | `misterslave` | Client |
| `les@primusville.com` | `johnthefisherman` | Practitioner |

Only SuperAdmin users can currently register new practitioners and clinics on behalf of users.

## Current Status

The original application was built while exploring the product concept. I have since been modernising it as a public demonstration application, applying the architectural approaches I use in current .NET development.

The application currently follows an anemic domain model. A migration to Clean Architecture is in progress on the `CleanArchMigration` branch.

## History
Development of this platform took place from 2023-2025, when the room booking aspect was forked to become UbiClinic.

The platform was open-sourced in February 2026, given a facelift and presented to the world.
