<template>
  <header class="absolute top-0 left-0 w-full z-50 px-6 md:px-10 py-5 flex justify-between items-center text-white">
    <div class="font-bold text-lg flex items-center gap-2">
      PhishingMinds
    </div>

    <nav class="hidden md:flex gap-6 text-sm">
      <a href="#secao-recursos" class="hover:text-green-300 transition-colors">Recursos</a>
      <a href="#secao-planos" class="hover:text-green-300 transition-colors">Planos</a>
      <a href="#secao-sobre" class="hover:text-green-300 transition-colors">Sobre</a>
    </nav>

    <div class="hidden md:flex gap-3">
      <button @click="$router.push('/login')" class="text-white hover:text-green-200 px-4 py-2">Entrar</button>
      <button @click="$router.push('/cadastro')" class="bg-green-500 hover:bg-green-400 text-[#1a3224] px-4 py-2 rounded-lg font-bold">Começar</button>
    </div>

    <button class="md:hidden text-white">
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-6 h-6">
        <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
      </svg>
    </button>
  </header>
</template>
