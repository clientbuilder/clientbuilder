<template>
  <div class="page">
    <div class="container">
      <main>
        <AppNavbar/>
        <article class="content pb-5">
          <slot/>
        </article>
      </main>
      <AppFooter/>
    </div>
  </div>
</template>

<script setup>
import AppNavbar from "./AppNavbar.vue";
import AppFooter from "./AppFooter.vue";

</script>

<style scoped>

</style>