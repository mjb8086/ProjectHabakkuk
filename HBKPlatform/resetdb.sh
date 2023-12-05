#!/bin/sh
dotnet ef database drop && dotnet ef migrations remove && dotnet ef migrations add init && dotnet ef database update && sqlite3 mci.db < insertspecs.sql
