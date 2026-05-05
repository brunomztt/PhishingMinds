<script setup>
import { ref, onMounted } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'

const userEmpresaId = ref(null)
const isDevAdmin = ref(false)
const loading = ref(true)

const empresaForm = ref({
  idEmpresa: 0,
  nm_Empresa: '',
  nm_Dono: '',
  mail: '',
  cnpj: '',
  idPlano: 1, // default
  ativo: true
})

const getToken = () => localStorage.getItem('token')

const fetchProfile = async () => {
  if (!userEmpresaId.value) return
  loading.value = true
  try {
    const res = await fetch(`/api/Empresa/${userEmpresaId.value}`, {
      headers: { 'Authorization': `Bearer ${getToken()}` }
    })
    if (res.ok) {
      const data = await res.json()
      empresaForm.value = { ...data }
    }
  } catch (error) {
    console.error("Erro ao buscar perfil", error)
  } finally {
    loading.value = false
  }
}

const saveProfile = async () => {
  try {
    const res = await fetch(`/api/Empresa/${userEmpresaId.value}`, {
      method: 'PUT',
      headers: { 
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getToken()}`
      },
      body: JSON.stringify(empresaForm.value)
    })
    if (res.ok) {
      alert('Perfil atualizado com sucesso!')
      fetchProfile()
    } else {
      alert('Erro ao atualizar perfil.')
    }
  } catch (error) {
    console.error("Erro ao salvar perfil", error)
    alert('Erro ao atualizar perfil.')
  }
}

onMounted(() => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)
    isDevAdmin.value = user.idEmpresa === 1
    userEmpresaId.value = user.idEmpresa
    fetchProfile()
  }
})
</script>

<template>
  <MainLayout>
    <div class="mb-6">
      <h2 class="text-3xl md:text-4xl font-bold text-green-900">Meu Perfil</h2>
      <p class="text-gray-500 mt-1">Gerencie as informações da sua organização.</p>
    </div>

    <div v-if="loading" class="text-center py-10 text-gray-500">
      Carregando...
    </div>

    <div v-else class="bg-white rounded-3xl shadow-sm overflow-hidden max-w-3xl mt-8">
      <div class="p-8 border-b border-gray-100 flex flex-col sm:flex-row items-center gap-6 text-center sm:text-left">
        <div class="w-24 h-24 bg-green-700 rounded-full flex-shrink-0 flex items-center justify-center text-white text-3xl font-bold shadow-md">
          {{ empresaForm.nm_Empresa ? empresaForm.nm_Empresa.charAt(0).toUpperCase() : 'O' }}
        </div>
        <div>
          <h3 class="text-2xl font-bold text-gray-800">{{ empresaForm.nm_Dono || 'Usuário' }}</h3>
          <p class="text-gray-500">{{ empresaForm.mail }}</p>
          <span class="inline-block mt-2 text-xs font-bold uppercase tracking-wider text-green-800 bg-green-100 px-3 py-1 rounded-full">
            {{ isDevAdmin ? 'Global Admin' : 'Org Admin' }}
          </span>
        </div>
      </div>

      <div class="p-8">
        <form @submit.prevent="saveProfile" class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Nome do Responsável</label>
              <input v-model="empresaForm.nm_Dono" type="text" class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" required />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">E-mail Administrativo</label>
              <input v-model="empresaForm.mail" type="email" class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" required />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Nome da Organização</label>
              <input v-model="empresaForm.nm_Empresa" type="text" class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" required />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">CNPJ</label>
              <input v-model="empresaForm.cnpj" type="text" class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" placeholder="00.000.000/0000-00" />
            </div>
          </div>

          <div class="pt-6 flex justify-end">
            <button type="submit" class="bg-green-700 hover:bg-green-800 text-white px-6 py-3 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto">
              Salvar Alterações
            </button>
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>
