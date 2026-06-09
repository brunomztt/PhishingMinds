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

const isPessoa = ref(false)
const currentUser = ref(null)

const minhasMetricas = ref({
  totalQuedas: 0,
  treinamentos: 0,
  status: 'Em dia'
})

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

  if (!userStr) return

  const user = JSON.parse(userStr)

  currentUser.value = user
  isPessoa.value = user.isPessoa === true
  userEmpresaId.value = user.idEmpresa

  loading.value = true

  try {

    // DASHBOARD DO FUNCIONÁRIO
    if (isPessoa.value) {

      const res = await fetch(
        `/api/Pessoa/necessita-treinamento/${user.idUser}`,
        {
          headers: {
            Authorization: `Bearer ${getToken()}`
          }
        }
      )

      if (res.ok) {
        const dados = await res.json()

        console.log('TREINAMENTO:', dados)

        minhasMetricas.value = {
          totalQuedas: dados.totalQuedas,
          treinamentos: dados.totalQuedas >= 3 ? 1 : 0,
          status: dados.necessitaTreinamento
            ? 'Treinamento Pendente'
            : 'Em Dia'
        }
      }

      return
    }

    // DASHBOARD DO GESTOR / EMPRESA
    const metricsRes = await fetch(
      `/api/Dashboard/metrics/${userEmpresaId.value}`,
      {
        headers: {
          Authorization: `Bearer ${getToken()}`
        }
      }
    )

    if (metricsRes.ok) {
      metrics.value = await metricsRes.json()
    }

    const rankingRes = await fetch(
      `/api/Dashboard/setores/${userEmpresaId.value}`,
      {
        headers: {
          Authorization: `Bearer ${getToken()}`
        }
      }
    )

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

 <!-- DASHBOARD FUNCIONÁRIO -->
<div v-if="isPessoa">

  <!-- responsividade -->
  <div class="grid grid-cols-1 md:grid-cols-3 gap-4">

    <div class="bg-white rounded-3xl p-6 shadow-sm">
      <p class="text-gray-500 mb-2">
        Status
      </p>

      <p
        class="text-2xl font-bold"
        :class="
          minhasMetricas.status === 'Em Dia'
            ? 'text-green-700'
            : 'text-red-600'
        "
      >
        {{ minhasMetricas.status }}
      </p>
    </div>

    <div class="bg-white rounded-3xl p-6 shadow-sm">
      <p class="text-gray-500 mb-2">
        Quedas em Phishing
      </p>

      <p class="text-2xl font-bold text-green-900">
        {{ minhasMetricas.totalQuedas }}
      </p>
    </div>

    <div class="bg-white rounded-3xl p-6 shadow-sm">
      <p class="text-gray-500 mb-2">
        Treinamentos
      </p>

      <p class="text-2xl font-bold text-green-900">
        {{ minhasMetricas.treinamentos }}
      </p>
    </div>

  </div>

  <div class="bg-white rounded-3xl p-8 shadow-sm mt-6">

    <h3 class="text-2xl font-bold text-green-900 mb-4">
      Minha Segurança
    </h3>

    <p class="text-gray-600">
      Fique atento a links suspeitos, remetentes desconhecidos e pedidos de senha por e-mail.
    </p>

    <div
      v-if="minhasMetricas.status === 'Treinamento Pendente'"
      class="mt-6"
    >
      <router-link
        to="/treinamentos"
        class="inline-flex bg-red-600 text-white px-5 py-3 rounded-xl font-semibold"
      >
        Iniciar Treinamento Obrigatório
      </router-link>
    </div>

    <div
      v-else
      class="mt-6 p-4 bg-green-50 rounded-xl text-green-700 font-medium"
    >
      🎓 Você está em dia com seus treinamentos.
    </div>

  </div>

</div>

<!-- DASHBOARD GESTOR -->
<div v-else>

  <InfoCards :metrics="metrics" />

  <!-- responsividade -->
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
      </div> <!-- FECHA O V-ELSE PRINCIPAL -->
    <!-- responsividade -->
    <div
  v-if="isCampaignModalOpen"
  class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4 overflow-y-auto"
>
  <div
    class="bg-white rounded-3xl shadow-xl w-full max-w-2xl overflow-hidden my-auto"
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
