<script setup>
import { ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';

const home = { icon: 'pi pi-home', to: '/' };
const items = ref([]);

const route = useRoute();
const router = useRouter();

const navigate = () => {
    router.push(home.to);
};

const watchRouter = () => {
    if (route.meta.breadcrumb) {
        items.value = [];
        const bc = route.meta.breadcrumb[0];
        for (let pro in bc) {
            items.value.push({ label: bc[pro] });
        }
    }
};

watch(
    route,
    () => {
        watchRouter();
    },
    { immediate: true }
);
</script>

<template>
    <div class="layout-breadcrumb-container">
        <nav class="layout-breadcrumb">
            <ol>
                <li>
                    <i :class="home.icon" @click="navigate"></i>
                </li>
                <li><i class="pi pi-angle-right"></i></li>
                <template v-for="(item, index) in items" :key="index">
                    <li>
                        <span> {{ item.label }}</span>
                    </li>
                    <li v-if="index !== items.length - 1">
                        <i class="pi pi-angle-right"></i>
                    </li>
                </template>
            </ol>
        </nav>
    </div>
</template>
