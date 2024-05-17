<script setup>
import {reactive, ref} from "vue";
import GenericLineGraphPanel from '@/common/components/GenericLineGraphPanel.vue';

const props = defineProps({
  data : Array
});

const dataRef = ref(props.data);

const chartData = ref({
  datasets: [
    {
      label: 'Appointments',
      data: dataRef.value,
      borderColor: ['#e5bb90'],
      backgroundColor: ['rgba(40, 136, 76, .05)'],
      borderWidth: 2,
      fill: true,
    },
  ],
});

const chartOptions = ref({
  plugins: {legend: {display:false}}
});

</script>
<template>
  <GenericLineGraphPanel>
    <template #title>
       Appointments Per Week
      <p>Chart Data: {{JSON.stringify(chartData)}}</p>
      <p>Data: {{data}}</p>
    </template>
    <template #chartArea>
        <Chart type="line" :data="chartData" :options="chartOptions" class="h-30rem" style="max-height:320px" />
    </template>
  </GenericLineGraphPanel>
</template>