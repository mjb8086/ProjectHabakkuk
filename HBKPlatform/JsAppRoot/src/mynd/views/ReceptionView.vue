<script setup>
import {ref} from 'vue';
import {API_BASE} from "@/common/lib/consts.js";
import AppointmentLiteItem from '@/common/components/controls/AppointmentLiteItem.vue';

const summaryData = ref({summaryData:{}});

fetch(`${API_BASE}/api/mynd/getreceptionsummary`)
    .then((res) => res.json())
    .then((json) => (summaryData.value = json));
//    .catch((err) => (error = err));
</script>

<template>
  <main>
    <div class="flex justify-content-between flex-wrap">
      <Card class="align-items-stretch">
        <template #title>
          Upcoming Appointments
        </template>
        <template #content>
          <ul v-for="appt in summaryData.upcomingAppointments" key="id">
            <AppointmentLiteItem v-bind="appt"></AppointmentLiteItem>
          </ul>
        </template>
      </Card>

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Recent Cancellations
          <ul v-for="appt in summaryData.recentCancellations" key="id">
            <AppointmentLiteItem v-bind="appt"></AppointmentLiteItem>
          </ul>
        </div>
      </div>

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Unread Messages
        </div>
      </div>

      <div class="col-12 md:col-12 xl:col-3">
        <div class="overview-card surface-card py-3 px-4 shadow-1 border-round-md h-full">
          Current Room Reservations
          <ul v-for="res in summaryData.roomReservations" key="id">
            {{res.roomTitle}} - {{res.when}}
          </ul>
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
