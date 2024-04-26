<template>
  <div class="about">
    <h1>This is a sample page showing useful info.</h1>
      <div>
        Id is this {{ $route.params.id }}
      </div>
  </div>
</template>

<style>
@media (min-width: 1024px) {
  .about {
    min-height: 100vh;
    display: flex;
    align-items: center;
  }
}
</style>
