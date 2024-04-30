/*
 *  MyNd UI Boot
 *  ---------------
 *  Author: Mark Brown
 *  Authored: 26/04/24
 * 
 *  Configure and start the myND UI here after login hands it off to Vue.Js
 *  © 2024 NowDoctor Ltd
 */
import './assets/main.css';

import 'vite/modulepreload-polyfill';
import { createApp } from 'vue';
import {createRouter, createWebHistory} from "vue-router";
import PrimeVue from 'primevue/config';

import Consts from './lib/common/consts.js';
import HomeView from "@/views/HomeView.vue";
import MyNDMain from './MyNDMain.vue';

// Import global style including PrimeVue/Poseidon
import '@/assets/styles.scss';

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
                component: () => import('./views/AboutView.vue')
            },
            {
                path: '/sample/:id',
                name: 'sample',
                component: () => import('./views/Sample.vue')
            }
        ]
    })
);

// Inject it into the template that was served by Asp.Net
myNd.mount('#app');
