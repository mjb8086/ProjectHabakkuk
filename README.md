H B K :: P L A T F O R M
========================

All-in-one practice management from NowDoctor Ltd.

This is an aborted attempt to produce a clinical management system for independent practitioners. It features appointment booking, availability management, instant messaging, record keeping and contact management.

There is no sign-up.

There is an admin section called the "Master Control Panel" or MCP.

The application runs as a monolith. It's written entirely in CSHTML (Razor) but some progress was made on the replacement Vue UI (I think on its own branch). It has some bugs, it's far from complete, and it was ultimately scrapped in favour of a clener Vue + .NET API separation that became our launch platform. That version remains closed source.

This version doesn't reflect how I'd develop a .NET application, or any application now for that matter. But it is functional and self contained - you can build it and run it, but it does require PostgreSQL.

For one, I'd separate the Dtos and Repositories into their own solutions. The DB is initalised with TPC which causes problems we fixed in the other version. I'd also do it entirely in Vue. Many other things too, but I'm just throwing this up for anyone who wants to play with it.

## Building on Linux or MacOS.

See [Wiki](http://10.8.0.1/wiki/HBKPlatform#Building) for up to date info.

# Environmnental Assumptions - Deployment

# Attribution
Please insert and update the template `attribution.tpl` at the top of any new Controllers, Repositories or modules. Please also type a brief description detailing the module's purpose. Also append your name to the list of authors if you have contributed to the source file. We believe that creators should get a chance to sign their work.

**Be kind, comment!**

# Preparing a production server
