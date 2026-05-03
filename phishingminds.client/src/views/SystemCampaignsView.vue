<script setup>
import MainLayout from '../layouts/MainLayout.vue'
</script>

<template>
  <MainLayout>
    <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-3xl md:text-4xl font-bold text-green-900">Gestão de Campanhas</h2>
        <p class="text-gray-500 mt-1">Crie, agende e monitore suas simulações de phishing.</p>
      </div>
      <button class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors">
        Nova Campanha
      </button>
    </div>

    <!-- Campaigns List Placeholder -->
    <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
      <div class="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
        <h3 class="text-xl font-semibold text-gray-800">Campanhas Recentes</h3>
        <select class="bg-white border border-gray-200 rounded-lg px-3 py-1.5 text-sm outline-none">
          <option>Todas as campanhas</option>
          <option>Ativas</option>
          <option>Concluídas</option>
        </select>
      </div>
      
      <div class="p-6">
        <div class="space-y-4">
          <!-- Campaign Item -->
          <div v-for="i in 3" :key="i" class="flex flex-col md:flex-row md:items-center justify-between p-4 border border-gray-100 rounded-2xl hover:bg-gray-50 transition-colors cursor-pointer gap-4">
            <div class="flex items-center gap-4">
              <div class="w-12 h-12 bg-green-100 text-green-700 rounded-xl flex items-center justify-center font-bold flex-shrink-0">
                C{{ i }}
              </div>
              <div>
                <h4 class="font-semibold text-gray-800">Simulação de Segurança Financeira #{{ i }}</h4>
                <p class="text-sm text-gray-500">Agendado para: 15/10/2026 - Alvo: Financeiro</p>
              </div>
            </div>
            
            <div class="flex gap-6 text-sm text-center md:justify-end">
              <div>
                <div class="font-bold text-gray-800">78%</div>
                <div class="text-gray-400 text-xs">Abertura</div>
              </div>
              <div>
                <div class="font-bold text-yellow-600">12%</div>
                <div class="text-gray-400 text-xs">Cliques</div>
              </div>
              <div>
                <div class="font-bold text-red-600">4%</div>
                <div class="text-gray-400 text-xs">Vítimas</div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>
