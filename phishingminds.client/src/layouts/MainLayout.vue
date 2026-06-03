<script setup>
import { ref } from 'vue'
import Sidebar from '../components/Layout/Sidebar.vue'
import Topbar from '../components/Layout/Topbar.vue'

const isSidebarOpen = ref(false)

const toggleSidebar = () => {
  isSidebarOpen.value = !isSidebarOpen.value
}
const closeSidebar = () => {
  isSidebarOpen.value = false
}
</script>

<template>
  <div class="min-h-screen bg-[#f5f3ef] flex relative overflow-hidden">
    
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
      <Topbar @toggle-sidebar="toggleSidebar" />
      <div class="flex-1 pb-6">
        <slot />
      </div>
    </main>

  </div>
</template>
