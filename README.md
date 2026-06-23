H B K :: P L A T F O R M
========================

All-in-one practice management from NowDoctor Ltd. (what was to be). I'm just throwing this up for anyone who wants to play with it.

This is an aborted attempt to produce a clinical management system for independent practitioners. It features appointment booking, availability management, instant messaging, record keeping and contact management.

You can login as each user type - see SeedNFeed.cs for credentials. There is no registration supported.

We use 'Areas' to group sections - i.e. There is an admin section called the "Master Control Panel" or MCP in the Areas/MCP directory. This keeps templates and controllers together.

The core application is under `Hbk.Platform`. It's written entirely in CSHTML (Razor) but some progress was made on the replacement Vue UI. It has plenty of bugs, it's far from complete, and it was ultimately scrapped in favour of a clener Vue + .NET API separation that became our launch platform. That version remains closed source.

This version is rough. I repeat, you can't register for the site. The styling was never updated from its static page placeholder style. It violates several good OO design principles - you can hunt for them if you're bored. Consequently it is not I'd develop a .NET application now. But it is functional and self contained - you can build it and run it, it requires only access to PostgreSQL to seed the DB.

## Fixes made since it was scrapped
I've recently separated the DTOs, models, and DB, and Common into their own projects. The DB is no longer initalised with TPC - this caused id sequence problems.

## Finishing it
I'd finish the new UI it entirely in Vue. Lots of other steps too.

## Building on Linux or MacOS.
See [Wiki](http://10.8.0.1/wiki/HBKPlatform#Building) for up to date info (lol)

# Environmnental Assumptions - Deployment
Weep, for God will not help you here.

## Preparing a production server
We had one working, but bollocks if I know how to set it up again.
