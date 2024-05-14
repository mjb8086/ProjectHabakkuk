<script setup>
defineProps({
  dateString: String,
  timeString: String,
  clientName: String,
  treatmentTitle: String,
  roomDetails: String
})
</script>

<template>
<div class="appointment-lite-item">
  <details>
    <summary>
      <span class="date-time">{{dateString}}</span> at
      <span class="date-time">{{timeString}}</span> -
      <span class="client-name">{{clientName}}</span>
    </summary>
    <span>{{treatmentTitle}}</span>
    <div v-if="roomDetails">{{roomDetails}}</div>
  </details>
</div>
</template>

<style scoped>
div.appointment-lite-item {
  max-width: 300px;
  margin: 2px;
}
div.appointment-lite-item summary {
  border-bottom: 1px solid var(--surface-200);
}
span.date-time {
  font-weight: 550;
}
</style>