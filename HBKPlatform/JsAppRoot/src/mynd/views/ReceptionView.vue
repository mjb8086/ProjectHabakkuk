<script setup>
import {ref} from 'vue';
import {API_BASE} from "@/common/lib/consts.js";
import AppointmentPanel from '@/common/components/AppointmentPanel.vue';

const summaryData = ref({summaryData:{}});

fetch(`${API_BASE}/api/mynd/getreceptionsummary`)
    .then((res) => res.json())
    .then((json) => (summaryData.value = json));
//    .catch((err) => (error = err));
</script>

<template>
  <main>
    <div class="flex justify-content-between flex-wrap">
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

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Unread Messages
        </div>
      </div>

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Current Room Reservations
          <div v-for="res in summaryData.roomReservations" key="id">
            {{res.roomTitle}} - {{res.when}}
          </div>
        </div>
      </div>

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Priority Items
          <ul v-for="record in summaryData.priorityItems" key="id">
            {{record.title}} {{record.displayDate}} {{record.clientName}}
          </ul>
        </div>
      </div>

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Statistics
          <div>Registered Clients: {{summaryData.numClientsRegistered}}</div>
          <div>Completed Appointments: {{summaryData.numAppointmentsCompleted}}</div>
        </div>
      </div>
    </div>
  </main>
</template>
