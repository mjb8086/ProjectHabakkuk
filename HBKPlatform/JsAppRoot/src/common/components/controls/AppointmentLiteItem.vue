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
<div>
  <span>{{dateString}}</span>
  <span>{{timeString}}</span>
  <span>{{clientName}}</span>
  <span>{{treatmentTitle}}</span>
  <div v-if="roomDetails">{{roomDetails}}</div>
</div>
</template>

<style scoped>

</style>