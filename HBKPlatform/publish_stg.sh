#!env bash
set -e

SSH_PORT=22
PUB="./bin/Release/net6.0/publish/"
HOST="mci-stg1"

echo "1. BUILDING RELEASE CONFIGURATION >>>>>"
dotnet publish --configuration Release
echo "2. STOPPING DAEMON ON REMOTE SERVER >>>>>>"
ssh $HOST systemctl stop mci-platform

# Todo: Copy DB if does not exist. Set DB permissions.

echo "3. RSYNC IT >>>>>"
rsync -prvc --delete -e "ssh -p $SSH_PORT" "$PUB" $HOST:/var/www/mci-platform
echo "4. START IT >>>>"
ssh $HOST systemctl start mci-platform
