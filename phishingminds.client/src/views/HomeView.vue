<template>
  <div>
    <AppHeader />
    <HeroSection />
    <Estatisticas />
    <Recursos />
    <Sobre />
    <Processo />
    <PlanosSection />
    <AppFooter></AppFooter>
  </div>
</template>

<script setup>
  import AppHeader from '../components/Layout/AppHeader.vue'
  import HeroSection from '../components/secoes/HeroSection.vue'
  import Estatisticas from '../components/secoes/Estatisticas.vue'
  import Recursos from '../components/secoes/Recursos.vue'
  import Sobre from '../components/secoes/Sobre.vue'
  import Processo from '../components/secoes/Processo.vue'
  import PlanosSection from '../components/secoes/PlanosSection.vue'
  import AppFooter from '../components/Layout/AppFooter.vue'
</script>
