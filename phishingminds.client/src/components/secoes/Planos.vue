<template>
  <section id="secao-planos" class="bg-[#2D4A38] text-white py-24 rounded-t-[4rem] md:rounded-t-[8rem]">

    <!-- HEADER -->
    <div class="text-center mb-16 px-6">
      <h2 class="text-4xl md:text-5xl font-bold mb-4">
        Investimento <span class="italic font-serif font-medium">Transparente</span>
      </h2>
      <p class="text-gray-300 max-w-2xl mx-auto text-sm md:text-base">
        Escolha o plano ideal para blindar sua empresa. Sem taxas ocultas, apenas segurança de ponta.
      </p>
    </div>

    <!-- LOADING -->
    <div v-if="loading"
         class="text-center text-green-200 py-10 flex flex-col items-center">

      <svg class="animate-spin h-8 w-8 mb-4 text-green-400"
           xmlns="http://www.w3.org/2000/svg"
           fill="none"
           viewBox="0 0 24 24">
        <circle class="opacity-25"
                cx="12"
                cy="12"
                r="10"
                stroke="currentColor"
                stroke-width="4" />
        <path class="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373
                 0 0 5.373 0 12h4zm2
                 5.291A7.962 7.962
                 0 014 12H0c0 3.042
                 1.135 5.824 3
                 7.938l3-2.647z" />
      </svg>

      Buscando planos...
    </div>

    <!-- EMPTY STATE -->
    <div v-else-if="planos.length === 0"
         class="max-w-2xl mx-auto text-center border-2 border-dashed border-[#4a6355] rounded-2xl p-10 bg-[#3a5244]/50">

      <h3 class="text-xl font-bold mb-2">Novos planos em breve!</h3>
      <p class="text-gray-300">
        Estamos preparando as melhores ofertas de segurança para você. Volte mais tarde.
      </p>
    </div>

    <!-- GRID -->
    <div v-else
         class="max-w-6xl mx-auto px-6 md:px-10 grid gap-8 lg:gap-6 items-stretch"
         :class="gridClass">

      <PlanoCard v-for="(plano, index) in planos"
                 :key="plano.IdPlano"
                 :plano="plano"
                 :index="index"
                 :total="planos.length" />

    </div>
  </section>
</template>

<script setup>
  import { ref, onMounted, computed } from 'vue'
  import PlanoCard from './PlanoCard.vue'

  const planos = ref([])
  const loading = ref(true)

  const gridClass = computed(() => {
    if (planos.value.length === 1) {
      return 'grid-cols-1 max-w-md mx-auto'
    }
    if (planos.value.length === 2) {
      return 'grid-cols-1 md:grid-cols-2 max-w-4xl mx-auto'
    }
    return 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3'
  })

  const fetchPlanos = async () => {
    try {
      const response = await fetch('/api/plano')

      if (!response.ok) {
        throw new Error('Erro ao buscar planos')
      }

      planos.value = await response.json()
    } catch (error) {
      console.error('Erro:', error)
      planos.value = []
    } finally {
      loading.value = false
    }
  }

  onMounted(fetchPlanos)
</script>
