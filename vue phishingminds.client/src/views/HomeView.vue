<template>
  <div>
    <AppHeader />
    <HeroSection />
    <Estatisticas />
  </div>
</template>

<script setup>
defineOptions({ name: 'HomeView' })

import AppHeader from '../components/layout/AppHeader.vue'
import HeroSection from '../components/secoes/HeroSection.vue'
import Estatisticas from '../components/secoes/Estatisticas.vue'
</script>