<template>
  <div :class="cardClass">

    <!-- BADGE -->
    <div v-if="isHighlight"
         class="absolute -top-4 left-1/2 -translate-x-1/2 bg-[#a9ceb6] text-[#1f3025] text-xs font-bold px-4 py-1.5 rounded-full uppercase tracking-wider">
      Destaque
    </div>

    <!-- NOME -->
    <h3 class="text-sm font-bold tracking-widest mb-2 uppercase"
        :class="isHighlight ? 'text-gray-500' : 'text-green-200'">
      {{ plano.nm_Plano }}
    </h3>

    <!-- PREÇO -->
    <div class="flex items-baseline gap-1 mb-4">
      <span class="text-4xl md:text-5xl font-bold"
            :class="isHighlight ? 'md:text-6xl' : ''">
        R$ {{ formatPrice(plano.value_Plano) }}
      </span>
      <span class="text-sm"
            :class="isHighlight ? 'text-gray-500' : 'text-gray-300'">
        /mês
      </span>
    </div>

    <!-- DESC -->
    <p class="text-[13px] mb-8 border-b pb-6 min-h-[60px]"
       :class="isHighlight ? 'text-gray-500 border-gray-200' : 'text-gray-300 border-[#4a6355]'">
      {{ plano.desc_Plano }}
    </p>

    <!-- FEATURES -->
    <ul class="space-y-3 mb-10 flex-1 text-sm"
        :class="isHighlight ? 'text-gray-600' : 'text-gray-200'">
      <li>✔ {{ plano.maxUsers }} colaboradores</li>
      <li>✔ {{ plano.maxCampaigns }} campanhas</li>
      <li>✔ Relatórios detalhados</li>
    </ul>

    <!-- BTN -->
    <button @click="$router.push({ path: '/cadastro', query: { plano: plano.id_Plano } })" :class="buttonClass">
      Escolher {{ plano.nm_Plano }}
    </button>

  </div>
</template>

<script setup>
  import { computed } from 'vue'

  const props = defineProps({
    plano: Object,
    index: Number,
    total: Number
  })

  const isHighlight = computed(() =>
    props.index === 1 && props.total > 1
  )

  const cardClass = computed(() =>
    isHighlight.value
      ? 'relative bg-white text-[#2c4033] p-8 md:p-10 rounded-2xl border-[3px] border-[#a9ceb6] shadow-xl scale-105 flex flex-col'
      : 'bg-[#3a5244] border border-[#4a6355] p-8 rounded-2xl text-white flex flex-col'
  )

  const buttonClass = computed(() =>
    isHighlight.value
      ? 'w-full bg-[#2c4033] text-white py-3 rounded-lg font-bold hover:bg-[#1f3025]'
      : 'w-full bg-white text-[#2c4033] py-3 rounded-lg font-bold hover:bg-gray-100'
  )


  // 🔥 evita quebrar se vier undefined/null
  const formatPrice = (value) => {
    if (!value) return '0,00'
    return value.toLocaleString('pt-BR', { minimumFractionDigits: 2 })
  }
</script>
