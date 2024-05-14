<script setup>
import AppointmentLiteItem from "@/common/components/controls/AppointmentLiteItem.vue";

defineProps({
  appointmentData: Array,
});
</script>

<template>
  <div class="appointment-panel-root col-12 md:col-12 xl:col-3">
    <div class="overview-card surface-card py-3 px-4 shadow-3 border-round-md h-full flex flex-column">
      <h1 class="appointment-panel-title">
        <slot name="title" />
      </h1>
      <div class="appointment-data">
        <slot>
          <div v-for="appt in appointmentData" key="id">
            <AppointmentLiteItem v-bind="appt"></AppointmentLiteItem>
          </div>
        </slot>
      </div>
      <footer class="appointment-data-footer text-sm">
        <slot name="bottom" />
      </footer>
    </div>
  </div>
</template>

<style>
div.appointment-panel-root {
  min-height: 300px; 
}
div.appointment-data {
  min-height: 200px;
}
footer.appointment-data-footer {
  border-top: 1px solid var(--surface-400);
  display: flex;
  justify-content: space-between;
}

h1.appointment-panel-title {
  font-size: 1.2em;
  border-bottom: 1px solid var(--surface-400);
  padding-bottom: 2px;
  margin-bottom: 2px;
}

/* Custom options depending on appointment data category */
div.upcoming h1.appointment-panel-title {
  color: var(--teal-800);
}
div.upcoming summary::marker { /* inside AppointmentLiteItem */
  color: var(--teal-600);
}

div.cancellations h1.appointment-panel-title {
  color: var(--red-800);
}
div.cancellations summary::marker {
  color: var(--red-600);
}
</style>