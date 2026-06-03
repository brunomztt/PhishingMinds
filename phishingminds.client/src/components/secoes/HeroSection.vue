<template>
  <section id="secao-hero" class="relative pt-32 pb-40 flex items-center justify-center text-center text-white bg-[#1a3224] rounded-b-[3rem] shadow-xl">

    <div class="absolute inset-0 bg-[url('/caminho-para-imagem-de-fundo.jpg')] bg-cover bg-center opacity-10 rounded-b-[3rem]"></div>

    <div class="relative z-10 max-w-4xl px-4">
      <p class="text-xs tracking-widest uppercase mb-4 text-green-300 font-semibold">
        PLATAFORMA INTELIGENTE DE SEGURANÇA
      </p>

      <h1 class="text-5xl md:text-6xl font-bold mb-6 leading-tight">
        A Primeira Linha de <span class="italic text-green-200">Defesa</span> da sua empresa
      </h1>

      <p class="mb-10 text-gray-300 text-lg max-w-2xl mx-auto">
        Treinamento e conscientização em segurança cibernética. Construa uma cultura de segurança robusta e reduza riscos corporativos.
      </p>

      <div class="flex gap-4 justify-center">
        <button @click="$router.push('/cadastro')" class="bg-green-500 hover:bg-green-400 text-[#1a3224] font-bold py-3 px-8 rounded-full transition-colors">
          Começar Agora
        </button>
        <a href="#secao-planos" class="bg-white hover:bg-gray-100 text-[#1a3224] font-bold py-3 px-8 rounded-full transition-colors">Ver Planos</a>
      </div>
    </div>
  </section>
</template>

<script setup>
  import BotaoPadrao from '../ui/BotaoPadrao.vue'
</script>
