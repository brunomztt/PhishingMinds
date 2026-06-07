<template>
  <!-- pt-28 to account for the AppHeader -->
  <section id="secao-hero" class="relative pt-32 pb-48 md:pb-64 flex flex-col items-center justify-center text-center text-white bg-[#2D4A38] rounded-b-[4rem] md:rounded-b-[8rem] shadow-2xl">
    
    <!-- Background layers -->
    <div class="absolute inset-0 bg-gradient-to-tr from-[#162a1e] via-[#2d4a38] to-[#3f6b4e] opacity-60 rounded-b-[4rem] md:rounded-b-[8rem]"></div>
    <div class="absolute inset-0 bg-gradient-to-br from-[#4A7C59]/40 to-transparent rounded-b-[4rem] md:rounded-b-[8rem]"></div>
    <div class="absolute inset-0 bg-black/40 rounded-b-[4rem] md:rounded-b-[8rem]"></div>

    <!-- Content -->
    <div class="relative z-10 w-full max-w-5xl px-4 flex flex-col items-center">
      <!-- Badge -->
      <div class="inline-flex items-center gap-2 px-5 py-2 mb-8 bg-[#F5F2EC]/10 border border-[#F5F2EC]/20 rounded-full backdrop-blur-sm">
        <svg class="w-4 h-4 text-[#A8D5B8]" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
        </svg>
        <span class="text-xs md:text-sm tracking-widest uppercase text-white/90 font-medium">Plataforma Inteligente de Segurança</span>
      </div>

      <!-- Main Headline -->
      <h1 class="text-5xl md:text-7xl lg:text-[72px] font-bold mb-6 leading-tight tracking-tight font-playfair">
        A Primeira Linha de <br class="hidden md:block"/>
        <span class="italic font-black text-[#A8D5B8]">Defesa da sua empresa</span>
      </h1>

      <!-- Subheadline -->
      <p class="mb-12 text-white/80 text-lg md:text-xl max-w-2xl mx-auto font-light leading-relaxed">
        Treinamento de conscientização em segurança cibernética. Construa uma cultura de segurança robusta e reduza riscos corporativos.
      </p>

      <!-- CTA Buttons -->
      <div class="flex flex-col sm:flex-row gap-4 justify-center items-center w-full max-w-lg">
        <button @click="$router.push('/cadastro')" class="w-full sm:w-auto bg-[#A8D5B8] hover:bg-[#97c4a7] text-[#2D4A38] font-bold text-lg py-3 px-8 rounded-full transition-transform hover:scale-105 flex items-center justify-center gap-2">
          COMEÇAR AGORA
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
            <path fill-rule="evenodd" d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z" clip-rule="evenodd" />
          </svg>
        </button>
        <a href="#secao-planos" class="w-full sm:w-auto bg-[#F5F2EC] hover:bg-white border border-white/30 text-black font-bold text-lg py-3 px-8 rounded-full transition-transform hover:scale-105 text-center">
          VER PLANOS
        </a>
      </div>
    </div>

    <!-- Overlapping Stats Card -->
    <div class="absolute -bottom-24 md:-bottom-20 left-1/2 transform -translate-x-1/2 w-[90%] max-w-5xl">
      <div class="bg-white border border-[#4A7C59]/10 shadow-[0_20px_40px_-15px_rgba(45,74,56,0.15)] rounded-3xl p-6 md:p-10 flex flex-col md:flex-row justify-between items-center gap-8 md:gap-4">
        
        <!-- Stat 1 -->
        <div class="flex flex-col items-center md:items-start text-center md:text-left flex-1">
          <span class="text-4xl md:text-[36px] font-bold text-[#2D4A38] leading-none mb-1">98%</span>
          <span class="text-[#4A7C59] text-sm font-medium uppercase tracking-widest">Redução de Riscos</span>
        </div>

        <div class="hidden md:block w-px h-16 bg-[#4A7C59]/10"></div>

        <!-- Stat 2 -->
        <div class="flex flex-col items-center md:items-start text-center md:text-left flex-1 pl-0 md:pl-4">
          <span class="text-4xl md:text-[36px] font-bold text-[#2D4A38] leading-none mb-1">500+</span>
          <span class="text-[#4A7C59] text-sm font-medium uppercase tracking-widest">Empresas Protegidas</span>
        </div>

        <div class="hidden md:block w-px h-16 bg-[#4A7C59]/10"></div>

        <!-- Stat 3 -->
        <div class="flex flex-col items-center md:items-start text-center md:text-left flex-1 pl-0 md:pl-4">
          <span class="text-4xl md:text-[36px] font-bold text-[#2D4A38] leading-none mb-1">10M+</span>
          <span class="text-[#4A7C59] text-sm font-medium uppercase tracking-widest">Simulações Enviadas</span>
        </div>

      </div>
    </div>

  </section>
</template>

<script setup>
</script>
