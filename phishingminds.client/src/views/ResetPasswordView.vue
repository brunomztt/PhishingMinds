<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

const email = ref('')
const password = ref('')
const confirmPassword = ref('')
const errorMsg = ref('')
const successMsg = ref('')
const loading = ref(false)

const handleReset = async () => {
  errorMsg.value = ''
  successMsg.value = ''

  if (password.value !== confirmPassword.value) {
    errorMsg.value = 'As senhas não coincidem.'
    return
  }

  loading.value = true

  try {
    const response = await fetch('/api/auth/reset-password', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        email: email.value,
        newPassword: password.value
      })
    })

    if (!response.ok) throw new Error()

    successMsg.value = 'Senha atualizada com sucesso!'

    setTimeout(() => {
      router.push('/login')
    }, 2000)

  } catch {
    errorMsg.value = 'Erro ao atualizar senha.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-[#f5f3ef] p-6">
    <div class="bg-white w-full max-w-md rounded-3xl shadow-xl p-8">

      <div class="text-center mb-6">
        <h2 class="text-2xl font-bold text-green-900">
          Redefinir senha
        </h2>
        <p class="text-gray-500 text-sm">
          Informe seus dados abaixo
        </p>
      </div>

      <form @submit.prevent="handleReset" class="space-y-5">

        <div v-if="errorMsg" class="bg-red-50 text-red-600 p-3 rounded-lg text-sm text-center">
          {{ errorMsg }}
        </div>

        <div v-if="successMsg" class="bg-green-50 text-green-700 p-3 rounded-lg text-sm text-center">
          {{ successMsg }}
        </div>

        <!-- EMAIL -->
        <div>
          <label class="text-sm text-gray-600">E-mail</label>
          <input v-model="email" type="email" required
                 class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3" />
        </div>

        <!-- NOVA SENHA -->
        <div>
          <label class="text-sm text-gray-600">Nova senha</label>
          <input v-model="password" type="password" required
                 class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3" />
        </div>

        <!-- CONFIRMAR -->
        <div>
          <label class="text-sm text-gray-600">Confirmar senha</label>
          <input v-model="confirmPassword" type="password" required
                 class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3" />
        </div>

        <button type="submit"
                class="w-full bg-green-700 text-white py-3 rounded-xl">
          {{ loading ? 'Salvando...' : 'Atualizar senha' }}
        </button>

      </form>
    </div>
  </div>
</template>
