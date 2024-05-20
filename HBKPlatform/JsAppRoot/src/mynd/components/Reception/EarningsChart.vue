<script setup>
import { ref, shallowRef } from "vue";
import GenericLineGraphPanel from '@/common/components/GenericLineGraphPanel.vue';
import { chartDataSource } from "@/mynd/components/Reception/State/chart-data-source.js";

const earningsData = ref({
  datasets: [
    {
      label: 'Earnings',
      data: chartDataSource.earningsData,
      borderColor: ['#4e904e'],
      backgroundColor: ['rgba(40, 136, 76, .05)'],
      borderWidth: 2,
      fill: true,
    },
  ],
});

const chartOptions = shallowRef({
  scales: { y: { ticks: { callback: function(value, index, ticks) { return `£${value}`; }}}},
  plugins: { legend: { display: false }}
});
</script>

<template>
  <GenericLineGraphPanel>
    <template #title>
        Monthly Income
    </template>
    <template #chartArea>
        <Chart type="line" :data="earningsData" :options="chartOptions" class="h-30rem" style="max-height:320px" />
    </template>
  </GenericLineGraphPanel>
</template>