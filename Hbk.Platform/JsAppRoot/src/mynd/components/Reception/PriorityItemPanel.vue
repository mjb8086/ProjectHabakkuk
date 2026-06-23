<script setup>
import GenericPanel from "@/common/components/GenericPanel.vue";
import PriorityItem from "@/mynd/components/Reception/Controls/PriorityItem.vue";

defineProps({
  priorityItems: Array
});
</script>

<template>
  <GenericPanel>
    <template #title>Priority Items</template>
    <template #data>
      <div v-for="item in priorityItems">
       <PriorityItem v-bind="item"></PriorityItem>
      </div>      
    </template>
  </GenericPanel>

</template>

<style>
span.entry-icon {
  width: 30px;
  text-align: center;
  color: var(--red-400);
}
</style>