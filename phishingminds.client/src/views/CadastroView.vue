<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()
const currentStep = ref(1)

const form = ref({
  Mail: '',
  CNPJ: '',
  Nm_Empresa: '',
  Nm_Dono: '',
  Senha: '',
  IdPlano: 1 // Default
})

onMounted(() => {
  if (route.query.plano) {
    form.value.IdPlano = parseInt(route.query.plano, 10) || 1
  }
})

const errorMsg = ref('')
const loading = ref(false)

const formatCNPJ = (val) => {
  let x = val.replace(/\D/g, '').match(/(\d{0,2})(\d{0,3})(\d{0,3})(\d{0,4})(\d{0,2})/)
  if (!x) return val
  return !x[2] ? x[1] : x[1] + '.' + x[2] + (x[3] ? '.' + x[3] : '') + (x[4] ? '/' + x[4] : '') + (x[5] ? '-' + x[5] : '')
}

const handleCNPJInput = (e) => {
  form.value.CNPJ = formatCNPJ(e.target.value)
}

const isValidEmail = (email) => {
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)
}

const nextStep = () => {
  errorMsg.value = ''
  
  if (currentStep.value === 1) {
    if (!isValidEmail(form.value.Mail)) {
      errorMsg.value = 'Por favor, insira um e-mail válido.'
      return
    }
  } else if (currentStep.value === 2) {
    if (form.value.CNPJ.replace(/\D/g, '').length !== 14) {
      errorMsg.value = 'Por favor, insira um CNPJ válido com 14 dígitos.'
      return
    }
  }
  
  currentStep.value++
}

const prevStep = () => {
  errorMsg.value = ''
  currentStep.value--
}

const handleSubmit = async () => {
  errorMsg.value = ''
  loading.value = true
  
  try {
    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    })

    if (!response.ok) {
      const errorData = await response.text()
      throw new Error(errorData || 'Falha ao realizar cadastro')
    }

    // Sucesso, redireciona para login
    router.push('/login')
  } catch (err) {
    errorMsg.value = err.message || 'Erro ao comunicar com o servidor.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="min-h-screen flex">

    <!-- LADO ESQUERDO -->
    <div class="hidden md:flex w-1/2 bg-gradient-to-br from-green-900 to-green-700 text-white p-12 flex-col justify-center">
      <div class="max-w-md">
        <!-- LOGO -->
        <div class="mb-10 flex items-center gap-3">
          <span class="text-xl font-semibold tracking-wide">PHISHING MINDS</span>
        </div>
        <h1 class="text-4xl font-bold leading-tight mb-6">
          Comece a proteger a sua organização hoje mesmo.
        </h1>
        <p class="text-green-100">
          Crie sua conta para treinar seus colaboradores e mitigar riscos cibernéticos com simulações inteligentes.
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

        <!-- Header -->
        <div class="text-center mb-6 mt-4">
          <h2 class="text-2xl font-bold text-green-900">Criar Conta Corporativa</h2>
          <p class="text-gray-500 text-sm mt-1">Passo {{ currentStep }} de 3</p>
          
          <!-- Barra de Progresso -->
          <div class="flex gap-2 mt-4">
            <div class="h-2 flex-1 rounded-full transition-all" :class="currentStep >= 1 ? 'bg-green-600' : 'bg-gray-200'"></div>
            <div class="h-2 flex-1 rounded-full transition-all" :class="currentStep >= 2 ? 'bg-green-600' : 'bg-gray-200'"></div>
            <div class="h-2 flex-1 rounded-full transition-all" :class="currentStep >= 3 ? 'bg-green-600' : 'bg-gray-200'"></div>
          </div>
        </div>

        <div v-if="errorMsg" class="bg-red-50 text-red-600 p-3 rounded-lg text-sm text-center mb-5">
          {{ errorMsg }}
        </div>

        <!-- FORM -->
        <form @submit.prevent="currentStep === 3 ? handleSubmit() : nextStep()" class="space-y-5">

          <!-- Passo 1: Email -->
          <div v-if="currentStep === 1">
            <label class="text-sm text-gray-600">E-mail Corporativo</label>
            <input v-model="form.Mail"
                   type="email"
                   class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                   placeholder="admin@empresa.com" />
          </div>

          <!-- Passo 2: CNPJ -->
          <div v-if="currentStep === 2">
            <label class="text-sm text-gray-600">CNPJ da Empresa</label>
            <input v-model="form.CNPJ"
                   @input="handleCNPJInput"
                   type="text"
                   maxlength="18"
                   class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                   placeholder="00.000.000/0000-00" />
          </div>

          <!-- Passo 3: Dados da Empresa e Senha -->
          <div v-if="currentStep === 3" class="space-y-4">
            <div>
              <label class="text-sm text-gray-600">Nome da Empresa</label>
              <input v-model="form.Nm_Empresa"
                     type="text"
                     required
                     class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                     placeholder="Sua Empresa LTDA" />
            </div>
            
            <div>
              <label class="text-sm text-gray-600">Nome do Responsável</label>
              <input v-model="form.Nm_Dono"
                     type="text"
                     required
                     class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                     placeholder="João Silva" />
            </div>

            <div>
              <label class="text-sm text-gray-600">Senha de Acesso</label>
              <input v-model="form.Senha"
                     type="password"
                     required
                     class="w-full mt-1 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500"
                     placeholder="••••••••" />
            </div>
          </div>

          <!-- Botoes -->
          <div class="flex gap-3 pt-2">
            <button v-if="currentStep > 1" 
                    type="button" 
                    @click="prevStep"
                    class="w-1/3 bg-gray-200 hover:bg-gray-300 text-gray-700 py-3 rounded-xl font-medium transition">
              Voltar
            </button>
            
            <button type="submit"
                    :disabled="loading"
                    class="flex-1 bg-green-700 hover:bg-green-800 text-white py-3 rounded-xl font-medium shadow-md transition">
              <span v-if="currentStep < 3">Continuar →</span>
              <span v-else-if="loading">Criando Conta...</span>
              <span v-else>Finalizar Cadastro</span>
            </button>
          </div>

          <p class="text-xs text-center text-gray-500 mt-4">
            Já possui uma conta?
            <span @click="$router.push('/login')" class="text-green-700 cursor-pointer hover:underline">
              Fazer login
            </span>
          </p>

        </form>
      </div>
    </div>
  </div>
</template>
