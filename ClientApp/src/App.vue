<script setup>
import { RouterView, RouterLink } from 'vue-router'
</script>

<template>
  <div class="min-h-screen bg-gray-100">
    <nav class="bg-gray-800 text-white p-4 flex gap-4 overflow-x-auto">
      <RouterLink to="/admin" class="hover:underline">Admin</RouterLink>
      <RouterLink to="/waiter" class="hover:underline">Waiter</RouterLink>
      <RouterLink to="/kds" class="hover:underline">KDS</RouterLink>
    </nav>
    
    <main>
      <RouterView />
    </main>
  </div>
</template>

<style>
/* Base styles if needed */
</style>
