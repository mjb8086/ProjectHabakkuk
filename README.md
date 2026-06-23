H B K :: P L A T F O R M
========================

All-in-one practice management from NowDoctor Ltd. (what was to be). 

This is an aborted attempt to produce a clinical management system for independent practitioners. It features appointment booking, availability management, instant messaging, record keeping and contact management.

Most of the work on this codebase was ended in 2024, when we pivoted to focus only on room booking. When we wound down NowDoctor (UbiClinic) in Feb 2026, I did some refactoring and patching of this older code just to throw this up on GH for anyone who wants to play with it.

The launch UbiClinic platform was forked from this in 2024. It remains closed source.

# Topology & Architecture
We use 'Areas' to group sections for each user role type. These comprise Practitoners, Clients, Clinics and SuperAdmins. E.g. The SuperAdmin section is called the "Master Control Panel" or MCP in the Areas/MCP directory. This keeps templates and controllers together.

The core application is under `Hbk.Platform`. It's written entirely in CSHTML (Razor) but some progress was made on the replacement Vue UI. It has plenty of bugs, it's far from complete, and it was ultimately scrapped in favour of a clener Vue + .NET API separation that became our launch platform. That version remains closed source.

This codebase is rough. You can't register for the site, but you can login (see below). The styling was never updated from its static page placeholder style. It violates several good OO design principles - you can hunt for these violations if you're bored. Consequently it is not how I'd develop a .NET application now. But it is functional and self contained - you can build it and run it, it by default uses an in-memory db. Should you wish to persist data, it requires access to PostgreSQL.

#### A note on the in-memory DB
It can be toggled on/of from `appsettings.Development.json`. I've left it as TRUE by default just to make this easy to run.

HOWEVER - certain methods, namely those using `ExecuteUpdateAsync` or `ExecuteDeleteAsync`, which are used in the instant messaging and room approval routes - these will throw an exception because these methods aren't supported for in-memory DBs. If you want to see the IMs, then you'll have to figure out how to set up PSQL.

# Fixes made since it was scrapped
In June 2026, I separated the DTOs, models, and DB, and Common into their own projects. I made a few other minor tweaks to make it slightly more usable. Also, the DB is no longer initalised with TPC - this caused id sequence problems.

# Sample Users
The default seed includes sample users:

| Email | Password | Role |
|---|---|---|
| `mjb+sudo1@nowdoctor.co.uk` | `changeme123` | SuperAdmin |
| `drwallace@lawrencestreetpractice.com` | `trustmeiamadoctor` | Practitioner |
| `another@hillvalley.com` | `trustmeiamadoctor` | Practitioner |
| `edward@fsmail.net` | `eddie_metal` | Client |
| `laura@hotmail.com` | `ihatemanure` | Client |
| `wolseley@btinternet.com` | `vip_pass_mode` | ClinicManager |
| `mrg@sphigh.com` | `misterslave` | Client |
| `les@primusville.com` | `johnthefisherman` | Practitioner |

Only SuperAdmin users can currently register new practitioners and clinics on behalf of users. Practitioners and clinics do not currently self-register directly.

### A brief explanation of the roles is in order:
- **SuperAdmin** Internal ND staff, can add/update new users of Prac and Clinic types. Can do lockout, password reset, cache clearing and view some autiting info.
- **Practitioner** A medical practitoner. He can book Clients into appointments, book rooms from Clinics, store notes, send and receive Instant Messages from clients, and view all client records.
- **Client** He can request bookings with his practitoner and send IMs to his practitioner.
- **ClinicManager** This user role can add rooms and set availability for Practioners to book into them. Also can approve/disapprove booking requests from Practitioners.

## Finishing it
I'd finish the new UI it entirely in Vue. Lots of other steps too. But tbh, I won't ever. Other things occupy my time now.

## Building on Linux or MacOS.
See [Wiki](http://10.8.0.1/wiki/HBKPlatform#Building) for up to date info (lol)

# Environmnental Assumptions - Deployment
Weep, for God will not help you here.

## Preparing a production server
We had one working, but bollocks if I know how to set it up again.
