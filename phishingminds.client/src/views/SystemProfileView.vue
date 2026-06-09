<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'

const userEmpresaId = ref(null)
const isDevAdmin = ref(false)
const isPessoa = ref(false)
const currentUser = ref(null)
const loading = ref(true)

const isModalOpen = ref(false)

const profileData = ref({
  idEmpresa: 0,
  nm_Empresa: '',
  nm_Dono: '',
  mail: '',
  cnpj: '',
  idPlano: 1,
  ativo: true
})

const empresaForm = ref({ ...profileData.value })
const router = useRouter()

const toast = ref({
  show: false,
  message: '',
  type: 'success'
})

const showToast = (message, type = 'success') => {
  toast.value.message = message
  toast.value.type = type
  toast.value.show = true
  setTimeout(() => {
    toast.value.show = false
  }, 4000)
}

const isDeleteConfirmOpen = ref(false)

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
      profileData.value = { ...data }
    } else {
      // Fallback for DEV if not in DB
      const userStr = localStorage.getItem('user')
      if (userStr) {
        const user = JSON.parse(userStr)
        profileData.value.nm_Empresa = user.nm_Empresa || 'Administração Central'
        profileData.value.nm_Dono = user.nm_Dono || 'Usuário DEV'
        profileData.value.mail = user.mail || 'admin@phishingminds.com'
        profileData.value.cnpj = '00.000.000/0000-00'
      }
    }
  } catch (error) {
    console.error("Erro ao buscar perfil", error)
  } finally {
    loading.value = false
  }
}

const openModal = () => {
  empresaForm.value = { ...profileData.value }
  isModalOpen.value = true
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
      isModalOpen.value = false
      fetchProfile()
      showToast('Atualizado com sucesso!', 'success')
    } else {
      showToast('Erro ao atualizar perfil.', 'error')
    }
  } catch (error) {
    console.error("Erro ao salvar perfil", error)
    showToast('Erro ao atualizar perfil.', 'error')
  }
}

const deleteAccount = async () => {
  try {
    const res = await fetch(`/api/Empresa/${userEmpresaId.value}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${getToken()}` }
    })
    if (res.ok) {
      isDeleteConfirmOpen.value = false
      isModalOpen.value = false
      showToast('Excluido com sucesso!', 'success')
      setTimeout(() => {
        localStorage.removeItem('token')
        localStorage.removeItem('user')
        router.push('/login')
      }, 1500)
    } else {
      const errText = await res.text()
      showToast('Erro ao excluir conta: ' + errText, 'error')
    }
  } catch (e) {
    console.error(e)
    showToast('Erro ao excluir conta.', 'error')
  }
}

onMounted(() => {
  const userStr = localStorage.getItem('user')
  if (userStr) {
    const user = JSON.parse(userStr)

    currentUser.value = user

    isDevAdmin.value =
      user.idEmpresa === 1 &&
      user.isEmpresa === true

    isPessoa.value = user.isPessoa === true

    userEmpresaId.value = user.idEmpresa

    fetchProfile()
  }
})
</script>

