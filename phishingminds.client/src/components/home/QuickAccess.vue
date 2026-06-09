<script setup>
defineProps({
  ranking: {
    type: Array,
    required: true
  }
})

const getBarColor = (score) => {
  if (score >= 80) return 'bg-green-400'
  if (score >= 50) return 'bg-yellow-400'
  return 'bg-red-500'
}
</script>

<template>
  <div class="bg-gradient-to-b from-green-800 to-green-900 rounded-3xl shadow-lg p-6 w-full h-[430px] text-white overflow-y-auto">
    <h3 class="text-2xl font-semibold mb-2">Ranking de setores</h3>
    <p class="text-sm text-green-100 mb-6">Pontuação de segurança por departamento</p>

    <div v-if="ranking.length === 0" class="text-center text-green-200 mt-12">
      Nenhum setor cadastrado para ranking.
    </div>

    <div v-else class="space-y-4">
      <div v-for="item in ranking.slice(0, 5)" :key="item.Setor">
        <div class="flex justify-between text-sm">
          <span>{{ item.Setor }}</span>
          <span class="font-bold text-xs bg-black/20 px-2 py-0.5 rounded">{{ Math.round(item.Score) }}%</span>
        </div>
        <div class="w-full h-3 bg-white/10 rounded-full mt-2 overflow-hidden">
          <div :class="[getBarColor(item.Score), 'h-full rounded-full transition-all duration-500']" :style="{ width: Math.max(0, Math.min(100, item.Score)) + '%' }"></div>
        </div>
      </div>
    </div>
  </div>
</template>