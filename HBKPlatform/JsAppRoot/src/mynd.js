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
import { createApp } from 'vue';
import {createRouter, createWebHistory} from "vue-router";
import PrimeVue from 'primevue/config';

import Consts from '@/common/lib/consts.js';
import HomeView from "@/mynd/views/HomeView.vue";
import MyNDMain from '@/MyNDMain.vue';

// TODO: Decide whether any of the theme should be imported in the cshtml, what are the tradeoffs
import '@/common/assets/theme/theme-light/denim/theme.css'; // other options exist, explore the colours
// Import global style including PrimeVue/Poseidon
//import '@/common/assets/main.css'; // vue default style - remove when appropriate
import '@/common/assets/styles.scss';

// Assign Logo to be used by common AppTopbar
Consts.APP_LOGO = Consts.LOGO_MYND;

const myNd = createApp(MyNDMain);

myNd.use(PrimeVue, { ripple: true });

myNd.use( createRouter({
        history: createWebHistory(Consts.MYND_BASE_URL),
        routes: [
            {
                path: '/',
                name: 'home',
                component: HomeView
            },
            {
                path: '/about',
                name: 'about',
                // route level code-splitting this generates a separate chunk (About.[hash].js) for this route
                // which is lazy-loaded when the route is visited.
                component: () => import('@/mynd/views/AboutView.vue')
            },
            {
                path: '/sample/:id',
                name: 'sample',
                component: () => import('@/mynd/views/Sample.vue')
            }
        ]
    })
);

// Inject it into the template that was served by Asp.Net
myNd.mount('#app');
