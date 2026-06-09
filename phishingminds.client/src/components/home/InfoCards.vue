<script setup>
defineProps({
  metrics: {
    type: Object,
    required: true
  }
})
</script>

<template>
  <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">

    <div class="bg-white rounded-2xl shadow-sm p-5 h-28 flex flex-col justify-center">
      <p class="text-gray-500 text-sm mb-2">Campanhas Ativas</p>
      <h3 class="text-3xl font-bold text-green-900">{{ metrics.activeCampaigns }}</h3>
    </div>

    <div class="bg-gradient-to-r from-green-700 to-green-900 rounded-2xl shadow-sm p-5 h-28 flex flex-col justify-center text-white">
      <p class="text-sm mb-2">Taxa de Falhas</p>
      <h3 class="text-3xl font-bold">{{ metrics.failureRate }}%</h3>
    </div>

    <div class="bg-white rounded-2xl shadow-sm p-5 h-28 flex flex-col justify-center">
      <p class="text-gray-500 text-sm mb-2">Usuários Treinados</p>
      <h3 class="text-3xl font-bold text-green-900">{{ metrics.trainedUsers }}</h3>
    </div>

    <div class="bg-white rounded-2xl shadow-sm p-5 h-28 flex flex-col justify-center">
      <p class="text-gray-500 text-sm mb-2">Evolução Média</p>
      <h3 class="text-3xl font-bold text-green-900">{{ metrics.avgEvolution }}%</h3>
    </div>

  </div>
</template>