<script setup>
import DateUtils from "@/common/lib/dateutils.js";
defineProps({
  roomTitle: String,
  buildingName: String,
  when: String
})
</script>

<template>
  <div class="room-res-entry">
    <span class="pi pi-warehouse entry-icon"></span>  
    <div>
      <div>{{roomTitle}}</div>
      <div v-if="buildingName">{{buildingName}}</div>
      <div class="text-sm">{{DateUtils.getFriendlyDateTime(when)}}</div>
    </div>
  </div>
</template>

<style scoped>
div.room-res-entry {
  padding: 3px 3px 3px 3px;
  border-radius: 5px;
  margin: 5px 5px 5px 2px;
  background-color: var(--surface-50);
  display: flex;
  align-items: center;
}

span.entry-icon {
  width: 30px;
  text-align: center;
  color: var(--green-400);
}
</style>