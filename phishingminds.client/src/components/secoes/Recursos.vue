<template>
  <section id="secao-recursos" class="bg-[#F5F2EC] pt-32 pb-16 px-6 md:px-12 lg:px-[137px]">

    <!-- Header -->
    <div class="flex flex-col md:flex-row justify-between items-start md:items-end mb-16 gap-8">
      <div class="max-w-2xl">
        <div class="flex items-center gap-2 mb-4">
          <span class="text-[#A8D5B8] font-bold text-base">//</span>
          <p class="text-sm text-[#4A7C59] font-medium tracking-[0.35px] uppercase">Nossos Recursos</p>
        </div>
        <h2 class="text-4xl md:text-5xl font-bold text-[#2D4A38] leading-tight font-playfair">
          Tudo o que você precisa para <br class="hidden md:block" />
          <span class="text-[#6B9E7E]">elevar sua segurança</span>
        </h2>
      </div>

      <button @click="$router.push('/cadastro')"
              class="bg-[#4A7C59] hover:bg-[#3d684a] transition-colors text-white px-6 py-3 rounded-full text-base font-medium flex items-center gap-2">
        Ver todos
        <svg xmlns="http://www.w3.org/2000/svg"
             fill="none"
             viewBox="0 0 24 24"
             stroke-width="2"
             stroke="currentColor"
             class="w-4 h-4">
          <path stroke-linecap="round"
                stroke-linejoin="round"
                d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
        </svg>
      </button>
    </div>

    <!-- Grid -->
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">

      <!-- Card Principal -->
      <div class="bg-[#2D4A38] text-white rounded-[32px] p-8 md:p-10 relative overflow-hidden flex flex-col min-h-[360px]">
        <!-- Decorative Circle -->
        <div class="absolute top-0 right-0 w-32 h-32 bg-white opacity-10 rounded-bl-[100px]"></div>

        <!-- Icon -->
        <div class="w-14 h-14 bg-[#4A7C59] rounded-2xl flex items-center justify-center mb-10 z-10">
          <svg class="w-7 h-7 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
          </svg>
        </div>

        <h3 class="text-2xl font-semibold mb-3 z-10 text-white">Simulações Realistas</h3>
        <p class="text-base text-white/80 mb-auto z-10 leading-[26px]">
          Crie campanhas de phishing que imitam ataques reais para testar a prontidão da sua equipe.
        </p>

        <button @click="abrirModal(
    'Simulações Realistas',
    'Crie campanhas de phishing altamente personalizadas que simulam ataques reais. Avalie o comportamento dos colaboradores, identifique vulnerabilidades humanas e fortaleça a cultura de segurança da informação.'
  )"
                class="mt-8 flex items-center gap-2 text-[#A8D5B8] font-semibold text-sm z-10 hover:text-white transition-colors">
          Saiba mais
          <svg xmlns="http://www.w3.org/2000/svg"
               fill="none"
               viewBox="0 0 24 24"
               stroke-width="2"
               stroke="currentColor"
               class="w-4 h-4">
            <path stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
          </svg>
        </button>
      </div>

      <!-- Outros Cards -->
      <div class="bg-white rounded-[32px] p-8 md:p-10 border border-[#4A7C59]/10 hover:shadow-lg transition-shadow flex flex-col min-h-[360px]">
        <div class="w-14 h-14 bg-[#E8F0EB] rounded-2xl flex items-center justify-center mb-10">
          <svg class="w-7 h-7 text-[#4A7C59]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
          </svg>
        </div>
        <h3 class="text-[24px] font-semibold mb-3 text-[#2D4A38]">Análise Detalhada</h3>
        <p class="text-base text-[#4A5565] mb-auto leading-[26px]">
          Dashboards completos com métricas de desempenho e identificação de vulnerabilidades.
        </p>
        <button @click="abrirModal(
    'Análise Detalhada',
    'Acompanhe dashboards completos com métricas de desempenho, taxas de clique, evolução histórica e identificação dos usuários mais vulneráveis.'
  )"
                class="mt-8 flex items-center gap-2 text-[#4A7C59] font-semibold text-sm hover:text-[#2D4A38] transition-colors">
          Saiba mais
          <svg xmlns="http://www.w3.org/2000/svg"
               fill="none"
               viewBox="0 0 24 24"
               stroke-width="2"
               stroke="currentColor"
               class="w-4 h-4">
            <path stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
          </svg>
        </button>
      </div>

      <div class="bg-white rounded-[32px] p-8 md:p-10 border border-[#4A7C59]/10 hover:shadow-lg transition-shadow flex flex-col min-h-[360px]">
        <div class="w-14 h-14 bg-[#E8F0EB] rounded-2xl flex items-center justify-center mb-10">
          <svg class="w-7 h-7 text-[#4A7C59]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19.428 15.428a2 2 0 00-1.022-.547l-2.387-.477a6 6 0 00-3.86.517l-.318.158a6 6 0 01-3.86.517L6.05 15.21a2 2 0 00-1.806.547M8 4h8l-1 1v5.172a2 2 0 00.586 1.414l5 5c1.26 1.26.367 3.414-1.415 3.414H4.828c-1.782 0-2.674-2.154-1.414-3.414l5-5A2 2 0 009 10.172V5L8 4z" />
          </svg>
        </div>
        <h3 class="text-[24px] font-semibold mb-3 text-[#2D4A38]">Treinamento Automático</h3>
        <p class="text-base text-[#4A5565] mb-auto leading-[26px]">
          Colaboradores recebem treinamento imediato e personalizado após falharem em testes.
        </p>
        <button @click="abrirModal(
    'Treinamento Automático',
    'Usuários que falham em testes recebem treinamentos imediatos e personalizados, aumentando a conscientização e reduzindo riscos futuros.'
  )"
                class="mt-8 flex items-center gap-2 text-[#4A7C59] font-semibold text-sm hover:text-[#2D4A38] transition-colors">
          Saiba mais
          <svg xmlns="http://www.w3.org/2000/svg"
               fill="none"
               viewBox="0 0 24 24"
               stroke-width="2"
               stroke="currentColor"
               class="w-4 h-4">
            <path stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
          </svg>
        </button>
      </div>

      <div class="bg-white rounded-[32px] p-8 md:p-10 border border-[#4A7C59]/10 hover:shadow-lg transition-shadow flex flex-col min-h-[360px]">
        <div class="w-14 h-14 bg-[#E8F0EB] rounded-2xl flex items-center justify-center mb-10">
          <svg class="w-7 h-7 text-[#4A7C59]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
          </svg>
        </div>
        <h3 class="text-[24px] font-semibold mb-3 text-[#2D4A38]">Conformidade LGPD</h3>
        <p class="text-base text-[#4A5565] mb-auto leading-[26px]">
          Atenda aos requisitos de privacidade e segurança essenciais para as leis atuais.
        </p>
        <button @click="abrirModal(
    'Conformidade LGPD',
    'Auxilie sua empresa no cumprimento das exigências da LGPD por meio de programas contínuos de conscientização e redução de riscos.'
  )"
                class="mt-8 flex items-center gap-2 text-[#4A7C59] font-semibold text-sm hover:text-[#2D4A38] transition-colors">
          Saiba mais
          <svg xmlns="http://www.w3.org/2000/svg"
               fill="none"
               viewBox="0 0 24 24"
               stroke-width="2"
               stroke="currentColor"
               class="w-4 h-4">
            <path stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
          </svg>
        </button>
      </div>

      <div class="bg-white rounded-[32px] p-8 md:p-10 border border-[#4A7C59]/10 hover:shadow-lg transition-shadow flex flex-col min-h-[360px]">
        <div class="w-14 h-14 bg-[#E8F0EB] rounded-2xl flex items-center justify-center mb-10">
          <svg class="w-7 h-7 text-[#4A7C59]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" />
          </svg>
        </div>
        <h3 class="text-[24px] font-semibold mb-3 text-[#2D4A38]">Campanhas Agendadas</h3>
        <p class="text-base text-[#4A5565] mb-auto leading-[26px]">
          Programe testes periódicos para manter seu time sempre alerta e preparado.
        </p>
        <button @click="abrirModal(
    'Campanhas Agendadas',
    'Automatize campanhas de phishing simuladas em intervalos programados para manter o nível de atenção dos colaboradores ao longo do tempo.'
  )"
                class="mt-8 flex items-center gap-2 text-[#4A7C59] font-semibold text-sm hover:text-[#2D4A38] transition-colors">
          Saiba mais
          <svg xmlns="http://www.w3.org/2000/svg"
               fill="none"
               viewBox="0 0 24 24"
               stroke-width="2"
               stroke="currentColor"
               class="w-4 h-4">
            <path stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
          </svg>
        </button>
      </div>

      <div class="bg-white rounded-[32px] p-8 md:p-10 border border-[#4A7C59]/10 hover:shadow-lg transition-shadow flex flex-col min-h-[360px]">
        <div class="w-14 h-14 bg-[#E8F0EB] rounded-2xl flex items-center justify-center mb-10">
          <svg class="w-7 h-7 text-[#4A7C59]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197M13 7a4 4 0 11-8 0 4 4 0 018 0z" />
          </svg>
        </div>
        <h3 class="text-[24px] font-semibold mb-3 text-[#2D4A38]">Gestão de Equipes</h3>
        <p class="text-base text-[#4A5565] mb-auto leading-[26px]">
          Organize usuários por departamentos e aplique políticas específicas de teste.
        </p>
        <button @click="abrirModal(
    'Gestão de Equipes',
    'Organize colaboradores por departamentos, unidades ou grupos específicos e acompanhe métricas segmentadas para cada equipe.'
  )"
                class="mt-8 flex items-center gap-2 text-[#4A7C59] font-semibold text-sm hover:text-[#2D4A38] transition-colors">
          Saiba mais
          <svg xmlns="http://www.w3.org/2000/svg"
               fill="none"
               viewBox="0 0 24 24"
               stroke-width="2"
               stroke="currentColor"
               class="w-4 h-4">
            <path stroke-linecap="round"
                  stroke-linejoin="round"
                  d="M4.5 12h15m0 0l-6.75-6.75M19.5 12l-6.75 6.75" />
          </svg>
        </button>
      </div>

    </div>
  </section>

  <!-- Modal -->
  <div v-if="modalAberto"
       @click="fecharModal"
       class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4 overflow-y-auto">
    <div @click.stop
         class="bg-white rounded-[32px] p-8 max-w-2xl w-full shadow-2xl relative my-auto">
      <button @click="fecharModal"
              class="absolute top-4 right-6 text-3xl text-gray-400 hover:text-gray-700">
        ×
      </button>

      <h3 class="text-3xl font-semibold text-[#2D4A38] mb-6">
        {{ recursoSelecionado.titulo }}
      </h3>

      <p class="text-[#4A5565] leading-8 text-lg">
        {{ recursoSelecionado.descricao }}
      </p>
    </div>
  </div>
</template>

<script setup>
  import { ref } from 'vue'

  const modalAberto = ref(false)

  const recursoSelecionado = ref({
    titulo: '',
    descricao: ''
  })

  function abrirModal(titulo, descricao) {
    recursoSelecionado.value = {
      titulo,
      descricao
    }

    modalAberto.value = true
  }

  function fecharModal() {
    modalAberto.value = false
  }
</script>
