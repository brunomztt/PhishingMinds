<script setup>
import SecurityChart from './SecurityChart.vue'

defineProps({
  evolucao: {
    type: Array,
    default: () => []
  }
})
</script>

<template>
  <div class="bg-white rounded-3xl shadow-sm p-8 w-full h-[430px]">
    <h3 class="text-3xl font-bold text-green-900 mb-2">
      Evolução de Segurança
    </h3>

    <p class="text-gray-400 mb-8">
      Evolução por campanha
    </p>

    <SecurityChart
      :evolucao="evolucao"
    />
    <pre>{{  evolucao }} </pre>
  </div>
</template>