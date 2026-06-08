<script setup>
import { ref, onMounted } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'

const isDevAdmin = ref(false)
const userEmpresaId = ref(null)
const loading = ref(true)

// Dev Admin State
const mrr = ref(0)
const activeCompanies = ref(0)
const subscriptions = ref([])

// Corporation State
const planName = ref('Starter')
const planValue = ref(99.90)
const activeEmployees = ref(0)
const maxEmployees = ref(50)
const campaignCount = ref(0)
const maxCampaigns = ref(5)

const getToken = () => localStorage.getItem('token')

onMounted(async () => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    isDevAdmin.value = user.idEmpresa === 1
    userEmpresaId.value = user.idEmpresa

    try {
      loading.value = true
      if (isDevAdmin.value) {
        // Fetch global contracts
        const res = await fetch('/api/Empresa/global-contracts', {
          headers: { 'Authorization': `Bearer ${getToken()}` }
        })
        if (res.ok) {
          const data = await res.json()
          mrr.value = data.mrr || 0
          activeCompanies.value = data.activeCompanies || 0
          subscriptions.value = data.subscriptions || []
        }
      } else {
        // Fetch corporation billing info
        const res = await fetch(`/api/Empresa/billing/${userEmpresaId.value}`, {
          headers: { 'Authorization': `Bearer ${getToken()}` }
        })
        if (res.ok) {
          const data = await res.json()
          planName.value = data.plano.nm_Plano
          planValue.value = data.plano.value_Plano
          activeEmployees.value = data.activeEmployees
          maxEmployees.value = data.plano.maxUsers
          campaignCount.value = data.campaignCount
          maxCampaigns.value = data.plano.maxCampaigns
        }
      }
    } catch (e) {
      console.error('Erro ao buscar dados de faturamento/contratos:', e)
    } finally {
      loading.value = false
    }
  }
})
</script>

<template>
  <MainLayout>
    <div v-if="loading" class="text-center py-12 text-gray-500 font-medium">
      Carregando informações de contratos...
    </div>
    
    <div v-else-if="isDevAdmin">
      <!-- Dev Admin View: Dashboard of all companies -->
      <div class="mb-6">
        <h2 class="text-3xl md:text-4xl font-bold text-green-900">Dashboard Financeiro Global</h2>
        <p class="text-gray-500 mt-1">Visão geral de todos os contratos ativos e receita mensal recorrente.</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
        <div class="bg-gradient-to-r from-green-700 to-green-900 rounded-3xl shadow-sm p-6 text-white">
          <p class="text-green-100 text-sm mb-2">MRR (Receita Mensal Recorrente)</p>
          <h3 class="text-4xl font-bold">R$ {{ mrr.toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }}</h3>
        </div>
        <div class="bg-white rounded-3xl shadow-sm p-6 border border-gray-100">
          <p class="text-gray-500 text-sm mb-2">Empresas Ativas</p>
          <h3 class="text-4xl font-bold text-green-900">{{ activeCompanies }}</h3>
        </div>
        <div class="bg-white rounded-3xl shadow-sm p-6 border border-gray-100">
          <p class="text-gray-500 text-sm mb-2">Novos Contratos (Mês)</p>
          <h3 class="text-4xl font-bold text-green-900">+{{ activeCompanies }}</h3>
        </div>
      </div>

      <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
        <div class="p-6 border-b border-gray-100">
          <h3 class="text-xl font-semibold text-gray-800">Assinaturas Ativas</h3>
        </div>
        <div class="p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse min-w-[600px]">
              <thead>
                <tr class="text-gray-500 text-sm border-b border-gray-100">
                  <th class="pb-3 font-medium">Empresa</th>
                  <th class="pb-3 font-medium">Plano</th>
                  <th class="pb-3 font-medium">Valor Mensal</th>
                  <th class="pb-3 font-medium">Status</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="sub in subscriptions" :key="sub.nm_Empresa" class="border-b border-gray-50 hover:bg-gray-50">
                  <td class="py-4 font-semibold text-gray-800">{{ sub.nm_Empresa }}</td>
                  <td class="py-4 text-gray-600">{{ sub.nm_Plano }}</td>
                  <td class="py-4 font-medium text-gray-800">R$ {{ (sub.value_Plano ?? 0).toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }}</td>
                  <td class="py-4">
                    <span :class="[sub.ativo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800', 'text-xs px-2 py-1 rounded-full font-bold']">
                      {{ sub.ativo ? 'ATIVO' : 'INATIVO' }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <div v-else>
      <div class="mb-6">
        <h2 class="text-3xl md:text-4xl font-bold text-green-900">Plano e Faturamento</h2>
        <p class="text-gray-500 mt-1">Gerencie os limites da sua assinatura SaaS e detalhes de cobrança.</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mt-8">
        <!-- Current Plan -->
        <div class="bg-white rounded-3xl shadow-sm p-8 border-t-4 border-green-700">
          <div class="flex justify-between items-start mb-6">
            <div>
              <span class="text-xs font-bold uppercase tracking-wider text-green-700 bg-green-50 px-3 py-1 rounded-full">Plano Atual</span>
              <h3 class="text-2xl font-bold text-gray-800 mt-3">{{ planName }}</h3>
            </div>
            <div class="text-right">
              <div class="text-3xl font-bold text-gray-800">R$ {{ planValue.toLocaleString('pt-BR', { minimumFractionDigits: 2 }) }}</div>
              <div class="text-sm text-gray-500">/mês</div>
            </div>
          </div>

          <div class="space-y-4 mb-8">
            <div class="flex items-center gap-3">
              <svg class="w-5 h-5 text-green-500 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-gray-600">Simulações de phishing inteligentes</span>
            </div>
            <div class="flex items-center gap-3">
              <svg class="w-5 h-5 text-green-500 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-gray-600">Treinamentos e campanhas integradas</span>
            </div>
            <div class="flex items-center gap-3">
              <svg class="w-5 h-5 text-green-500 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-gray-600">Dashboard de métricas corporativas</span>
            </div>
          </div>

          <button class="w-full bg-green-50 text-green-700 hover:bg-green-100 font-semibold py-3 rounded-xl transition-colors">
            Fazer Upgrade
          </button>
        </div>

        <!-- Limits & Usage -->
        <div class="bg-white rounded-3xl shadow-sm p-8">
          <h3 class="text-xl font-semibold text-gray-800 mb-6">Uso da Conta</h3>

          <div class="space-y-6">
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="font-medium text-gray-700">Funcionários Ativos</span>
                <span class="text-gray-500">{{ activeEmployees }} / {{ maxEmployees }}</span>
              </div>
              <div class="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
                <div class="h-full bg-green-500" :style="{ width: Math.min(100, (activeEmployees * 100 / maxEmployees)) + '%' }"></div>
              </div>
            </div>

            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="font-medium text-gray-700">Campanhas Mensais</span>
                <span class="text-gray-500">{{ campaignCount }} / {{ maxCampaigns === 999 ? 'Ilimitado' : maxCampaigns }}</span>
              </div>
              <div class="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
                <div class="h-full bg-green-500" :style="{ width: Math.min(100, (campaignCount * 100 / (maxCampaigns === 999 ? 100 : maxCampaigns))) + '%' }"></div>
              </div>
            </div>
            
            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="font-medium text-gray-700">Armazenamento de Relatórios</span>
                <span class="text-gray-500">4GB / 10GB</span>
              </div>
              <div class="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
                <div class="h-full bg-yellow-400" style="width: 40%"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </MainLayout>
</template>
