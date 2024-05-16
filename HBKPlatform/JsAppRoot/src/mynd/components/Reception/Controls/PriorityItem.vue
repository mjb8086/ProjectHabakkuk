<script setup>
import DateUtils from "@/common/lib/dateutils.js";
defineProps({
  title: String,
  date: String,
  clientName: String,
  id: Number
})
</script>

<template>
  <div class="priority-item">
    <span class="pi pi-exclamation-circle entry-icon"></span>  
    <div>
      <div>{{title}}</div>
      <div class="text-sm">{{clientName}}</div>
      <div class="text-sm">{{DateUtils.getFriendlyDateTime(date)}}</div>
    </div>
  </div>
</template>

<style scoped>
div.priority-item {
  padding: 3px 3px 3px 3px;
  border-radius: 5px;
  margin: 5px 5px 5px 2px;
  background-color: var(--surface-50);
  display: flex;
  align-items: center;
}
</style>