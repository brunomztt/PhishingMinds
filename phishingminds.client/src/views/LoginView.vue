<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const email = ref('')
const password = ref('')
const errorMsg = ref('')
const loading = ref(false)

const handleLogin = async () => {
  loading.value = true
  errorMsg.value = ''
  
  try {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email: email.value, password: password.value })
    })

    if (!response.ok) {
      throw new Error('Falha na autenticação')
    }

    const data = await response.json()
    localStorage.setItem('token', data.token)
    localStorage.setItem('user', JSON.stringify(data.user))
    
    router.push('/')
  } catch (err) {
    errorMsg.value = 'Credenciais inválidas. Tente novamente.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen bg-[#f5f3ef] flex items-center justify-center p-4">
    <div class="bg-white max-w-md w-full rounded-3xl shadow-xl p-8">
      <div class="text-center mb-8">
        <h1 class="text-3xl font-bold text-green-900 leading-tight">
          PHISHING <br /> MINDS
        </h1>
        <p class="text-gray-500 mt-2">Faça login para acessar o painel.</p>
      </div>

      <form @submit.prevent="handleLogin" class="space-y-6">
        <div v-if="errorMsg" class="bg-red-50 text-red-600 p-3 rounded-lg text-sm text-center">
          {{ errorMsg }}
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">E-mail</label>
          <input 
            v-model="email" 
            type="email" 
            required
            class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" 
            placeholder="admin@phishingminds.com"
          />
        </div>

        <div>
          <label class="block text-sm font-medium text-gray-700 mb-2">Senha</label>
          <input 
            v-model="password" 
            type="password" 
            required
            class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" 
            placeholder="••••••••"
          />
        </div>

        <button 
          type="submit" 
          :disabled="loading"
          class="w-full bg-green-700 hover:bg-green-800 text-white py-3 rounded-xl font-medium shadow-sm transition-colors"
        >
          {{ loading ? 'Entrando...' : 'Entrar' }}
        </button>
      </form>
    </div>
  </div>
</template>
