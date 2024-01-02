#!/bin/sh
dotnet ef database drop -f && dotnet ef migrations remove && dotnet ef migrations add init && dotnet ef database update
