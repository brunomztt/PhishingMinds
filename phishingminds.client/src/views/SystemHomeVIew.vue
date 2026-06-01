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

const getToken = () => localStorage.getItem('token')

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
          <MainPlaceholder />
        </div>

        <div class="flex-1 w-full lg:w-auto">
          <QuickAccess :ranking="ranking" />
        </div>
      </section>
    </div>
  </MainLayout>
</template>