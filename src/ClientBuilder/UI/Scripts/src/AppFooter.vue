<template>
  <footer class="footer">
    <div class="container">
      <p class="text">
        MIT License • Copyright © {{ now.getFullYear() }} <a href="https://georgi.karagogov.com/" target="_blank">Georgi Karagogov</a>
      </p>
    </div>
  </footer>
</template>

<script setup>
const now = new Date();
</script>

<style scoped>

</style>