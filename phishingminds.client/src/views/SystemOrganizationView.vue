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
      <!-- Dev Admin View: List of all Organizations -->
      <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 class="text-3xl md:text-4xl font-bold text-green-900">Lista de Organizações</h2>
          <p class="text-gray-500 mt-1">Gerencie todos os clientes conectados à plataforma Phishing Minds.</p>
        </div>
        <button class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto">
          Cadastrar Cliente
        </button>
      </div>

      <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
        <div class="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
          <h3 class="text-xl font-semibold text-gray-800">Clientes Ativos</h3>
        </div>
        
        <div class="p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse min-w-[700px]">
              <thead>
                <tr class="text-gray-500 text-sm border-b border-gray-100">
                  <th class="pb-3 font-medium">Empresa</th>
                  <th class="pb-3 font-medium">Data Cadastro</th>
                  <th class="pb-3 font-medium">Plano Atual</th>
                  <th class="pb-3 font-medium">Score Médio</th>
                  <th class="pb-3 font-medium">Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr class="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                  <td class="py-4 font-semibold text-gray-800">TechCorp Solutions</td>
                  <td class="py-4 text-gray-500">10/01/2026</td>
                  <td class="py-4"><span class="bg-blue-50 text-blue-700 text-xs px-2 py-1 rounded-md font-semibold">Enterprise Plus</span></td>
                  <td class="py-4 font-medium text-green-600">B+</td>
                  <td class="py-4">
                    <button class="text-green-700 hover:underline text-sm font-medium">Gerenciar</button>
                  </td>
                </tr>
                <tr class="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                  <td class="py-4 font-semibold text-gray-800">Banco Alfa S.A.</td>
                  <td class="py-4 text-gray-500">22/03/2026</td>
                  <td class="py-4"><span class="bg-blue-50 text-blue-700 text-xs px-2 py-1 rounded-md font-semibold">Enterprise</span></td>
                  <td class="py-4 font-medium text-yellow-600">C</td>
                  <td class="py-4">
                    <button class="text-green-700 hover:underline text-sm font-medium">Gerenciar</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <div v-else>
      <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 class="text-3xl md:text-4xl font-bold text-green-900">Estrutura Organizacional</h2>
          <p class="text-gray-500 mt-1">Gerencie departamentos e visualize vulnerabilidades por setor.</p>
        </div>
        <div class="flex flex-col sm:flex-row gap-2 w-full sm:w-auto">
          <button class="bg-white border border-gray-200 hover:bg-gray-50 text-gray-700 px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto">
            Novo Departamento
          </button>
          <button class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto">
            Adicionar Funcionário
          </button>
        </div>
      </div>

      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-8">
        
        <!-- Departments list -->
        <div class="lg:col-span-2 bg-white rounded-3xl shadow-sm overflow-hidden">
          <div class="p-6 border-b border-gray-100">
            <h3 class="text-xl font-semibold text-gray-800">Departamentos</h3>
          </div>
          
          <div class="p-6">
            <div class="space-y-6">
              <div v-for="(dept, index) in ['Financeiro', 'Tecnologia da Informação', 'Recursos Humanos', 'Vendas e Marketing']" :key="index">
                <div class="flex justify-between text-sm mb-2">
                  <span class="font-medium text-gray-700">{{ dept }}</span>
                  <span class="text-gray-500">Risco: Médio</span>
                </div>
                <div class="w-full h-3 bg-gray-100 rounded-full overflow-hidden">
                  <div class="h-full bg-gradient-to-r from-green-500 to-yellow-400" :style="{ width: 40 + (index * 15) + '%' }"></div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <!-- Company Overview -->
        <div class="bg-gradient-to-b from-green-800 to-green-900 rounded-3xl shadow-sm p-6 text-white h-fit">
          <h3 class="text-xl font-semibold mb-6">Visão Geral da Empresa</h3>
          
          <div class="space-y-4">
            <div class="flex justify-between items-center border-b border-green-700 pb-3">
              <span class="text-green-100">Total de Funcionários</span>
              <span class="font-bold text-xl">142</span>
            </div>
            <div class="flex justify-between items-center border-b border-green-700 pb-3">
              <span class="text-green-100">Departamentos</span>
              <span class="font-bold text-xl">8</span>
            </div>
            <div class="flex justify-between items-center pb-3">
              <span class="text-green-100">Score Médio da Empresa</span>
              <span class="font-bold text-xl text-green-300">B+</span>
            </div>
          </div>
        </div>

      </div>
    </div>
  </MainLayout>
</template>
