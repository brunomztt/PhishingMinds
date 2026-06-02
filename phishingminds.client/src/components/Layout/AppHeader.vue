<template>
  <header class="absolute top-0 left-0 w-full z-50 px-6 md:px-12 py-5 flex justify-between items-center text-white bg-[#2D4A38]">
    <div class="font-bold text-xl flex items-center gap-2 tracking-wide">
      <div class="w-10 h-10 flex-shrink-0">
        <img src="@/assets/img/logo.svg" alt="Logo Phishing Minds" class="w-full h-full object-contain" />
      </div>
      PhishingMinds
    </div>

    <nav class="hidden md:flex gap-8 text-sm font-bold tracking-wide">
      <a href="#secao-processo" class="hover:text-[#A8D5B8] transition-colors">COMO FUNCIONA</a>
      <a href="#secao-planos" class="hover:text-[#A8D5B8] transition-colors">PREÇOS</a>
      <a href="#secao-sobre" class="hover:text-[#A8D5B8] transition-colors">BENEFÍCIOS</a>
      <a href="#secao-recursos" class="hover:text-[#A8D5B8] transition-colors">RECURSOS</a>
    </nav>

    <div class="hidden md:flex gap-4 items-center">
      <button @click="$router.push('/login')" class="text-white border border-white hover:bg-white/10 px-5 py-2 rounded-lg font-bold transition-colors">
        ENTRAR
      </button>
      <button @click="$router.push('/cadastro')" class="bg-[#030213] hover:bg-black text-white px-5 py-2 rounded-lg font-bold transition-colors">
        COMEÇAR AGORA
      </button>
    </div>

    <button class="md:hidden text-white">
      <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" class="w-7 h-7">
        <path stroke-linecap="round" stroke-linejoin="round" d="M3.75 6.75h16.5M3.75 12h16.5m-16.5 5.25h16.5" />
      </svg>
    </button>
  </header>
</template>
