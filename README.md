H B K :: P L A T F O R M
========================

All-in-one practice management from NowDoctor Ltd.

## Building on Linux.

Install Dotnet:

https://learn.microsoft.com/en-gb/dotnet/core/install/linux-debian

Install Efcore:

```
dotnet tool install --global dotnet-ef
```

Update PATH:

```
PATH="$PATH:~/.dotnet/tools"
```

Update DB:

```
dotnet-ef database update && sqlite3 mci.db < insertspecs.sql
```

Run it:

```
$ dotnet run
```

# Environmnental Assumptions - Deployment

# Development

# Attribution
Please insert and update the template `attribution.tpl` at the top of any new Controllers, Repositories or modules. Please also type a brief description detailing the module's purpose. Also append your name to the list of authors if you have contributed to the source file. We believe that creators should get a chance to sign their work.

**Be kind, comment!**

# Preparing a production server
