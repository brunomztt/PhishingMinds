<template>
  <aside :class="['w-64 md:m-4 md:rounded-3xl shadow-md flex flex-col justify-between px-5 py-6 h-full fixed md:sticky top-0 md:top-4 md:h-[calc(100vh-2rem)] overflow-y-auto transition-colors', isDevAdmin ? 'bg-gray-800 border border-gray-700' : 'bg-white']">
    <div>
      <div class="mb-10 flex items-center justify-between md:block text-center">
        <h1 class="text-2xl font-bold leading-tight w-full" :class="isDevAdmin ? 'text-white' : 'text-green-900'">
          PHISHING <br class="hidden md:block" />
          <span class="md:hidden"> </span>MINDS
        </h1>
        <button @click="$emit('close')" class="md:hidden text-gray-500 hover:text-gray-800 text-3xl px-2 leading-none">&times;</button>
      </div>

      <nav class="space-y-2">
        <router-link to="/painel" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium" :class="$route.path === '/painel' ? (isDevAdmin ? 'bg-green-600 text-white' : 'bg-green-700 text-white') : (isDevAdmin ? 'hover:bg-gray-700 text-gray-300' : 'hover:bg-gray-100 text-gray-600')">
          Overview
        </router-link>
        <router-link to="/campanhas" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium" :class="$route.path.startsWith('/campanhas') ? (isDevAdmin ? 'bg-green-600 text-white' : 'bg-green-700 text-white') : (isDevAdmin ? 'hover:bg-gray-700 text-gray-300' : 'hover:bg-gray-100 text-gray-600')">
          Campanhas
        </router-link>
        <router-link to="/organizacao" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium" :class="$route.path.startsWith('/organizacao') ? (isDevAdmin ? 'bg-green-600 text-white' : 'bg-green-700 text-white') : (isDevAdmin ? 'hover:bg-gray-700 text-gray-300' : 'hover:bg-gray-100 text-gray-600')">
          Organização
        </router-link>
        <router-link to="/contratos" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium" :class="$route.path.startsWith('/contratos') ? (isDevAdmin ? 'bg-green-600 text-white' : 'bg-green-700 text-white') : (isDevAdmin ? 'hover:bg-gray-700 text-gray-300' : 'hover:bg-gray-100 text-gray-600')">
          Contratos
        </router-link>
        <router-link to="/perfil" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium" :class="$route.path.startsWith('/perfil') ? (isDevAdmin ? 'bg-green-600 text-white' : 'bg-green-700 text-white') : (isDevAdmin ? 'hover:bg-gray-700 text-gray-300' : 'hover:bg-gray-100 text-gray-600')">
          Meu Perfil
        </router-link>
      </nav>
    </div>

    <button @click="handleLogout" class="text-red-500 text-left px-4 mt-10 w-full py-3 rounded-xl transition-colors font-medium" :class="isDevAdmin ? 'hover:bg-gray-700' : 'hover:bg-red-50'">Sair</button>
  </aside>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

defineEmits(['close'])

const router = useRouter()
const isDevAdmin = ref(false)

onMounted(() => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    isDevAdmin.value = user.idEmpresa === 1
  }
})
const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  router.push('/login')
}
</script>
