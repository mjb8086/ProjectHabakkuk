<script setup>
import GenericPanel from "@/common/components/GenericPanel.vue";
import UnreadMessageItem from "@/mynd/components/Reception/Controls/UnreadMessageItem.vue";

defineProps({
  unreadMessageDetails: Array
});
</script>

<template>
  <GenericPanel>
    <template #title>Unread Messages</template>
    <template #data>
      <div v-for="details in unreadMessageDetails">
       <UnreadMessageItem v-bind="details"></UnreadMessageItem>
      </div>      
    </template>
    
  </GenericPanel>

</template>

<style>
</style>