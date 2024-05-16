<script setup>
import GenericPanel from "@/common/components/GenericPanel.vue";
import StatisticItem from "@/mynd/components/Reception/Controls/StatisticItem.vue";

defineProps({
  numClientsRegistered: Number,
  numAppointmentsCompleted: Number
});
</script>

<template>
  <GenericPanel>
    <template #title>Statistics</template>
    <template #data>
       <StatisticItem v-bind="{title: 'Completed Appointments', value: numAppointmentsCompleted, icon: 'pi-calendar'}"></StatisticItem>
      <StatisticItem v-bind="{title: 'Registered Clients', value: numClientsRegistered, icon: 'pi-address-book'}"></StatisticItem>
    </template>
  </GenericPanel>

</template>

<style>
</style>