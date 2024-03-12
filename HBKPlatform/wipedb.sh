#!/bin/bash

psql -U devlogin -d HBKPlatform << EOF

delete from client_messages;
delete from clinic_homepages;
delete from client_records;
delete from timeslot_availabilities;
delete from timeslots;
delete from clients;
delete from clinics;
delete from practitioners;
delete from user_roles;
delete from users;
delete from user_claims;
delete from appointments;
delete from tenancies;
delete from attributes;
delete from room_attributes;
delete from rooms;

EOF


