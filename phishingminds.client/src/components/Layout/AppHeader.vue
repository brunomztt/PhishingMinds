<template>
  <header class="absolute top-0 left-0 w-full z-50 px-10 py-5 flex justify-between items-center text-white">
    <h1 class="font-bold text-lg">PhishingMinds</h1>

    <nav class="flex gap-6 text-sm">
      <a href="#secao-recursos">Recursos</a>
      <a href="#secao-planos">Planos</a>
      <a href="#secao-sobre">Sobre</a>
    </nav>

    <div class="flex gap-3">
      <BotaoPadrao tipo="secundario">Entrar</BotaoPadrao>
      <BotaoPadrao>Começar</BotaoPadrao>
    </div>
  </header>
</template>

<script setup>
import BotaoPadrao from '../ui/BotaoPadrao.vue'
</script>
