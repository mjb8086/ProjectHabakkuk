<script setup>
import GenericPanel from "./GenericPanel.vue";
import AppointmentLiteItem from "@/common/components/controls/AppointmentLiteItem.vue";

defineProps({
  appointmentData: Array,
});
</script>

<template>
  <GenericPanel>
    <template #title>
      <slot name="title" />
    </template>
    <template #data>
      <slot>
        <div v-if="appointmentData" v-for="appt in appointmentData" key="id">
          <AppointmentLiteItem v-bind="appt"></AppointmentLiteItem>
        </div>
        <p v-else>None yet.</p>
      </slot>
    </template> 
    <template #bottom>
      <slot name="bottom" />
    </template>
  </GenericPanel>
</template>

<style>
/* Custom options depending on appointment data category */
div.upcoming h1.generic-panel-title {
  color: var(--teal-800);
}
div.upcoming summary::marker { /* inside AppointmentLiteItem */
  color: var(--teal-600);
}

div.cancellations h1.generic-panel-title {
  color: var(--red-800);
}
div.cancellations summary::marker {
  color: var(--red-600);
}
</style>