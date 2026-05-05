<script setup>
import { ref, onMounted } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'

const isDevAdmin = ref(false)

onMounted(() => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    isDevAdmin.value = user.idEmpresa === 1
  }
})
</script>

<template>
  <MainLayout>
    <div v-if="isDevAdmin">
      <!-- Dev Admin View: Dashboard of all companies -->
      <div class="mb-6">
        <h2 class="text-3xl md:text-4xl font-bold text-green-900">Dashboard Financeiro Global</h2>
        <p class="text-gray-500 mt-1">Visão geral de todos os contratos ativos e receita mensal recorrente.</p>
      </div>

      <div class="grid grid-cols-1 md:grid-cols-3 gap-6 mt-8">
        <div class="bg-gradient-to-r from-green-700 to-green-900 rounded-3xl shadow-sm p-6 text-white">
          <p class="text-green-100 text-sm mb-2">MRR (Receita Mensal Recorrente)</p>
          <h3 class="text-4xl font-bold">R$ 14.850</h3>
        </div>
        <div class="bg-white rounded-3xl shadow-sm p-6 border border-gray-100">
          <p class="text-gray-500 text-sm mb-2">Empresas Ativas</p>
          <h3 class="text-4xl font-bold text-green-900">24</h3>
        </div>
        <div class="bg-white rounded-3xl shadow-sm p-6 border border-gray-100">
          <p class="text-gray-500 text-sm mb-2">Novos Contratos (Mês)</p>
          <h3 class="text-4xl font-bold text-green-900">+3</h3>
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
                <tr class="border-b border-gray-50 hover:bg-gray-50">
                  <td class="py-4 font-semibold text-gray-800">TechCorp Solutions</td>
                  <td class="py-4 text-gray-600">Enterprise Plus</td>
                  <td class="py-4 font-medium text-gray-800">R$ 990</td>
                  <td class="py-4"><span class="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full font-bold">ATIVO</span></td>
                </tr>
                <tr class="border-b border-gray-50 hover:bg-gray-50">
                  <td class="py-4 font-semibold text-gray-800">Banco Alfa S.A.</td>
                  <td class="py-4 text-gray-600">Enterprise</td>
                  <td class="py-4 font-medium text-gray-800">R$ 750</td>
                  <td class="py-4"><span class="bg-green-100 text-green-800 text-xs px-2 py-1 rounded-full font-bold">ATIVO</span></td>
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
              <h3 class="text-2xl font-bold text-gray-800 mt-3">Enterprise Plus</h3>
            </div>
            <div class="text-right">
              <div class="text-3xl font-bold text-gray-800">R$ 990</div>
              <div class="text-sm text-gray-500">/mês</div>
            </div>
          </div>

          <div class="space-y-4 mb-8">
            <div class="flex items-center gap-3">
              <svg class="w-5 h-5 text-green-500 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-gray-600">Simulações de phishing ilimitadas</span>
            </div>
            <div class="flex items-center gap-3">
              <svg class="w-5 h-5 text-green-500 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-gray-600">Treinamentos interativos</span>
            </div>
            <div class="flex items-center gap-3">
              <svg class="w-5 h-5 text-green-500 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20"><path fill-rule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clip-rule="evenodd"></path></svg>
              <span class="text-gray-600">Dashboard executivo avançado</span>
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
                <span class="text-gray-500">142 / 500</span>
              </div>
              <div class="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
                <div class="h-full bg-green-500" style="width: 28%"></div>
              </div>
            </div>

            <div>
              <div class="flex justify-between text-sm mb-2">
                <span class="font-medium text-gray-700">Campanhas Mensais</span>
                <span class="text-gray-500">12 / Ilimitado</span>
              </div>
              <div class="w-full h-2 bg-gray-100 rounded-full overflow-hidden">
                <div class="h-full bg-green-500" style="width: 10%"></div>
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
