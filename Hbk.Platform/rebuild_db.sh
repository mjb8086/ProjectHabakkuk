#!/bin/sh
CTX="Hbk.Database.ApplicationDbContext"
PRO="../Hbk.Database"
    dotnet ef database drop --context $CTX --project $PRO -f && 
    dotnet ef migrations remove --context $CTX --project $PRO && 
    dotnet ef migrations add init --context $CTX --project $PRO && 
    dotnet ef database update --context $CTX --project $PRO
