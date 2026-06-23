<template>
  <div class="generic-line-graph-root col-12 md:col-12 xl:col-6">
    <div class="generic-line-graph-inner surface-card py-3 px-4 shadow-3 border-round-md h-full flex flex-column">
      <h1 class="generic-line-graph-title">
        <slot name="title"></slot>
      </h1>
      <div class="chart-area">
        <slot name="chartArea"></slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
div.generic-line-graph-root {
  max-height: 400px;
}
h1.generic-line-graph-title {
  font-size: 1.1em;
  border-bottom: 1px solid var(--surface-400);
  padding-bottom: 2px;
  margin-bottom: 0px;
}
div.chart-area {
  margin-top: 10px;
}
</style>