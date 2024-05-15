<script setup>
import AppointmentLiteItem from "@/common/components/controls/AppointmentLiteItem.vue";

defineProps({
  appointmentData: Array,
});
</script>

<template>
  <div class="appointment-panel-root col-12 md:col-12 xl:col-3">
    <div class="appointment-panel-inner surface-card py-3 px-4 shadow-3 border-round-md h-full flex flex-column">
      <h1 class="appointment-panel-title">
        <slot name="title" />
      </h1>
      <div class="appointment-data flex-grow-1">
        <slot>
          <div v-if="appointmentData.length > 0" v-for="appt in appointmentData" key="id">
            <AppointmentLiteItem v-bind="appt"></AppointmentLiteItem>
          </div>
          <p v-else>None yet.</p>
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
  height: 400px;
}
div.appointment-panel-inner {
}
div.appointment-data {
  overflow-y: scroll;
}
footer.appointment-data-footer {
  height: 22px;
  border-top: 1px solid var(--surface-400);
  display: flex;
  justify-content: space-between;
}

h1.appointment-panel-title {
  font-size: 1.1em;
  border-bottom: 1px solid var(--surface-400);
  padding-bottom: 2px;
  margin-bottom: 0px;
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