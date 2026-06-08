<script setup>
import { ref, onMounted } from 'vue'
import MainLayout from '@/layouts/MainLayout.vue'
</script>

<template>
  <MainLayout>
    <div>
      <h1 class="text-5xl font-bold text-green-900">
        Treinamentos
      </h1>

      <p class="text-gray-500 text-xl mt-2 mb-8">
        Capacitações obrigatórias para reforçar a segurança da informação.
      </p>

      <div class="bg-white rounded-3xl p-8 shadow-sm">
        <h2 class="text-2xl font-semibold text-gray-800 mb-3">
          Verificando necessidade de treinamento...
        </h2>

        <p class="text-gray-500">
          Carregando...
        </p>
      </div>
    </div>
  </MainLayout>
</template>
