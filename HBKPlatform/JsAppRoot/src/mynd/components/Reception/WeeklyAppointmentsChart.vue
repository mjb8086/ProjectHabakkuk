<script setup>
import {ref, shallowRef} from "vue";
import GenericLineGraphPanel from '@/common/components/GenericLineGraphPanel.vue';

import { chartDataSource } from "@/mynd/components/Reception/State/chart-data-source.js";

const chartData = ref({
  datasets: [
    {
      label: 'Appointments',
      data: chartDataSource.weeklyAppointmentsChartData,
      borderColor: ['#e5bb90'],
      backgroundColor: ['rgba(40, 136, 76, .05)'],
      borderWidth: 2,
      fill: true,
    },
  ],
});

const chartOptions = shallowRef({
  scales: { y: { precision: 0, ticks: { stepSize: 1.0 }}},
  plugins: { legend: { display: false }}
});

</script>
<template>
  <GenericLineGraphPanel>
    <template #title>
       Appointments Per Week
    </template>
    <template #chartArea>
        <Chart type="line" :data="chartData" :options="chartOptions" class="h-30rem" style="max-height:320px" />
    </template>
  </GenericLineGraphPanel>
</template>