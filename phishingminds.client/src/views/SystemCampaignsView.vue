<script setup>
import { ref, onMounted } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'

const campaigns = ref([])
const loading = ref(true)
const userEmpresaId = ref(null)

const getToken = () => localStorage.getItem('token')

onMounted(async () => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    userEmpresaId.value = user.idEmpresa

    try {
      loading.value = true
      const res = await fetch(`/api/Campanha/empresa/${userEmpresaId.value}`, {
        headers: { 'Authorization': `Bearer ${getToken()}` }
      })
      if (res.ok) {
        campaigns.value = await res.json()
      }
    } catch (e) {
      console.error('Erro ao buscar campanhas:', e)
    } finally {
      loading.value = false
    }
  }
})

const formatDate = (dateStr) => {
  if (!dateStr) return '--'
  const date = new Date(dateStr)
  return date.toLocaleDateString('pt-BR') + ' ' + date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
}
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

    <!-- Campaigns List -->
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
        <div v-if="loading" class="text-center py-8 text-gray-500 font-medium">
          Carregando campanhas...
        </div>
        <div v-else-if="campaigns.length === 0" class="text-center py-8 text-gray-500">
          Nenhuma campanha encontrada.
        </div>
        <div v-else class="space-y-4">
          <!-- Campaign Item -->
          <div v-for="c in campaigns" :key="c.idCampaign" class="flex flex-col md:flex-row md:items-center justify-between p-4 border border-gray-100 rounded-2xl hover:bg-gray-50 transition-colors cursor-pointer gap-4">
            <div class="flex items-center gap-4">
              <div class="w-12 h-12 bg-green-100 text-green-700 rounded-xl flex items-center justify-center font-bold flex-shrink-0">
                C{{ c.idCampaign }}
              </div>
              <div>
                <h4 class="font-semibold text-gray-800">{{ c.nomeCampaign || c.nomeCampanha }}</h4>
                <p class="text-sm text-gray-500">Disparo em: {{ formatDate(c.dt_Disparo) }} - Alvo: {{ c.nm_Setor || 'Geral' }}</p>
              </div>
            </div>
            
            <div class="flex gap-6 text-sm text-center md:justify-end items-center">
              <div>
                <span class="bg-blue-50 text-blue-700 text-xs px-3 py-1.5 rounded-full font-bold uppercase tracking-wider">
                  {{ c.status }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>
