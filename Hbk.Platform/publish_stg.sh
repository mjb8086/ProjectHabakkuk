#!env bash
set -e

SSH_PORT=22
PUB="./bin/Release/net8.0/publish/"
HOST="hbk-stg1"

echo "1. BUILDING RELEASE CONFIGURATION >>>>>"
dotnet publish --configuration Release
echo "2. STOPPING DAEMON ON REMOTE SERVER >>>>>>"
ssh $HOST systemctl stop hbk-platform

# Todo: Copy DB if does not exist. Set DB permissions.

echo "3. RSYNC IT >>>>>"
rsync -prvc --delete -e "ssh -p $SSH_PORT" "$PUB" $HOST:/var/www/hbk-platform
echo "4. START IT >>>>"
ssh $HOST systemctl start hbk-platform
