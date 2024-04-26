/*
 *  Client UI Boot
 *  ---------------
 *  Author: Mark Brown
 *  Authored: 26/04/24
 * 
 *  Configure and start the client UI here after login hands it off to Vue.Js
 *  © 2024 NowDoctor Ltd
 */
import './assets/main.css';

import { createApp } from 'vue';
import { createRouter, createWebHistory } from 'vue-router';
import App from './App.vue';
import Consts from './lib/common/consts.js'
import 'vite/modulepreload-polyfill';
import HomeView from "@/views/HomeView.vue";

const app = createApp(App);

// Set routing.
app.use( createRouter({
        history: createWebHistory(Consts.CLIENT_BASE_URL),
        routes: [
            {
                path: '/',
                name: 'home',
                component: HomeView
            },
            {
                path: '/about',
                name: 'about',
                // route level code-splitting - this generates a separate chunk (About.[hash].js) for this route
                // which is lazy-loaded when the route is visited.
                component: () => import('/views/AboutView.vue')
            }
        ]
    })
);
    
app.mount('#app');