<template>
  <MainLayout>
    <!-- responsividade -->
    <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-3xl md:text-4xl font-bold text-green-900">Meu Perfil</h2>
        <p class="text-gray-500 mt-1">Gerencie as informações da sua organização.</p>
      </div>
      <button v-if="!isPessoa"
              @click="openModal"
              class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto flex items-center justify-center gap-2">
        <svg xmlns="http://www.w3.org/2000/svg"
             class="h-5 w-5"
             fill="none"
             viewBox="0 0 24 24"
             stroke="currentColor">
          <path stroke-linecap="round"
                stroke-linejoin="round"
                stroke-width="2"
                d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
        </svg>
        Editar Perfil
      </button>
    </div>

    <div v-if="loading" class="text-center py-10 text-gray-500">
      Carregando...
    </div>
    <div v-else-if="!isPessoa"
         class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
      <!-- responsividade -->
      <div class="p-8 border-b border-gray-100 flex flex-col sm:flex-row items-center gap-6 text-center sm:text-left bg-gray-50/50">
        <div class="w-24 h-24 bg-green-700 rounded-full flex-shrink-0 flex items-center justify-center text-white text-3xl font-bold shadow-md">
          {{ profileData.nm_Empresa ? profileData.nm_Empresa.charAt(0).toUpperCase() : 'O' }}
        </div>
        <div class="flex-1">
          <h3 class="text-2xl font-bold text-gray-800">{{ profileData.nm_Empresa || 'Organização' }}</h3>
          <p class="text-gray-500">{{ profileData.mail }}</p>
          <div class="mt-3 flex flex-wrap gap-2 justify-center sm:justify-start">
            <span class="inline-block text-xs font-bold uppercase tracking-wider text-green-800 bg-green-100 px-3 py-1 rounded-full">
              {{ isDevAdmin ? 'Global Admin' : 'Org Admin' }}
            </span>
            <span v-if="profileData.idPlano" class="inline-block text-xs font-bold uppercase tracking-wider text-blue-800 bg-blue-100 px-3 py-1 rounded-full">
              Plano ID: {{ profileData.idPlano }}
            </span>
          </div>
        </div>
      </div>

      <div class="p-8">
        <h4 class="text-lg font-semibold text-gray-800 mb-6">Informações Detalhadas</h4>
        <!-- responsividade -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div>
            <p class="text-sm font-medium text-gray-500 mb-1">Nome do Responsável</p>
            <p class="text-base font-semibold text-gray-900">{{ profileData.nm_Dono || 'Não informado' }}</p>
          </div>
          <div>
            <p class="text-sm font-medium text-gray-500 mb-1">E-mail Administrativo</p>
            <p class="text-base font-semibold text-gray-900">{{ profileData.mail || 'Não informado' }}</p>
          </div>
          <div>
            <p class="text-sm font-medium text-gray-500 mb-1">Nome da Organização</p>
            <p class="text-base font-semibold text-gray-900">{{ profileData.nm_Empresa || 'Não informado' }}</p>
          </div>
          <div>
            <p class="text-sm font-medium text-gray-500 mb-1">CNPJ</p>
            <p class="text-base font-semibold text-gray-900">{{ profileData.cnpj || 'Não informado' }}</p>
          </div>
        </div>
      </div>
    </div>
    <!-- PERFIL FUNCIONÁRIO -->
    <div v-else
         class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
      <div class="p-8 border-b border-gray-100 flex items-center gap-6 bg-gray-50/50">
        <div class="w-24 h-24 bg-green-700 rounded-full flex items-center justify-center text-white text-3xl font-bold">
          {{ currentUser?.nome?.charAt(0)?.toUpperCase() }}
        </div>

        <div>
          <h3 class="text-2xl font-bold text-gray-800">
            {{ currentUser?.nome }}
          </h3>

          <p class="text-gray-500">
            {{ currentUser?.email }}
          </p>

          <span class="inline-block mt-3 text-xs font-bold uppercase tracking-wider text-green-800 bg-green-100 px-3 py-1 rounded-full">
            Funcionário
          </span>
        </div>
      </div>

      <div class="p-8">
        <h4 class="text-lg font-semibold text-gray-800 mb-6">
          Informações Detalhadas
        </h4>

        <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div>
            <p class="text-sm text-gray-500">Nome</p>
            <p class="font-semibold">{{ currentUser?.nome }}</p>
          </div>

          <div>
            <p class="text-sm text-gray-500">Email</p>
            <p class="font-semibold">{{ currentUser?.email }}</p>
          </div>

          <div>
            <p class="text-sm text-gray-500">Empresa</p>
            <p class="font-semibold">{{ profileData.nm_Empresa }}</p>
          </div>

          <div>
            <p class="text-sm text-gray-500">ID Empresa</p>
            <p class="font-semibold">{{ currentUser?.idEmpresa }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Edit Modal -->
    <!-- responsividade -->
    <div v-if="isModalOpen" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4 overflow-y-auto">
      <div class="bg-white rounded-3xl w-full max-w-2xl shadow-xl overflow-hidden my-auto">
        <div class="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50">
          <h3 class="text-xl font-bold text-gray-800">Editar Perfil</h3>
          <button @click="isModalOpen = false" class="text-gray-400 hover:text-gray-600">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <div class="p-6">
          <form @submit.prevent="saveProfile" class="space-y-6">
            <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Nome do Responsável</label>
                <input v-model="empresaForm.nm_Dono" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" required />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">E-mail Administrativo</label>
                <input v-model="empresaForm.mail" type="email" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" required />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Nome da Organização</label>
                <input v-model="empresaForm.nm_Empresa" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" required />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">CNPJ</label>
                <input v-model="empresaForm.cnpj" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" placeholder="00.000.000/0000-00" />
              </div>
            </div>

            <div class="pt-6 flex justify-between items-center border-t border-gray-100">
              <button type="button" @click="isDeleteConfirmOpen = true" class="bg-red-50 hover:bg-red-100 text-red-600 px-5 py-2.5 rounded-xl font-semibold transition-all">
                Excluir Conta
              </button>
              <div class="flex gap-3">
                <button type="button" @click="isModalOpen = false" class="px-5 py-2.5 text-gray-600 font-medium hover:bg-gray-100 rounded-xl transition-colors">
                  Cancelar
                </button>
                <button type="submit" class="bg-green-700 hover:bg-green-800 text-white px-6 py-2.5 rounded-xl font-medium shadow-sm transition-colors">
                  Salvar Alterações
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>
    </div>
    <!-- Confirm Delete Account Modal -->
    <div v-if="isDeleteConfirmOpen" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4 overflow-y-auto">
      <div class="bg-white rounded-3xl w-full max-w-sm shadow-xl p-6 text-center my-auto">
        <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-red-100 mb-4">
          <svg class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
        </div>
        <h3 class="text-xl font-bold text-gray-900 mb-2">Excluir Conta</h3>
        <p class="text-gray-500 mb-6">Tem certeza que deseja excluir permanentemente a sua conta corporativa (<strong>{{ profileData.nm_Empresa }}</strong>) e TODOS os dados vinculados (setores, colaboradores, campanhas)? Essa ação é irreversível.</p>
        <div class="flex justify-center gap-3">
          <button @click="isDeleteConfirmOpen = false" class="px-4 py-2 text-gray-600 font-medium hover:bg-gray-100 rounded-xl">Cancelar</button>
          <button @click="deleteAccount" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-medium rounded-xl">Excluir permanentemente</button>
        </div>
      </div>
    </div>

    <!-- Beautiful Floating Toast Notification -->
    <Transition
      enter-active-class="transform ease-out duration-300 transition"
      enter-from-class="translate-y-2 opacity-0 sm:translate-y-0 sm:translate-x-2"
      enter-to-class="translate-y-0 opacity-100 sm:translate-x-0"
      leave-active-class="transition ease-in duration-200"
      leave-from-class="opacity-100"
      leave-to-class="opacity-0"
    >
      <div v-if="toast.show" class="fixed top-5 right-5 left-5 sm:left-auto sm:w-96 z-[100] flex bg-white rounded-2xl shadow-xl border border-gray-100 overflow-hidden">
        <div :class="[toast.type === 'success' ? 'bg-green-500' : 'bg-red-500', 'w-2']"></div>
        <div class="p-4 flex items-center justify-between flex-1 gap-4">
          <div class="flex items-center gap-3">
            <!-- Check Icon for Success -->
            <svg v-if="toast.type === 'success'" class="w-6 h-6 text-green-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <!-- Error Icon for Error -->
            <svg v-else class="w-6 h-6 text-red-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z"></path>
            </svg>
            <p class="text-sm font-semibold text-gray-800">{{ toast.message }}</p>
          </div>
          <button @click="toast.show = false" class="text-gray-400 hover:text-gray-600 transition-colors flex-shrink-0">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>
      </div>
    </Transition>
  </MainLayout>
</template>
