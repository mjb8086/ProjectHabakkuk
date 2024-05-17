<script setup>
defineProps({
  title: String,
  value: Number,
  icon: String
})
</script>

<template>
  <div class="stat-item">
    <span class="pi entry-icon" :class="icon"></span>  
    <div>
      <div>{{value}} {{title}}</div>
    </div>
  </div>
</template>

<style scoped>
div.stat-item {
  padding: 3px 3px 3px 3px;
  border-radius: 5px;
  margin: 5px 5px 5px 2px;
  background-color: var(--surface-50);
  display: flex;
  align-items: center;
}

span.entry-icon {
  width: 30px;
  text-align: center;
  color: var(--blue-200);
}
</style>