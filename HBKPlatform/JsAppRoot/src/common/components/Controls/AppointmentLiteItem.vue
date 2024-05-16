<script setup>
import DateUtils from '@/common/lib/dateutils.js';

defineProps({
  dateTime: String,
  clientName: String,
  treatmentTitle: String,
  roomDetails: String
});
</script>

<template>
<div class="appointment-lite-item">
  <details>
    <summary>
      <span class="date-time">{{DateUtils.getFormattedDateTime(dateTime)}}</span>
      <div class="client-name">{{clientName}}</div>
    </summary>
    <div class="item-body">
      <span>{{treatmentTitle}}</span>
      <div v-if="roomDetails">{{roomDetails}}</div>
    </div>
  </details>
</div>
</template>

<style scoped>
div.item-body, div.client-name {
  margin-left: 14px;
}
div.appointment-lite-item {
  margin: 2px 2px 6px 2px;
  padding: 1px 1px 1px 1px;
  border-radius: 5px;
  background-color: var(--surface-50);
}
div.appointment-lite-item details[open] > summary {
}
span.date-time {
  font-weight: 550;
}
</style>