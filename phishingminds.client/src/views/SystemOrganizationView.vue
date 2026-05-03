<script setup>
import MainLayout from '../layouts/MainLayout.vue'
</script>

<template>
  <MainLayout>
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
  </MainLayout>
</template>
