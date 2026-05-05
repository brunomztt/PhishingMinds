<template>
  <div>
    <AppHeader />
    <HeroSection />
    <Estatisticas />
    <Recursos />
    <Sobre />
    <Processo />
    <Planos />
    <AppFooter></AppFooter>
  </div>
</template>

<script setup>
  import AppHeader from '../components/layout/AppHeader.vue'
  import HeroSection from '../components/secoes/HeroSection.vue'
  import Estatisticas from '../components/secoes/Estatisticas.vue'
  import Recursos from '../components/secoes/Recursos.vue'
  import Sobre from '../components/secoes/Sobre.vue'
  import Processo from '../components/secoes/Processo.vue'
  import Planos from '../components/secoes/Planos.vue'
  import AppFooter from '../components/layout/AppFooter.vue'
</script>
