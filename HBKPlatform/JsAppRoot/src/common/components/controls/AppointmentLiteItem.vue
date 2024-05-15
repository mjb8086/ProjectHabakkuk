<script setup>
defineProps({
  dateTime: String,
  clientName: String,
  treatmentTitle: String,
  roomDetails: String
})
</script>

<template>
<div class="appointment-lite-item">
  <details>
    <summary>
      <span class="date-time">{{dateTime}}</span>
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
}
div.appointment-lite-item details[open] > summary {
  border-bottom: 1px solid var(--surface-200);
}
span.date-time {
  font-weight: 550;
}
</style>