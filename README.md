# Project Habakkuk

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
- Server-rendered Razor UI, backed by Entity Framework Core and PostgreSQL or an in-memory development database.
- Experimental Vue 3 and Vite frontend work in `Hbk.Platform/JsAppRoot`.

## Current Architecture

The solution is currently organised into focused projects:

- `Hbk.Platform` is the ASP.NET Core host, containing Razor views, MVC controllers, role-specific Areas, repositories, and application services.
- `Hbk.Database` contains the EF Core `ApplicationDbContext`, persistence entities, migrations, and development seed data.
- `Hbk.Models` contains DTOs and view models shared across the application.
- `Hbk.Common` contains common helpers and cross-cutting services.
- `Hbk.Test` contains the unit test suite.

The application uses ASP.NET Core Identity for authentication and routes signed-in users to the appropriate role-specific area. In Development, it can run against a seeded EF Core in-memory database; PostgreSQL is available when persistent, relational storage is required.

## Run Locally

### Prerequisites

- .NET 8 SDK
- PostgreSQL only when running with the relational provider
- Node.js 18+ only when working on the experimental Vue frontend

### Start the application

The Development configuration uses the seeded in-memory database by default, so no database setup is required for a first run.

```bash
dotnet restore
dotnet run --project Hbk.Platform
```

The in-memory database is recreated when the application stops. To use PostgreSQL, set `Database:UseInMemory` to `false` in `Hbk.Platform/appsettings.Development.json` and configure `ConnectionStrings:HbkContext`.

Some workflows use relational EF Core operations and therefore require PostgreSQL rather than the in-memory provider.

### Run the tests

```bash
dotnet test Hbk.Test/HBK.Test.csproj -m:1 --no-restore
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
