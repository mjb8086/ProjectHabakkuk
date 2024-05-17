<script setup>
import {ref} from 'vue';
import {API_BASE} from "@/common/lib/consts.js";

import AppointmentPanel from '@/common/components/AppointmentPanel.vue';
import UnreadMessagePanel from "@/mynd/components/Reception/UnreadMessagePanel.vue";
import RoomReservationPanel from "@/mynd/components/Reception/RoomReservationPanel.vue";
import PriorityItemPanel from "@/mynd/components/Reception/PriorityItemPanel.vue";
import StatisticsPanel from "@/mynd/components/Reception/StatisticsPanel.vue";
import EarningsChart from "@/mynd/components/Reception/EarningsChart.vue";
import WeeklyAppointmentsChart from "@/mynd/components/Reception/WeeklyAppointmentsChart.vue";
import AvailabiltyPanel from "@/mynd/components/Reception/AvailabilityPanel.vue";

const summaryData = ref({weeklyAppointmentsChartData: []});

fetch(`${API_BASE}/api/mynd/getreceptionsummary`)
    .then((res) => res.json())
    .then((json) => (summaryData.value = json));
//    .catch((err) => (error = err));

const sampleEarningsData = ref([ { x: 'May 2024', y : 1310 }, { x: 'June 2024', y: 2830 }, { x: 'July 2024', y: 1692}, { x: 'August 2024', y: 1580}, { x: 'September 2024', y: 2980}  ]);

</script>

<template>
  <main>
    <div class="grid flex flex-wrap">
      <AppointmentPanel class="upcoming" v-bind:appointmentData="summaryData.upcomingAppointments">
        <template #title>
          Upcoming Appointments
        </template>
        <template #bottom>
          <span v-if="summaryData.additionalUpcoming">with {{summaryData.additionalUpcoming}} more.</span> <span><router-link :to="{ name: 'appointments-overview' }" class="underline">Open Appointments.</router-link></span>
        </template>
      </AppointmentPanel>

      <AppointmentPanel class="cancellations" v-bind:appointmentData="summaryData.recentCancellations">
        <template #title>
          Recent Cancellations
        </template>
        <template #bottom>
          <span v-if="summaryData.additionalCancellations">with {{summaryData.additionalCancellations}} more.</span>
        </template>
      </AppointmentPanel>

      <UnreadMessagePanel v-bind:unreadMessageDetails="summaryData.unreadMessageDetails" />
      <RoomReservationPanel v-bind:currentReservations="summaryData.roomReservations" />
      <EarningsChart v-bind:data="sampleEarningsData" />
      <PriorityItemPanel v-bind:priorityItems="summaryData.priorityItems" />
      <StatisticsPanel v-bind="summaryData" />
      <WeeklyAppointmentsChart v-bind:data="summaryData.weeklyAppointmentsChartData" />
      <p>{{summaryData.weeklyAppointmentsChartData}}</p>
      <AvailabiltyPanel />
    </div>
  </main>
</template>
