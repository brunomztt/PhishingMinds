<script setup>
import { ref, onMounted } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'
import InfoCards from '../components/home/InfoCards.vue'
import MainPlaceholder from '../components/home/MainPlaceholder.vue'
import QuickAccess from '../components/home/QuickAccess.vue'

const userEmpresaId = ref(null)
const loading = ref(true)
const metrics = ref({
  activeCampaigns: 0,
  trainedUsers: 0,
  failureRate: 0,
  avgEvolution: 100
})
const ranking = ref([])
const evolucao = ref([])
const selectedCampaign = ref(null)
const isCampaignModalOpen = ref(false)

const getToken = () => localStorage.getItem('token')

const openCampaignModal = (campaign) => {
  selectedCampaign.value = campaign
  isCampaignModalOpen.value = true

  console.log('Campanha clicada:')
  console.log(campaign)
}

onMounted(async () => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    userEmpresaId.value = user.idEmpresa

    try {
      loading.value = true
      
      // Fetch general metrics
      const metricsRes = await fetch(`/api/Dashboard/metrics/${userEmpresaId.value}`, {
        headers: { 'Authorization': `Bearer ${getToken()}` }
      })
      if (metricsRes.ok) {
        metrics.value = await metricsRes.json()
      }

      // Fetch sector ranking
      const rankingRes = await fetch(`/api/Dashboard/setores/${userEmpresaId.value}`, {
        headers: { 'Authorization': `Bearer ${getToken()}` }
      })
      if (rankingRes.ok) {
        ranking.value = await rankingRes.json()
      }

      // Fetch evolução
      const evolucaoRes = await fetch(
        `/api/Dashboard/evolucao/${userEmpresaId.value}`,
        {
          headers: {
            'Authorization': `Bearer ${getToken()}`
          }
        }
      )

      if (evolucaoRes.ok) {
        evolucao.value = await evolucaoRes.json()

        console.log('EVOLUCAO:')
        console.log(evolucao.value)
        console.log(JSON.stringify(evolucao.value, null, 2))
      }

    } catch (e) {
      console.error('Erro ao buscar dados do dashboard:', e)
    } finally {
      loading.value = false
    }
  }

  
})
</script>

<template>
  <MainLayout>
    <div class="mb-6">
      <h2 class="text-3xl md:text-4xl font-bold text-green-900">Overview da Segurança</h2>
      <p class="text-gray-500 mt-1">Monitoramento em tempo real de vulnerabilidades e métricas.</p>
    </div>

    <div v-if="loading" class="text-center py-12 text-gray-500 font-medium">
      Carregando overview de segurança...
    </div>

    <div v-else>
      <InfoCards :metrics="metrics" />

      <section class="mt-4 flex flex-col lg:flex-row gap-4 items-stretch">
        <div class="flex-[2.3] w-full">
          <MainPlaceholder
            :evolucao="evolucao"
            @campaign-click="openCampaignModal"
            />
        </div>

        <div class="flex-1 w-full lg:w-auto">
          <QuickAccess :ranking="ranking" />
        </div>
      </section>
    </div>
    <div
  v-if="isCampaignModalOpen"
  class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4"
>
  <div
    class="bg-white rounded-3xl shadow-xl w-full max-w-2xl overflow-hidden"
  >

    <div class="p-6 border-b flex justify-between items-center">
      <h3 class="text-2xl font-bold text-green-900">
        {{ selectedCampaign.nomeCampanha }}
      </h3>

      <button
        @click="isCampaignModalOpen = false"
        class="text-gray-400 hover:text-gray-600"
      >
        ✕
      </button>
    </div>

    <div class="p-6 space-y-4">

      <div>
        <span class="font-semibold">Data:</span>
        {{ new Date(selectedCampaign.dt_Disparo)
            .toLocaleDateString('pt-BR') }}
      </div>

      <div>
        <span class="font-semibold">Score:</span>
        {{ selectedCampaign.score }}
      </div>

      <div>
        <span class="font-semibold">Setores:</span>
        {{ selectedCampaign.setores }}
      </div>

      <div>
        <span class="font-semibold">Total de Usuários:</span>
        {{ selectedCampaign.totalUsuarios }}
      </div>

      <div>
        <span class="font-semibold">Links Clicados:</span>
        {{ selectedCampaign.linksClicados }}
      </div>

      <div>
        <span class="font-semibold">Credenciais Enviadas:</span>
        {{ selectedCampaign.credenciaisEnviadas }}
      </div>

    </div>

    <div class="p-6 bg-gray-50 flex justify-end">
      <button
        @click="isCampaignModalOpen = false"
        class="px-5 py-2 bg-green-700 text-white rounded-xl hover:bg-green-800"
      >
        Fechar
      </button>
    </div>

  </div>
</div>
  </MainLayout>
</template>