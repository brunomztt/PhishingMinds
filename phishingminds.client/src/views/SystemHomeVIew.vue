<template>
  <div class="min-h-screen bg-[#f5f3ef] flex gap-4 p-4">

    <SidebarHome />

    <main class="flex-1">
      <TopHome />
      <InfoCards />

      <section class="mt-4 flex gap-4 items-stretch">
        <div class="flex-[2.3]">
          <MainPlaceholder />
        </div>

        <div class="flex-1">
          <QuickAccess />
        </div>
      </section>
    </main>

  </div>
</template>

<script setup>
import SidebarHome from '../components/home/SidebarHome.vue'
import TopHome from '../components/home/TopHome.vue'
import InfoCards from '../components/home/InfoCards.vue'
import MainPlaceholder from '../components/home/MainPlaceholder.vue'
import QuickAccess from '../components/home/QuickAccess.vue'
</script>