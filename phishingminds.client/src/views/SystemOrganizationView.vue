<script setup>
import { ref, onMounted, computed } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'

const isDevAdmin = ref(false)
const isPessoa = ref(false)
const userEmpresaId = ref(null)

const setores = ref([])
const funcionarios = ref([])
const loadingSetores = ref(false)
const loadingFuncionarios = ref(false)

const funcionariosPhishing = ref([])
const loadingPhishing = ref(false)
const funcionariosPhishingFiltrados = computed(() => {
  const userStr = localStorage.getItem('user')

  if (!userStr)
    return funcionariosPhishing.value

  const user = JSON.parse(userStr)

  // Admin continua vendo tudo
  if (!user.isPessoa)
    return funcionariosPhishing.value

  // Colaborador vê apenas ele mesmo
  return funcionariosPhishing.value.filter(
    f =>
      f.Email?.toLowerCase() ===
      user.email?.toLowerCase()
  )
})



// Modals
const isSetorModalOpen = ref(false)
const isFuncionarioModalOpen = ref(false)
const isDeleteSetorModalOpen = ref(false)
const isDeleteFuncionarioModalOpen = ref(false)

// Forms
const setorForm = ref({ idSetor: 0, nm_Setor: '' })
const funcionarioForm = ref({ idUser: 0, nome: '', email: '', idSetor: null })

const itemToDelete = ref(null)

// Search & Pagination
const searchSetor = ref('')
const searchFuncionario = ref('')
const currentPageSetor = ref(1)
const currentPageFuncionario = ref(1)
const itemsPerPage = ref(5)

const fetchFuncionariosPhishing = async () => {
  if (!userEmpresaId.value) return

  loadingPhishing.value = true

  try {
    const res = await fetch(
      `/api/Pessoa/caidos-phishing/${userEmpresaId.value}`,
      {
        headers: {
          Authorization: `Bearer ${getToken()}`
        }
      }
    )

    if (res.ok) {
      funcionariosPhishing.value = await res.json()
    }
  }
  catch (e) {
    console.error(e)
  }
  finally {
    loadingPhishing.value = false
  }
}

const filteredSetores = computed(() => {
  let result = setores.value
  if (searchSetor.value) {
    result = result.filter(s => s.nm_Setor.toLowerCase().includes(searchSetor.value.toLowerCase()))
  }
  return result
})

const paginatedSetores = computed(() => {
  const start = (currentPageSetor.value - 1) * itemsPerPage.value
  return filteredSetores.value.slice(start, start + itemsPerPage.value)
})

const totalPagesSetor = computed(() => Math.ceil(filteredSetores.value.length / itemsPerPage.value) || 1)

const filteredFuncionarios = computed(() => {
  let result = funcionarios.value

  const userStr = localStorage.getItem('user')

  if (userStr) {
    const user = JSON.parse(userStr)

    // colaborador comum só vê a si mesmo
    if (user.isPessoa === true) {
      result = result.filter(
        f =>
          f.email?.toLowerCase() === user.email?.toLowerCase()
      )
    }
  }

  if (searchFuncionario.value) {
    result = result.filter(
      f =>
        f.nome?.toLowerCase().includes(searchFuncionario.value.toLowerCase()) ||
        f.email?.toLowerCase().includes(searchFuncionario.value.toLowerCase())
    )
  }

  return result
})

const paginatedFuncionarios = computed(() => {
  const start = (currentPageFuncionario.value - 1) * itemsPerPage.value
  return filteredFuncionarios.value.slice(start, start + itemsPerPage.value)
})

const totalPagesFuncionario = computed(() => Math.ceil(filteredFuncionarios.value.length / itemsPerPage.value) || 1)

const getToken = () => localStorage.getItem('token')

