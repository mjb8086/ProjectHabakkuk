<script setup>
import {ref} from "vue";
defineProps({
  earningsData: Object
});

const chartOptions = ref({
  scales: {y: {ticks: {callback: function(value, index, ticks) {return `£${value}`;}}}}
});
</script>

<template>
  <div class="generic-panel-root col-6 md:col-12 xl:col-6">
    <div class="generic-panel-inner surface-card py-3 px-4 shadow-3 border-round-md h-full flex flex-column">
      <h1 class="generic-panel-title">
        Weekly Income
      </h1>
      <div class="generic-data ">
        <Chart type="line" :data="earningsData" :options="chartOptions" class="h-30rem" style="max-height:320px" />
      </div>
    </div>
  </div>
</template>

<style scoped>
div.generic-panel-root {
  max-height: 400px;
}
h1.generic-panel-title {
  font-size: 1.1em;
  border-bottom: 1px solid var(--surface-400);
  padding-bottom: 2px;
  margin-bottom: 0px;
}
</style>