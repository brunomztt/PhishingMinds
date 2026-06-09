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
        <!-- Toggle Dark Mode -->
        <button 
          v-if="canToggleTheme"
          @click="$emit('toggle-theme')" 
          class="p-2.5 rounded-xl transition-all shadow-sm border focus:outline-none"
          :class="isDarkMode ? 'bg-[#1d3326] border-[#2b4a37] text-amber-400 hover:bg-[#2b4a37]' : 'bg-white border-gray-100 text-gray-500 hover:bg-gray-50'"
          title="Alternar Tema"
        >
          <!-- Sun Icon for Dark Mode -->
          <svg v-if="isDarkMode" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 3v1m0 16v1m9-9h-1M4 12H3m15.364-6.364l-.707.707M6.343 17.657l-.707.707m0-11.314l.707.707m11.314 11.314l.707.707M12 8a4 4 0 100 8 4 4 0 000-8z"></path>
          </svg>
          <!-- Moon Icon for Light Mode -->
          <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20.354 15.354A9 9 0 018.646 3.646 9.003 9.003 0 0012 21a9.003 9.003 0 008.354-5.646z"></path>
          </svg>
        </button>

        <div class="hidden md:block text-right leading-tight">
          <h4 class="font-semibold text-sm" :class="isDarkMode ? 'text-white' : 'text-gray-700'">
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

  defineProps({
    isDarkMode: {
      type: Boolean,
      default: false
    },
    canToggleTheme: {
      type: Boolean,
      default: false
    }
  })

  defineEmits(['toggle-sidebar', 'toggle-theme'])

  const nomeUsuario = ref('Administrador')
  const tipoUsuario = ref('Usuário')

  onMounted(() => {
    const userStr = localStorage.getItem('user')

    if (!userStr) return

    const user = JSON.parse(userStr)

    if (user.isPessoa) {
      nomeUsuario.value = user.nome || 'Funcionário'
      tipoUsuario.value = 'Funcionário'
    } else if (user.idEmpresa === 1 && user.isEmpresa === true) {
      nomeUsuario.value = user.nome || 'Admin Dev'
      tipoUsuario.value = 'Desenvolvedor'
    } else {
      nomeUsuario.value = user.nome || 'Administrador'
      tipoUsuario.value = 'Administrador'
    }
  })
</script>
