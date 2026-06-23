<script setup>
import GenericPanel from "@/common/components/GenericPanel.vue";
import RoomReservationItem from "@/mynd/components/Reception/Controls/RoomReservationItem.vue";

defineProps({
  currentReservations: Array
});
</script>

<template>
  <GenericPanel>
    <template #title>Current Room Reservations</template>
    <template #data>
      <div v-for="res in currentReservations">
       <RoomReservationItem v-bind="res"></RoomReservationItem>
      </div>      
    </template>
  </GenericPanel>

</template>

<style>
</style>