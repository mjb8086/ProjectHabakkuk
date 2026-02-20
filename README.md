H B K :: P L A T F O R M
========================

All-in-one practice management from NowDoctor Ltd. (what was to be)

This is an aborted attempt to produce a clinical management system for independent practitioners. It features appointment booking, availability management, instant messaging, record keeping and contact management.

There is no sign-up.

We use 'Areas' to group sections - i.e. There is an admin section called the "Master Control Panel" or MCP in the Areas/MCP directory. This keeps templates and controllers together.

The application runs as a monolith. It's written entirely in CSHTML (Razor) but some progress was made on the replacement Vue UI. It has plenty of bugs, it's far from complete, and it was ultimately scrapped in favour of a clener Vue + .NET API separation that became our launch platform. That version remains closed source.

This version is rough. You can't register for the site. The styling was never updated from its placeholder style. It's not I'd develop a .NET application now, or any application now for that matter. But it is functional and self contained - you can build it and run it, it requires only PostgreSQL.

## Improvements to be made
I'd separate the DTOs and Repositories into their own solutions. The DB is initalised with TPC which causes id sequence problems we fixed in the other version. I'd also do it entirely in Vue. Many other things too, but I'm just throwing this up for anyone who wants to play with it.

## Building on Linux or MacOS.
See [Wiki](http://10.8.0.1/wiki/HBKPlatform#Building) for up to date info (lol)

# Environmnental Assumptions - Deployment
Weep, for God will not help you here.

# Attribution
Please insert and update the template `attribution.tpl` at the top of any new Controllers, Repositories or modules. Please also type a brief description detailing the module's purpose. Also append your name to the list of authors if you have contributed to the source file. We believe that creators should get a chance to sign their work.

**Be kind, comment!**

# Preparing a production server
We had one working, but bollocks if I know how to set it up again.
