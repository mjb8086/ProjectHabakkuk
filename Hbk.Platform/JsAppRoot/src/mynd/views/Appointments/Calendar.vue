<script setup>
import {ref} from "vue";
import FullCalendar from '@fullcalendar/vue3';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';

const events = ref({});
const options = ref({
  plugins: [dayGridPlugin, timeGridPlugin, interactionPlugin],
  initialDate: '2022-05-11',
  height: 720,
  headerToolbar: {
    left: 'prev,next today',
    center: 'title',
    right: 'dayGridMonth,timeGridWeek,timeGridDay'
  },
  editable: true,
  selectable: true,
  selectMirror: true,
  dayMaxEvents: true,
//  eventClick: (e) => onEventClick(e),
//  select: (e) => onDateSelect(e)
});

</script>
<template>
  <main class="about">
    <h1>Appointment Calendar</h1>
    <FullCalendar :events="events" :options="options" />
    <p>WIP</p>
  </main>
</template>

<style>
</style>
