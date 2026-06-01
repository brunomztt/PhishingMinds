<script setup>
  import { ref } from 'vue'
  import { useRouter } from 'vue-router'
  import logo from '@/assets/logo.svg'
  import LogoPhishingMinds from '@/assets/LogoPhishingMinds.png'

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

      router.push('/painel')
    } catch (err) {
      errorMsg.value = 'Credenciais inválidas. Tente novamente.'
    } finally {
      loading.value = false
    }
  }
</script>

<template>
  <div class="min-h-screen flex">

    <!--  LADO ESQUERDO -->
    <div class="hidden md:flex w-1/2 bg-gradient-to-br bg-[#2D4A38] text-white p-12 flex-col justify-center">

      <div class="max-w-md">

        <!-- LOGO -->
        <div class="mb-10 flex items-center gap-3">
          <span class="text-xl font-semibold tracking-wide">
            PHISHING MINDS
          </span>
        </div>

        <h1 class="text-4xl font-bold leading-tight mb-6">
          Proteja a mente da sua organização.
        </h1>

        <p class="text-green-100">
          O Phishing Minds simula ataques reais para treinar, mensurar e evoluir a cultura de cibersegurança da sua empresa.
        </p>
      </div>
    </div>

    <!-- LADO DIREITO -->
    <div class="w-full md:w-1/2 bg-[#f5f3ef] flex items-center justify-center p-6">

      <div class="bg-white w-full max-w-md rounded-3xl shadow-xl p-8 relative">

        <!-- Botão Voltar -->
        <button @click="$router.push('/')" class="absolute top-6 left-6 text-green-700 flex items-center gap-1 text-sm font-medium hover:underline">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"></path></svg>
          Voltar
        </button>

        <div class="text-center mb-6 mt-4">
          <h2 class="text-2xl font-bold text-green-900">
            Bem-vindo de volta
          </h2>
          <p class="text-gray-500 text-sm">
            Acesse seu painel de controle
          </p>
        </div>

        <form @submit.prevent="handleLogin" class="space-y-5">

          <div v-if="errorMsg" class="bg-red-50 text-red-600 p-3 rounded-lg text-sm text-center">
            {{ errorMsg }}
          </div>

          <!-- EMAIL -->
          <div>
            <label class="text-sm text-gray-600">E-mail Corporativo</label>
            <input v-model="email"
                   type="email"
                   required
                   class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                   placeholder="admin@empresa.com" />
          </div>

          <!-- SENHA -->
          <div>
            <div class="flex justify-between text-sm text-gray-600">
              <label>Senha</label>
              <span @click="$router.push('/reset-password')"
                    class="text-green-700 cursor-pointer hover:underline">
                Esqueceu a senha?
              </span>
            </div>

            <input v-model="password"
                   type="password"
                   required
                   class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                   placeholder="••••••••" />
          </div>

          <!-- BOTÃO -->
          <button type="submit"
                  :disabled="loading"
                  class="w-full bg-[#2D4A38] hover:bg-green-800 text-white py-3 rounded-xl font-medium shadow-md transition">
            {{ loading ? 'Entrando...' : 'Entrar na plataforma →' }}
          </button>

          <p class="text-xs text-center text-gray-500 mt-4">
            Sua empresa ainda não tem conta?
            <span @click="$router.push('/cadastro')"
                  class="text-green-700 cursor-pointer hover:underline">
              Solicite uma demonstração
            </span>
          </p>

        </form>
      </div>

    </div>
  </div>
</template>
