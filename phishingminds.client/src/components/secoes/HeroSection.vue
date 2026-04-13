<template>
  <section id="secao-hero" class="relative h-screen flex items-center justify-center text-center text-white">

    <!-- Overlay -->
    <div class="absolute inset-0 bg-black/60"></div>

    <!-- Conteúdo -->
    <div class="relative z-10 max-w-3xl">
      <p class="text-sm mb-4">PLATAFORMA INTELIGENTE DE SEGURANÇA</p>

      <h1 class="text-5xl font-bold mb-4">
        A Primeira Linha de <span class="italic">Defesa</span> da sua empresa
      </h1>

      <p class="mb-6 text-gray-200">
        Treinamento e conscientização em segurança cibernética
      </p>

      <div class="flex gap-4 justify-center">
        <BotaoPadrao>Começar Agora</BotaoPadrao>
        <BotaoPadrao tipo="secundario">Ver Planos</BotaoPadrao>
      </div>
    </div>
  </section>
</template>

<script setup>
  import BotaoPadrao from '../ui/BotaoPadrao.vue'
</script>
