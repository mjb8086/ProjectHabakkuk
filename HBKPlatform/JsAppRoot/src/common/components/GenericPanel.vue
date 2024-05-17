<script setup>

</script>

<template>
  <div class="generic-panel-root col-3 md:col-6 xl:col-3">
    <div class="generic-panel-inner surface-card py-3 px-4 shadow-3 border-round-md h-full flex flex-column">
      <h1 class="generic-panel-title">
        <slot name="title" />
      </h1>
      <div class="generic-data flex-grow-1">
        <slot name="data">
        </slot>
      </div>
      <footer class="generic-data-footer text-sm">
        <slot name="bottom" />
      </footer>
    </div>
  </div>
</template>

<style scoped>
div.generic-panel-root {
  height: 400px;
}
div.generic-panel-inner {
}
div.generic-data {
  overflow-y: scroll;
}
footer.generic-data-footer {
  height: 22px;
  border-top: 1px solid var(--surface-400);
  display: flex;
  justify-content: space-between;
}

h1.generic-panel-title {
  font-size: 1.1em;
  border-bottom: 1px solid var(--surface-400);
  padding-bottom: 2px;
  margin-bottom: 0px;
}

</style>