const fetchSetores = async () => {
  if (!userEmpresaId.value) return
  loadingSetores.value = true
  try {
    const res = await fetch(`/api/Setor/empresa/${userEmpresaId.value}`, {
      headers: { 'Authorization': `Bearer ${getToken()}` }
    })
    if (res.ok) {
      setores.value = await res.json()
    }
  } catch (e) {
    console.error(e)
  } finally {
    loadingSetores.value = false
  }
}

const fetchFuncionarios = async () => {
  if (!userEmpresaId.value) return
  loadingFuncionarios.value = true
  try {
    const res = await fetch(`/api/Pessoa/empresa/${userEmpresaId.value}`, {
      headers: { 'Authorization': `Bearer ${getToken()}` }
    })
    if (res.ok) {
      funcionarios.value = await res.json()
    }
  } catch (e) {
    console.error(e)
  } finally {
    loadingFuncionarios.value = false
  }
}

const openSetorModal = (setor = null) => {
  setorForm.value = setor ? { ...setor } : { idSetor: 0, nm_Setor: '' }
  isSetorModalOpen.value = true
}

const openFuncionarioModal = (funcionario = null) => {
  funcionarioForm.value = funcionario ? { ...funcionario } : { idUser: 0, nome: '', email: '', idSetor: null }
  isFuncionarioModalOpen.value = true
}

const saveSetor = async () => {
  const isEdit = setorForm.value.idSetor > 0
  const url = isEdit ? `/api/Setor/${setorForm.value.idSetor}` : '/api/Setor'
  const method = isEdit ? 'PUT' : 'POST'

  const payload = { ...setorForm.value, idEmpresa: userEmpresaId.value, idGestor: null }

  try {
    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${getToken()}` },
      body: JSON.stringify(payload)
    })
    if (res.ok) {
      isSetorModalOpen.value = false
      fetchSetores()
    }
  } catch (e) {
    console.error(e)
  }
}

const saveFuncionario = async () => {
  const isEdit = funcionarioForm.value.idUser > 0
  const url = isEdit ? `/api/Pessoa/${funcionarioForm.value.idUser}` : '/api/Pessoa'
  const method = isEdit ? 'PUT' : 'POST'

  const payload = { 
    ...funcionarioForm.value, 
    idEmpresa: userEmpresaId.value,
    ativo: true,
    idCargo: null,
    idGestor: null,
    phishingScore: 0
  }

  try {
    const res = await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${getToken()}` },
      body: JSON.stringify(payload)
    })
    if (res.ok) {
      isFuncionarioModalOpen.value = false
      fetchFuncionarios()
    }
  } catch (e) {
    console.error(e)
  }
}

const confirmDeleteSetor = (setor) => {
  itemToDelete.value = setor
  isDeleteSetorModalOpen.value = true
}

const deleteSetor = async () => {
  try {
    const res = await fetch(`/api/Setor/${itemToDelete.value.idSetor}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${getToken()}` }
    })
    if (res.ok) {
      isDeleteSetorModalOpen.value = false
      fetchSetores()
    }
  } catch (e) {
    console.error(e)
  }
}

const confirmDeleteFuncionario = (funcionario) => {
  itemToDelete.value = funcionario
  isDeleteFuncionarioModalOpen.value = true
}

const deleteFuncionario = async () => {
  try {
    const res = await fetch(`/api/Pessoa/${itemToDelete.value.idUser}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${getToken()}` }
    })
    if (res.ok) {
      isDeleteFuncionarioModalOpen.value = false
      fetchFuncionarios()
    }
  } catch (e) {
    console.error(e)
  }
}

  onMounted(() => {
    const userStr = localStorage.getItem('user')

    if (userStr) {
      const user = JSON.parse(userStr)

      isDevAdmin.value =
        user.idEmpresa === 1 &&
        user.isEmpresa === true

      isPessoa.value =
        user.isPessoa === true

      userEmpresaId.value = user.idEmpresa

      if (!isDevAdmin.value) {
        fetchSetores()
        fetchFuncionarios()
        fetchFuncionariosPhishing()

        // marca o momento em que o gestor visualizou a tela
        localStorage.setItem(
          'ultimaVisualizacaoPhishing',
          Date.now().toString()
        )
      }
    }
  })
