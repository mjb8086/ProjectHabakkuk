<script setup>
import {ref} from 'vue';
import {API_BASE} from "@/common/lib/consts.js";
import AppointmentPanel from '@/common/components/AppointmentPanel.vue';
import UnreadMessagePanel from "@/mynd/components/Reception/UnreadMessagePanel.vue";
import RoomReservationPanel from "@/mynd/components/Reception/RoomReservationPanel.vue";
import PriorityItemPanel from "@/mynd/components/Reception/PriorityItemPanel.vue";
import StatisticsPanel from "@/mynd/components/Reception/StatisticsPanel.vue";

const summaryData = ref({summaryData:{}});

fetch(`${API_BASE}/api/mynd/getreceptionsummary`)
    .then((res) => res.json())
    .then((json) => (summaryData.value = json));
//    .catch((err) => (error = err));
</script>

<template>
  <main>
    <div class="flex  flex-wrap ">
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

      <UnreadMessagePanel v-bind:unreadMessageDetails="summaryData.unreadMessageDetails">
      </UnreadMessagePanel>

      <RoomReservationPanel v-bind:currentReservations="summaryData.roomReservations">
      </RoomReservationPanel>

      <PriorityItemPanel v-bind:priorityItems="summaryData.priorityItems">
      </PriorityItemPanel>

      <StatisticsPanel v-bind="summaryData" />
    </div>
  </main>
</template>
