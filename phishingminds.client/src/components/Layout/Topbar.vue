<template>
  <div class="mb-6 pt-4 md:pt-6">
    <div class="flex justify-between items-center mb-5 gap-4">
      
      <!-- Hamburger for Mobile -->
      <button 
        @click="$emit('toggle-sidebar')"
        class="md:hidden p-2 text-gray-600 bg-white rounded-lg shadow-sm hover:bg-gray-50"
      >
        <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"></path>
        </svg>
      </button>

      <input
        type="text"
        placeholder="Buscar..."
        class="bg-white px-5 py-3 rounded-xl shadow-sm outline-none flex-1 md:w-72 md:flex-none"
      />

      <div class="flex items-center gap-3">
        <div class="hidden md:block text-right leading-tight">
          <h4 class="font-semibold text-sm text-gray-700">
            {{ nomeUsuario }}
          </h4>

          <p class="text-xs text-gray-400">
            {{ tipoUsuario }}
          </p>
        </div>
        <div class="w-10 h-10 bg-green-700 rounded-full flex items-center justify-center text-white font-semibold">
          {{ nomeUsuario.charAt(0).toUpperCase() }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted } from 'vue'

  defineEmits(['toggle-sidebar'])

  const nomeUsuario = ref('Administrador')
  const tipoUsuario = ref('Usuário')

  onMounted(() => {
    const userStr = localStorage.getItem('user')

    if (!userStr) return

    const user = JSON.parse(userStr)

    if (user.isPessoa) {
      nomeUsuario.value = user.nome
      tipoUsuario.value = 'Funcionário'
    }
  })
</script>