</script>

<template>
  <MainLayout>
    <div v-if="isDevAdmin">
      <!-- Dev Admin View: Unchanged -->
      <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 class="text-3xl md:text-4xl font-bold text-green-900">Lista de Organizações</h2>
          <p class="text-gray-500 mt-1">Gerencie todos os clientes conectados à plataforma Phishing Minds.</p>
        </div>
        <button class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto">
          Cadastrar Cliente
        </button>
      </div>

      <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
        <div class="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
          <h3 class="text-xl font-semibold text-gray-800">Clientes Ativos</h3>
        </div>
        <div class="p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse min-w-[700px]">
              <thead>
                <tr class="text-gray-500 text-sm border-b border-gray-100">
                  <th class="pb-3 font-medium">Empresa</th>
                  <th class="pb-3 font-medium">Data Cadastro</th>
                  <th class="pb-3 font-medium">Plano Atual</th>
                  <th class="pb-3 font-medium">Score Médio</th>
                  <th class="pb-3 font-medium">Ações</th>
                </tr>
              </thead>
              <tbody>
                <tr class="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                  <td class="py-4 font-semibold text-gray-800">TechCorp Solutions</td>
                  <td class="py-4 text-gray-500">10/01/2026</td>
                  <td class="py-4"><span class="bg-blue-50 text-blue-700 text-xs px-2 py-1 rounded-md font-semibold">Enterprise Plus</span></td>
                  <td class="py-4 font-medium text-green-600">B+</td>
                  <td class="py-4">
                    <button class="text-green-700 hover:underline text-sm font-medium">Gerenciar</button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>

    <div v-else>
      <!-- Org Admin View -->
      <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h2 class="text-3xl md:text-4xl font-bold text-green-900">Estrutura Organizacional</h2>
          <p class="text-gray-500 mt-1">Gerencie departamentos e funcionários da sua empresa.</p>
        </div>
      </div>

      <!-- Setores Section -->
      <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
        <div class="p-6 border-b border-gray-100 flex flex-col sm:flex-row justify-between items-center gap-4 bg-gray-50/50">
          <h3 class="text-xl font-semibold text-gray-800">Setores</h3>
          <div class="flex gap-4 w-full sm:w-auto">
            <input v-model="searchSetor" @input="currentPageSetor = 1" type="text" placeholder="Buscar setor..." class="w-full sm:w-64 px-4 py-2 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-green-500" />
            <button v-if="!isPessoa"
                    @click="openSetorModal()" class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors whitespace-nowrap">
              Novo Setor
            </button>
          </div>
        </div>

        <div class="p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse min-w-[500px]">
              <thead>
                <tr class="text-gray-500 text-sm border-b border-gray-100">
                  <th class="pb-3 font-medium">ID</th>
                  <th class="pb-3 font-medium">Nome do Setor</th>
                  <th v-if="!isPessoa"
                      class="pb-3 font-medium w-32 text-center">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="loadingSetores" class="border-b border-gray-50"><td colspan="3" class="py-4 text-center text-gray-500">Carregando...</td></tr>
                <tr v-else-if="paginatedSetores.length === 0" class="border-b border-gray-50"><td colspan="3" class="py-4 text-center text-gray-500">Nenhum setor encontrado.</td></tr>
                <tr v-else v-for="setor in paginatedSetores" :key="setor.idSetor" class="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                  <td class="py-4 font-medium text-gray-500">#{{ setor.idSetor }}</td>
                  <td class="py-4 font-semibold text-gray-800">{{ setor.nm_Setor }}</td>
                  <td v-if="!isPessoa"
                      class="py-4 flex justify-center gap-3">
                    <button @click="openSetorModal(setor)" class="text-blue-600 hover:text-blue-800 transition-colors" title="Editar">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" /></svg>
                    </button>
                    <button @click="confirmDeleteSetor(setor)" class="text-red-500 hover:text-red-700 transition-colors" title="Excluir">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <!-- Pagination -->
          <div class="flex justify-between items-center mt-6 text-sm text-gray-500">
            <span>Mostrando {{ paginatedSetores.length }} de {{ filteredSetores.length }} setores</span>
            <div class="flex gap-2">
              <button :disabled="currentPageSetor === 1" @click="currentPageSetor--" class="px-3 py-1 border rounded hover:bg-gray-50 disabled:opacity-50">Anterior</button>
              <button :disabled="currentPageSetor === totalPagesSetor" @click="currentPageSetor++" class="px-3 py-1 border rounded hover:bg-gray-50 disabled:opacity-50">Próxima</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Funcionarios Section -->
      <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
        <div class="p-6 border-b border-gray-100 flex flex-col sm:flex-row justify-between items-center gap-4 bg-gray-50/50">
          <h3 class="text-xl font-semibold text-gray-800">Funcionários</h3>
          <div class="flex gap-4 w-full sm:w-auto">
            <input v-model="searchFuncionario" @input="currentPageFuncionario = 1" type="text" placeholder="Buscar funcionário..." class="w-full sm:w-64 px-4 py-2 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-green-500" />
            <button v-if="!isPessoa"
                    @click="openFuncionarioModal()" class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors whitespace-nowrap">
              Novo Funcionário
            </button>
          </div>
        </div>

        <div class="p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse min-w-[700px]">
              <thead>
                <tr class="text-gray-500 text-sm border-b border-gray-100">
                  <th class="pb-3 font-medium">Nome</th>
                  <th class="pb-3 font-medium">Email</th>
                  <th class="pb-3 font-medium">Setor</th>
                  <th v-if="!isPessoa"
                      class="pb-3 font-medium w-32 text-center">
                    Ações
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr v-if="loadingFuncionarios" class="border-b border-gray-50"><td colspan="4" class="py-4 text-center text-gray-500">Carregando...</td></tr>
                <tr v-else-if="paginatedFuncionarios.length === 0" class="border-b border-gray-50"><td colspan="4" class="py-4 text-center text-gray-500">Nenhum funcionário encontrado.</td></tr>
                <tr v-else v-for="func in paginatedFuncionarios" :key="func.idUser" class="border-b border-gray-50 hover:bg-gray-50 transition-colors">
                  <td class="py-4 font-semibold text-gray-800">{{ func.nome }}</td>
                  <td class="py-4 text-gray-500">{{ func.email }}</td>
                  <td class="py-4"><span class="bg-green-50 text-green-700 text-xs px-2 py-1 rounded-md font-semibold">{{ func.nm_Setor || 'Sem Setor' }}</span></td>
                  <td v-if="!isPessoa"
                      class="py-4 flex justify-center gap-3">
                    <button @click="openFuncionarioModal(func)" class="text-blue-600 hover:text-blue-800 transition-colors" title="Editar">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" /></svg>
                    </button>
                    <button @click="confirmDeleteFuncionario(func)" class="text-red-500 hover:text-red-700 transition-colors" title="Excluir">
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" /></svg>
                    </button>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
          <!-- Pagination -->
          <div class="flex justify-between items-center mt-6 text-sm text-gray-500">
            <span>Mostrando {{ paginatedFuncionarios.length }} de {{ filteredFuncionarios.length }} funcionários</span>
            <div class="flex gap-2">
              <button :disabled="currentPageFuncionario === 1" @click="currentPageFuncionario--" class="px-3 py-1 border rounded hover:bg-gray-50 disabled:opacity-50">Anterior</button>
              <button :disabled="currentPageFuncionario === totalPagesFuncionario" @click="currentPageFuncionario++" class="px-3 py-1 border rounded hover:bg-gray-50 disabled:opacity-50">Próxima</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Funcionários que Caíram em Phishing -->
      <div class="bg-white rounded-3xl shadow-sm overflow-hidden mt-8">
        <div class="p-6 border-b border-gray-100 bg-red-50">
          <h3 class="text-xl font-semibold text-red-700">
            ⚠ Funcionários que Caíram em Phishing
          </h3>
          <p class="text-sm text-gray-500 mt-1">
            Usuários que clicaram em links de campanhas de phishing.
          </p>
        </div>

        <div class="p-6">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse min-w-[700px]">
              <thead>
                <tr class="text-gray-500 text-sm border-b border-gray-100">
                  <th class="pb-3 font-medium">Nome</th>
                  <th class="pb-3 font-medium">Email</th>
                  <th class="pb-3 font-medium">Setor</th>
                  <th class="pb-3 font-medium">Campanha</th>
                </tr>
              </thead>

              <tbody>
                <tr v-if="loadingPhishing">
                  <td colspan="4" class="py-4 text-center text-gray-500">
                    Carregando...
                  </td>
                </tr>

                <tr v-else-if="funcionariosPhishing.length === 0">
                  <td colspan="4" class="py-4 text-center text-gray-500">
                    Nenhum funcionário caiu em phishing.
                  </td>
                </tr>

                <tr v-else
                    v-for="func in funcionariosPhishingFiltrados"
                    :key="func.idUser"
                    class="border-b border-gray-50 hover:bg-red-50 transition-colors">
                  <td class="py-4 font-semibold text-gray-800">
                    {{ func.Nome }}
                  </td>

                  <td class="py-4 text-gray-500">
                    {{ func.Email }}
                  </td>

                  <td class="py-4">
                    <span class="bg-gray-100 text-gray-700 text-xs px-2 py-1 rounded-md font-semibold">
                      {{ func.Nm_Setor || 'Sem Setor' }}
                    </span>
                  </td>

                  <td class="py-4">
                    <span class="bg-red-100 text-red-700 text-xs px-2 py-1 rounded-md font-semibold">
                      {{ func.NomeCampanha }}
                    </span>
                  </td>
                </tr>
              </tbody>
            </table>
          </div>

          <div class="mt-4 text-sm text-gray-500">
            Total de funcionários comprometidos:
            <strong>{{ funcionariosPhishingFiltrados.length }}</strong>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal Setor -->
    <div v-if="isSetorModalOpen" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-white rounded-3xl w-full max-w-md shadow-xl overflow-hidden">
        <div class="p-6 border-b border-gray-100 flex justify-between items-center">
          <h3 class="text-xl font-bold text-gray-800">{{ setorForm.idSetor ? 'Editar' : 'Novo' }} Setor</h3>
          <button @click="isSetorModalOpen = false" class="text-gray-400 hover:text-gray-600">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <div class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nome do Setor</label>
            <input v-model="setorForm.nm_Setor" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" placeholder="Ex: Financeiro" />
          </div>
          <div v-if="!setorForm.idSetor">
            <label class="block text-sm font-medium text-gray-700 mb-1">Upload de Anexo (Em breve)</label>
            <div class="border-2 border-dashed border-gray-300 rounded-xl p-4 text-center text-gray-400">
              <span class="text-sm">Clique para fazer upload (Desabilitado)</span>
            </div>
          </div>
        </div>
        <div class="p-6 bg-gray-50 flex justify-end gap-3">
          <button @click="isSetorModalOpen = false" class="px-5 py-2.5 text-gray-600 font-medium hover:bg-gray-200 rounded-xl transition-colors">Cancelar</button>
          <button @click="saveSetor" class="px-5 py-2.5 bg-green-700 hover:bg-green-800 text-white font-medium rounded-xl transition-colors shadow-sm">Salvar</button>
        </div>
      </div>
    </div>

    <!-- Modal Funcionario -->
    <div v-if="isFuncionarioModalOpen" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-white rounded-3xl w-full max-w-lg shadow-xl overflow-hidden">
        <div class="p-6 border-b border-gray-100 flex justify-between items-center">
          <h3 class="text-xl font-bold text-gray-800">{{ funcionarioForm.idUser ? 'Editar' : 'Novo' }} Funcionário</h3>
          <button @click="isFuncionarioModalOpen = false" class="text-gray-400 hover:text-gray-600">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" /></svg>
          </button>
        </div>
        <div class="p-6 space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Nome Completo</label>
            <input v-model="funcionarioForm.nome" type="text" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" placeholder="João Silva" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Email Corporativo</label>
            <input v-model="funcionarioForm.email" type="email" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all" placeholder="joao@empresa.com" />
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1">Setor</label>
            <select v-model="funcionarioForm.idSetor" class="w-full px-4 py-2 border border-gray-300 rounded-xl focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none transition-all">
              <option :value="null">Selecione um setor...</option>
              <option v-for="s in setores" :key="s.idSetor" :value="s.idSetor">{{ s.nm_Setor }}</option>
            </select>
          </div>
          <div v-if="!funcionarioForm.idUser">
            <label class="block text-sm font-medium text-gray-700 mb-1">Upload de Foto (Em breve)</label>
            <div class="border-2 border-dashed border-gray-300 rounded-xl p-4 text-center text-gray-400">
              <span class="text-sm">Clique para fazer upload (Desabilitado)</span>
            </div>
          </div>
        </div>
        <div class="p-6 bg-gray-50 flex justify-end gap-3">
          <button @click="isFuncionarioModalOpen = false" class="px-5 py-2.5 text-gray-600 font-medium hover:bg-gray-200 rounded-xl transition-colors">Cancelar</button>
          <button @click="saveFuncionario" class="px-5 py-2.5 bg-green-700 hover:bg-green-800 text-white font-medium rounded-xl transition-colors shadow-sm">Salvar</button>
        </div>
      </div>
    </div>

    <!-- Confirm Delete Modal Setor -->
    <div v-if="isDeleteSetorModalOpen" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-white rounded-3xl w-full max-w-sm shadow-xl p-6 text-center">
        <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-red-100 mb-4">
          <svg class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
        </div>
        <h3 class="text-xl font-bold text-gray-900 mb-2">Excluir Setor</h3>
        <p class="text-gray-500 mb-6">Tem certeza que deseja excluir o setor <strong>{{ itemToDelete?.nm_Setor }}</strong>? Esta ação não pode ser desfeita.</p>
        <div class="flex justify-center gap-3">
          <button @click="isDeleteSetorModalOpen = false" class="px-4 py-2 text-gray-600 font-medium hover:bg-gray-100 rounded-xl">Cancelar</button>
          <button @click="deleteSetor" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-medium rounded-xl">Excluir</button>
        </div>
      </div>
    </div>

    <!-- Confirm Delete Modal Funcionario -->
    <div v-if="isDeleteFuncionarioModalOpen" class="fixed inset-0 bg-black/50 backdrop-blur-sm z-50 flex items-center justify-center p-4">
      <div class="bg-white rounded-3xl w-full max-w-sm shadow-xl p-6 text-center">
        <div class="mx-auto flex items-center justify-center h-12 w-12 rounded-full bg-red-100 mb-4">
          <svg class="h-6 w-6 text-red-600" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" /></svg>
        </div>
        <h3 class="text-xl font-bold text-gray-900 mb-2">Excluir Funcionário</h3>
        <p class="text-gray-500 mb-6">Tem certeza que deseja excluir <strong>{{ itemToDelete?.nome }}</strong>? Esta ação não pode ser desfeita.</p>
        <div class="flex justify-center gap-3">
          <button @click="isDeleteFuncionarioModalOpen = false" class="px-4 py-2 text-gray-600 font-medium hover:bg-gray-100 rounded-xl">Cancelar</button>
          <button @click="deleteFuncionario" class="px-4 py-2 bg-red-600 hover:bg-red-700 text-white font-medium rounded-xl">Excluir</button>
        </div>
      </div>
    </div>
  </MainLayout>
</template>
