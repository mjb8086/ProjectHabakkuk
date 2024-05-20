/*
 *  MyNd UI Boot
 *  ---------------
 *  Author: Mark Brown
 *  Authored: 26/04/24
 * 
 *  Configure and start the myND UI here after login hands it off to Vue.Js
 *  © 2024 NowDoctor Ltd
 */

import 'vite/modulepreload-polyfill';
import { nextTick, createApp } from 'vue';
import { createRouter, createWebHistory } from "vue-router";
import PrimeVue from 'primevue/config';

import Consts from '@/common/lib/consts.js';
import {CurrentUserData} from '@/common/lib/globals.js';
import ReceptionView from "@/mynd/views/ReceptionView.vue";
import MyNDMain from '@/MyNDMain.vue';

// TODO: Decide whether any of the theme should be imported in the cshtml, what are the tradeoffs
import '@/common/assets/theme/theme-light/denim/theme.css'; // other options exist, explore the colours
// Import global style including PrimeVue/Poseidon
//import '@/common/assets/main.css'; // vue default style - remove when appropriate
import '@/common/assets/styles.scss';
import {getCurrentUserData} from "@/common/lib/api/user.js";

// Assign Logo to be used by common AppTopbar
Consts.APP_LOGO = Consts.LOGO_MYND;

// Fetch current user data
CurrentUserData.value = await getCurrentUserData();

const myNd = createApp(MyNDMain);

myNd.use(PrimeVue, { ripple: true });

// Routing. Don't forget to append the new routes to MyNDMenu if appropriate.
const router = createRouter({
        history: createWebHistory(Consts.MYND_BASE_PATH),
        routes: [
            // RECEPTION
            {
                path: '/',
                name: 'reception',
                component: ReceptionView,
                meta: {
                    breadcrumb: [{label: 'Reception'}],
                    title: 'Reception'
                }
            },
            // APPOINTMENTS
            {
                path: '/appointments/overview',
                name: 'appointments-overview',
                // route level code-splitting, this generates a separate chunk (About.[hash].js) for this route
                // which is lazy-loaded when the route is visited.
                component: () => import('@/mynd/views/Appointments/Overview.vue'),
                meta: {
                    breadcrumb: [{parent: 'Appointments', label: 'Overview'}],
                    title: 'Appointments'
                }
            },
            {
                path: '/appointments/booking',
                name: 'appointments-booking',
                component: () => import('@/mynd/views/Appointments/Booking.vue'),
                meta: {
                    breadcrumb: [{parent: 'Appointments', label: 'Booking'}],
                    title: 'Appointment Booking'
                }
            },
            {
                path: '/appointments/availability-management',
                name: 'availability-management',
                component: () => import('@/mynd/views/Appointments/AvailabilityManagement.vue'),
                meta: {
                    breadcrumb: [{parent: 'Appointments', label: 'Availability Management'}],
                    title: 'Availability Management'
                }
            },
            {
                path: '/appointments/treatment-management',
                name: 'treatment-management',
                component: () => import('@/mynd/views/Appointments/TreatmentManagement.vue'),
                meta: {
                    breadcrumb: [{parent: 'Appointments', label: 'Treatment Management'}],
                    title: 'Treatment Management'
                }
            },
            // RESERVATIONS
            {
                path: '/reservations/my-reservations',
                name: 'my-reservations',
                component: () => import('@/mynd/views/Reservations/MyReservations.vue'),
                meta: {
                    breadcrumb: [{parent: 'Reservations', label: 'My Reservations'}],
                    title: 'My Reservations'
                }
            },
            {
                path: '/reservations/make',
                name: 'make-reservation',
                component: () => import('@/mynd/views/Reservations/MakeAReservation.vue'),
                meta: {
                    breadcrumb: [{parent: 'Reservations', label: 'Make A Reservation'}],
                    title: 'Make a Reservation'
                }
            },
            // CLIENT DATA
            {
                path: '/client-data/records',
                name: 'client-records',
                component: () => import('@/mynd/views/ClientData/RecordKeeping.vue'),
                meta: {
                    breadcrumb: [{parent: 'Client Data', label: 'Records'}],
                    title: 'Client Records'
                }
            },
            {
                path: '/client-data/contacts',
                name: 'client-contacts',
                component: () => import('@/mynd/views/ClientData/ContactDetails.vue'),
                meta: {
                    breadcrumb: [{parent: 'Client Data', label: 'Contacts'}],
                    title: 'Client Contacts'
                }
            },
            // MESSAGING
            {
                path: '/messaging/inbox',
                name: 'messaging-inbox',
                component: () => import('@/mynd/views/Messaging/Inbox.vue'),
                meta: {
                    breadcrumb: [{parent: 'Messaging', label: 'Inbox'}],
                    title: 'Inbox'
                }
            },
            {
                path: '/messaging/archive',
                name: 'messaging-archive',
                component: () => import('@/mynd/views/Messaging/Archive.vue'),
                meta: {
                    breadcrumb: [{parent: 'Messaging', label: 'Archive'}],
                    title: 'Archive'
                }
            },
            // CONFIGURATION
            {
                path: '/configuration/system',
                name: 'system-configuration',
                component: () => import('@/mynd/views/Configuration/SysConfig.vue'),
                meta: {
                    breadcrumb: [{parent: 'Configuration', label: 'System'}],
                    title: 'System Configuration'
                }
            },
            {
                path: '/configuration/import-export',
                name: 'import-export',
                component: () => import('@/mynd/views/Configuration/ImportExport.vue'),
                meta: {
                    breadcrumb: [{parent: 'Configuration', label: 'Import/Export'}],
                    title: 'Import/Export'
                }
            },
            {
                path: '/configuration/personal-details',
                name: 'my-personal-details',
                component: () => import('@/mynd/views/Configuration/MyPersonalDetails.vue'),
                meta: {
                    breadcrumb: [{parent: 'Configuration', label: 'My Personal Details'}],
                    title: 'My Personal Details'
                }
            },
        ]
    });

router.afterEach((to, from) => {
    // Use next tick to handle router history correctly
    nextTick(() => {
        document.title = `${Consts.MYND_BASE_TITLE} ${to.meta.title}`;
    });
});

myNd.use(router);

console.log('HBKPlatform New UI booting. MyND mode.');

// Inject it into the template that was served by Asp.Net
myNd.mount('#app');
