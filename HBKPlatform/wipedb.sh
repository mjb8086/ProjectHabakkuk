#!/bin/bash

psql -U devlogin -d HBKPlatform << EOF

delete from client_messages;
delete from clinic_homepages;
delete from clients;
delete from practitioners;
delete from clinics;
delete from user_roles;
delete from users;
delete from user_claims;
delete from appointments;
delete from client_records;
delete from timeslot_availabilities;
delete from timeslots;

EOF


