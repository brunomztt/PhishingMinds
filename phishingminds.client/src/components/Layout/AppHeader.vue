<template>
  <!-- responsividade: header container changes to flex-col on mobile when menu is open -->
  <header class="absolute top-0 left-0 w-full z-50 px-6 md:px-12 py-5 flex flex-col text-white bg-[#2D4A38] transition-all duration-300">
    <div class="w-full flex justify-between items-center">
      <div class="font-bold text-xl flex items-center gap-2 tracking-wide">
        <div class="w-10 h-10 flex-shrink-0">
          <img src="@/assets/img/Logo.svg" alt="Logo Phishing Minds" class="w-full h-full object-contain" />
        </div>
        PhishingMinds
      </div>

      <!-- responsividade: navigation links hidden on mobile, visible on desktop -->
      <nav class="hidden md:flex gap-8 text-sm font-bold tracking-wide">
        <a href="#secao-processo" class="hover:text-[#A8D5B8] transition-colors">COMO FUNCIONA</a>
        <a href="#secao-planos" class="hover:text-[#A8D5B8] transition-colors">PREÇOS</a>
        <a href="#secao-sobre" class="hover:text-[#A8D5B8] transition-colors">BENEFÍCIOS</a>
        <a href="#secao-recursos" class="hover:text-[#A8D5B8] transition-colors">RECURSOS</a>
      </nav>

      <!-- responsividade: cta buttons hidden on mobile, visible on desktop -->
      <div class="hidden md:flex gap-4 items-center">
        <button @click="$router.push('/login')" class="text-white border border-white hover:bg-white/10 px-5 py-2 rounded-lg font-bold transition-colors">
          ENTRAR
        </button>
        <button @click="$router.push('/cadastro')" class="bg-[#030213] hover:bg-black text-white px-5 py-2 rounded-lg font-bold transition-colors">
          COMEÇAR AGORA
        </button>
      </div>

      <!-- responsividade: mobile menu toggle button, visible on mobile only -->
      <button @click="isMobileMenuOpen = !isMobileMenuOpen" class="md:hidden text-white focus:outline-none">
        <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-7 h-7">
          <path v-if="!isMobileMenuOpen" stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
          <path v-else stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>

    <!-- responsividade: mobile navigation dropdown menu -->
    <div v-if="isMobileMenuOpen" class="md:hidden w-full flex flex-col gap-4 mt-4 pt-4 border-t border-white/10 text-sm font-bold tracking-wide transition-all duration-300">
      <a href="#secao-processo" @click="isMobileMenuOpen = false" class="hover:text-[#A8D5B8] transition-colors py-2">COMO FUNCIONA</a>
      <a href="#secao-planos" @click="isMobileMenuOpen = false" class="hover:text-[#A8D5B8] transition-colors py-2">PREÇOS</a>
      <a href="#secao-sobre" @click="isMobileMenuOpen = false" class="hover:text-[#A8D5B8] transition-colors py-2">BENEFÍCIOS</a>
      <a href="#secao-recursos" @click="isMobileMenuOpen = false" class="hover:text-[#A8D5B8] transition-colors py-2">RECURSOS</a>
      <div class="border-t border-white/10 pt-4 flex flex-col gap-3">
        <button @click="$router.push('/login'); isMobileMenuOpen = false" class="w-full text-white border border-white hover:bg-white/10 px-5 py-2 rounded-lg font-bold transition-colors">
          ENTRAR
        </button>
        <button @click="$router.push('/cadastro'); isMobileMenuOpen = false" class="w-full bg-[#030213] hover:bg-black text-white px-5 py-2 rounded-lg font-bold transition-colors">
          COMEÇAR AGORA
        </button>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref } from 'vue'
// responsividade
const isMobileMenuOpen = ref(false)
</script>
