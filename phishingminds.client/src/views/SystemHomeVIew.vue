<script setup>
import MainLayout from '../layouts/MainLayout.vue'
import InfoCards from '../components/home/InfoCards.vue'
import MainPlaceholder from '../components/home/MainPlaceholder.vue'
import QuickAccess from '../components/home/QuickAccess.vue'
</script>

<template>
  <MainLayout>
    <div class="mb-6">
      <h2 class="text-3xl md:text-4xl font-bold text-green-900">Overview da Segurança</h2>
      <p class="text-gray-500 mt-1">Monitoramento em tempo real de vulnerabilidades e métricas.</p>
    </div>

    <InfoCards />

    <section class="mt-4 flex flex-col lg:flex-row gap-4 items-stretch">
      <div class="flex-[2.3] w-full">
        <MainPlaceholder />
      </div>

      <div class="flex-1 w-full lg:w-auto">
        <QuickAccess />
      </div>
    </section>
  </MainLayout>
</template>