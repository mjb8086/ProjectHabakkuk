<script setup>
defineProps({
  name: String,
  clientId: Number,
  unreadMessageCount: Number,
  img: String
})
</script>

<template>
  <div class="convo-entry">
    <Avatar v-bind:image="img" class="mr-2" size="small" shape="circle" />
    <div>
      <div>{{name}} has <Badge v-bind:value="unreadMessageCount" class="badge-sml"/> unread.</div>
      <div><a class="text-sm underline">Open conversation.</a></div>
    </div>
  </div>
</template>

<style scoped>
div.convo-entry {
  padding: 3px 3px 3px 8px;
  border-radius: 5px;
  margin: 5px 5px 5px 2px;
  background-color: var(--surface-50);
  display: flex;
  align-items: center;
}
</style>