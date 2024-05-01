<script setup>
import { ref } from 'vue';
import AppMenuItem from '@/common/components/layout/AppMenuItem.vue';

const model = ref([
    {
        label: 'Favorites',
        icon: 'pi pi-fw pi-home',
        items: [{ label: 'Reception', icon: 'pi pi-fw pi-home', to: '/', colour: '#caf5ff' }]
    },
    {
        label: 'Appointments',
        icon: 'pi pi-fw pi-th-large',
        items: [
            {
                label: 'Overview',
                icon: 'pi pi-fw pi-calendar',
                to: '/appointments/overview',
                colour: '#ffbdbd'
            },
            {
                label: 'Booking',
                icon: 'pi pi-fw pi-calendar-plus',
                to: '/appointments/booking',
                colour: '#ffd483'
            },
            {
                label: 'Availability Management',
                icon: 'pi pi-fw pi-calendar-clock',
                to: '/appointments/availability',
                colour: '#b9f6c2'
            },
            {
                label: 'Treatment Management',
                icon: 'pi pi-fw pi-money-bill',
                to: '/appointments/treatments',
                colour: '#7973f6'
            }]
    },
    {
      label: 'Room Reservation',
      icon: 'pi pi-fw pi-th-large',
      items: [
        {
          label: 'My Reservations',
          icon: 'pi pi-fw pi-warehouse',
          to: '/room-reservation/overview',
          colour: '#b0fbba'
        },
        {
          label: 'Make A Reservation',
          icon: 'pi pi-fw pi-ticket',
          to: '/room-reservation/make',
          colour: '#bab0fb'
        }]
    },
    {
      label: 'Client Data',
      icon: 'pi pi-fw pi-th-large',
      items: [
        {
          label: 'Record Keeping',
          icon: 'pi pi-fw pi-pen-to-square',
          to: '/client-data/records',
          colour: '#e5bb90'
        },
        {
          label: 'Contact Details',
          icon: 'pi pi-fw pi-address-book',
          to: '/client-data/contacts',
          colour: '#f2e8ff'
        }]
    },
    {
      label: 'Messaging',
      icon: 'pi pi-fw pi-th-large',
      items: [
        {
          label: 'Inbox',
          icon: 'pi pi-fw pi-inbox',
          to: '/messaging/inbox',
          colour: '#caffb6'
        },
        {
          label: 'Archive',
          icon: 'pi pi-fw pi-folder',
          to: '/messaging/archive',
          colour: '#ffd77d'
        }]
    },
    {
      label: 'Configuration',
      icon: 'pi pi-fw pi-th-large',
      items: [
        {
          label: 'System Configuration',
          icon: 'pi pi-fw pi-cog',
          to: '/configuration/system',
          colour: '#5b84c8'
        },
        {
          label: 'My Personal Details',
          icon: 'pi pi-fw pi-user',
          to: '/configuration/personal',
          colour: '#d2a7fd'
        },
        {
          label: 'Import/Export',
          icon: 'pi pi-fw pi-upload',
          to: '/configuration/import-export',
          colour: '#8bc98b'
        }]
    }
]);
</script>

<template>
    <ul class="layout-menu">
        <template v-for="(item, i) in model" :key="item">
            <AppMenuItem :item="item" root :index="i" />

            <li class="menu-separator"></li>
        </template>
    </ul>
</template>

<style lang="scss" scoped></style>
