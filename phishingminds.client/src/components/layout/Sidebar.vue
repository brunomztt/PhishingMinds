<template>
  <aside class="w-64 bg-white md:m-4 md:rounded-3xl shadow-md flex flex-col justify-between px-5 py-6 h-full fixed md:sticky top-0 md:top-4 md:h-[calc(100vh-2rem)] overflow-y-auto">
    <div>
      <div class="mb-10 flex items-center justify-between md:block text-center">
        <h1 class="text-2xl font-bold text-green-900 leading-tight w-full">
          PHISHING <br class="hidden md:block" />
          <span class="md:hidden"> </span>MINDS
        </h1>
        <button @click="$emit('close')" class="md:hidden text-gray-500 hover:text-gray-800 text-3xl px-2 leading-none">&times;</button>
      </div>

      <nav class="space-y-2">
        <router-link to="/" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium" :class="$route.path === '/' ? 'bg-green-700 text-white' : 'hover:bg-gray-100 text-gray-600'">
          Overview
        </router-link>
        <router-link to="/campanhas" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium hover:bg-gray-100 text-gray-600" :class="$route.path.startsWith('/campanhas') ? 'bg-green-700 text-white' : ''">
          Campanhas
        </router-link>
        <router-link to="/organizacao" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium hover:bg-gray-100 text-gray-600" :class="$route.path.startsWith('/organizacao') ? 'bg-green-700 text-white' : ''">
          Organização
        </router-link>
        <router-link to="/contratos" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium hover:bg-gray-100 text-gray-600" :class="$route.path.startsWith('/contratos') ? 'bg-green-700 text-white' : ''">
          Contratos
        </router-link>
        <router-link to="/perfil" @click="$emit('close')" class="block w-full text-center md:text-left px-4 py-3 rounded-xl font-medium hover:bg-gray-100 text-gray-600" :class="$route.path.startsWith('/perfil') ? 'bg-green-700 text-white' : ''">
          Meu Perfil
        </router-link>
      </nav>
    </div>

    <button @click="handleLogout" class="text-red-500 text-left px-4 mt-10 w-full hover:bg-red-50 py-3 rounded-xl transition-colors font-medium">Sair</button>
  </aside>
</template>

<script setup>
import { useRouter } from 'vue-router'

defineEmits(['close'])

const router = useRouter()
const handleLogout = () => {
  localStorage.removeItem('token')
  localStorage.removeItem('user')
  router.push('/login')
}
</script>
