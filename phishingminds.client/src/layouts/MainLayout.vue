<script setup>
import { ref, onMounted, onUnmounted, watch } from 'vue'
import Sidebar from '../components/Layout/Sidebar.vue'
import Topbar from '../components/Layout/Topbar.vue'

const isDevAdmin = ref(false)
const isCorpAdmin = ref(false)
const isPessoa = ref(false)
const isDarkMode = ref(false)

const handleToggleTheme = () => {
  isDarkMode.value = !isDarkMode.value
  localStorage.setItem('theme', isDarkMode.value ? 'dark' : 'light')
}

onMounted(() => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    isDevAdmin.value = user.idEmpresa === 1 && user.isEmpresa === true
    isCorpAdmin.value = !isDevAdmin.value && user.isEmpresa === true
    isPessoa.value = user.isPessoa === true
    
    // Check saved theme
    const savedTheme = localStorage.getItem('theme')
    if (isDevAdmin.value) {
      // Dev admin is dark by default
      isDarkMode.value = savedTheme !== 'light'
    } else {
      // Corp admin/others are light by default
      isDarkMode.value = savedTheme === 'dark'
    }
  }
})

watch([isDarkMode, isDevAdmin, isCorpAdmin, isPessoa], () => {
  if (isDarkMode.value) {
    document.body.classList.add('dark-theme')
    document.body.classList.remove('corp-light-theme')
    document.body.classList.remove('dev-light-theme')
    if (isCorpAdmin.value || isPessoa.value) {
      document.body.classList.add('corp-theme')
    } else {
      document.body.classList.remove('corp-theme')
    }
  } else {
    document.body.classList.remove('dark-theme')
    document.body.classList.remove('corp-theme')
    if (isCorpAdmin.value || isPessoa.value) {
      document.body.classList.add('corp-light-theme')
      document.body.classList.remove('dev-light-theme')
    } else if (isDevAdmin.value) {
      document.body.classList.add('dev-light-theme')
      document.body.classList.remove('corp-light-theme')
    } else {
      document.body.classList.remove('corp-light-theme')
      document.body.classList.remove('dev-light-theme')
    }
  }
}, { immediate: true })

onUnmounted(() => {
  document.body.classList.remove('dark-theme')
  document.body.classList.remove('corp-theme')
  document.body.classList.remove('corp-light-theme')
  document.body.classList.remove('dev-light-theme')
})

const isSidebarOpen = ref(false)

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value
}
const closeSidebar = () => {
  isSidebarOpen.value = false
}
</script>

<template>
  <div :class="['min-h-screen flex relative overflow-hidden transition-all duration-300', isDarkMode ? 'dark-theme bg-[#0a120e]' : 'bg-[#f5f3ef]']">
    
    <!-- Mobile overlay -->
    <div 
      v-if="isSidebarOpen" 
      @click="closeSidebar"
      class="fixed inset-0 bg-black/50 z-40 md:hidden"
    ></div>

    <!-- Sidebar -->
    <Sidebar 
      :is-open="isSidebarOpen" 
      @close="closeSidebar" 
      class="z-50 transition-transform duration-300 absolute md:static"
      :class="isSidebarOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0'"
    />

    <!-- Main Content -->
    <main class="flex-1 w-full flex flex-col h-screen overflow-y-auto px-4 md:px-6">
      <Topbar 
        @toggle-sidebar="toggleSidebar" 
        :is-dark-mode="isDarkMode"
        :can-toggle-theme="isDevAdmin || isCorpAdmin || isPessoa"
        @toggle-theme="handleToggleTheme"
      />
      <div class="flex-1 pb-6">
        <slot />
      </div>
    </main>

  </div>
</template>
