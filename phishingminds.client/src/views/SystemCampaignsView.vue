<script setup>
import { ref, onMounted, computed } from 'vue'
import MainLayout from '../layouts/MainLayout.vue'

// Authentication and Context
const userEmpresaId = ref(null)
const isDevAdmin = ref(false)
const token = () => localStorage.getItem('token')

// State
const campaigns = ref([])
const templates = ref([])
const sectors = ref([])
const loading = ref(true)

// Modal state
const isModalOpen = ref(false)
const modalMode = ref('create') // 'create' | 'edit'
const selectedCampaignId = ref(null)

// Campaign Form state
const campaignForm = ref({
  nomeCampanha: '',
  idTemplateEmpresa: '',
  idSetores: [],
  dt_Disparo: ''
})

// Template Customizer state
const isCustomizingTemplate = ref(false)
const selectedTemplateForEdit = ref(null)
const customTemplateForm = ref({
  idTemplate: null,
  nomePersonalizado: '',
  parameters: [] // array of { idParameter, parameterName, parameterValue }
})

// Validation Message / Alert
const apiError = ref('')
const formSubmitting = ref(false)

// Load campaigns, templates, and sectors
const loadData = async () => {
  const userStr = localStorage.getItem('user')
  if (!userStr) return

  const user = JSON.parse(userStr)
  userEmpresaId.value = user.idEmpresa
  isDevAdmin.value = user.idEmpresa === 1

  try {
    loading.value = true
    
    // 1. Fetch Campaigns
    const campRes = await fetch(`/api/Campanha/empresa/${userEmpresaId.value}`, {
      headers: { 'Authorization': `Bearer ${token()}` }
    })
    if (campRes.ok) {
      campaigns.value = await campRes.json()
    }

    // Only corporate accounts (or dev admin) load sectors and templates
    // Fetch sectors
    const secRes = await fetch(`/api/Setor/empresa/${userEmpresaId.value}`, {
      headers: { 'Authorization': `Bearer ${token()}` }
    })
    if (secRes.ok) {
      sectors.value = await secRes.json()
    }

    // Fetch templates
    const tempRes = await fetch(`/api/Template/empresa/${userEmpresaId.value}`, {
      headers: { 'Authorization': `Bearer ${token()}` }
    })
    if (tempRes.ok) {
      templates.value = await tempRes.json()
    }

  } catch (err) {
    console.error('Erro ao buscar dados:', err)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadData()
})

// Date helpers
const formatDate = (dateStr) => {
  if (!dateStr) return '--'
  const date = new Date(dateStr)
  return date.toLocaleDateString('pt-BR') + ' ' + date.toLocaleTimeString('pt-BR', { hour: '2-digit', minute: '2-digit' })
}

const formatForInput = (dateStr) => {
  if (!dateStr) return ''
  const d = new Date(dateStr)
  const year = d.getFullYear()
  const month = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  const hours = String(d.getHours()).padStart(2, '0')
  const minutes = String(d.getMinutes()).padStart(2, '0')
  return `${year}-${month}-${day}T${hours}:${minutes}`
}

const isCampaignEditable = (c) => {
  if (isDevAdmin.value) return true
  // Can only edit if firing date is in the future
  return new Date(c.dt_Disparo) > new Date()
}

// Modal actions
const openCreateModal = () => {
  modalMode.value = 'create'
  selectedCampaignId.value = null
  campaignForm.value = {
    nomeCampanha: '',
    idTemplateEmpresa: templates.value[0]?.idTemplateEmpresa || '',
    idSetores: [],
    dt_Disparo: formatForInput(new Date(Date.now() + 24 * 60 * 60 * 1000)) // default to tomorrow
  }
  apiError.value = ''
  isModalOpen.value = true
}

const openEditModal = (c) => {
  if (!isCampaignEditable(c)) return

  modalMode.value = 'edit'
  selectedCampaignId.value = c.idCampaign
  campaignForm.value = {
    nomeCampanha: c.nomeCampanha,
    idTemplateEmpresa: c.idTemplateEmpresa,
    idSetores: c.idSetores ? [...c.idSetores] : [],
    dt_Disparo: formatForInput(c.dt_Disparo)
  }
  apiError.value = ''
  isModalOpen.value = true
}

const saveCampaign = async () => {
  if (!campaignForm.value.nomeCampanha || !campaignForm.value.idTemplateEmpresa || !campaignForm.value.dt_Disparo) {
    apiError.value = 'Preencha todos os campos obrigatórios.'
    return
  }

  try {
    formSubmitting.value = true
    apiError.value = ''

    const payload = {
      nomeCampanha: campaignForm.value.nomeCampanha,
      idEmpresa: userEmpresaId.value,
      idTemplateEmpresa: parseInt(campaignForm.value.idTemplateEmpresa, 10),
      idSetores: campaignForm.value.idSetores.map(id => parseInt(id, 10)),
      dt_Disparo: new Date(campaignForm.value.dt_Disparo).toISOString()
    }

    let url = '/api/Campanha'
    let method = 'POST'

    if (modalMode.value === 'edit') {
      url = `/api/Campanha/${selectedCampaignId.value}`
      method = 'PUT'
    }

    const res = await fetch(url, {
      method: method,
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token()}`
      },
      body: JSON.stringify(payload)
    })

    if (res.ok) {
      isModalOpen.value = false
      await loadData()
    } else {
      const errText = await res.json()
      apiError.value = errText.message || 'Erro ao salvar campanha.'
    }
  } catch (e) {
    console.error(e)
    apiError.value = 'Falha de comunicação com o servidor.'
  } finally {
    formSubmitting.value = false
  }
}

const handleSelectAllSectors = (e) => {
  if (e.target.checked) {
    campaignForm.value.idSetores = []
  }
}

const isDeleteModalOpen = ref(false)
const campaignIdToDelete = ref(null)

const confirmDeleteCampaign = (id) => {
  campaignIdToDelete.value = id
  isDeleteModalOpen.value = true
}

const executeDeleteCampaign = async () => {
  if (!campaignIdToDelete.value) return

  try {
    const res = await fetch(`/api/Campanha/${campaignIdToDelete.value}`, {
      method: 'DELETE',
      headers: { 'Authorization': `Bearer ${token()}` }
    })
    if (res.ok) {
      isDeleteModalOpen.value = false
      campaignIdToDelete.value = null
      await loadData()
    } else {
      alert('Erro ao excluir campanha.')
    }
  } catch (e) {
    console.error(e)
    alert('Erro de comunicação ao excluir campanha.')
  }
}

// Template Customization
const activeTemplateDetails = computed(() => {
  const id = parseInt(campaignForm.value.idTemplateEmpresa, 10)
  return templates.value.find(t => t.idTemplateEmpresa === id)
})

const openTemplateCustomizer = () => {
  const currentTemp = activeTemplateDetails.value
  if (!currentTemp) return

  selectedTemplateForEdit.value = currentTemp
  customTemplateForm.value = {
    idTemplate: currentTemp.idTemplate,
    nomePersonalizado: `${currentTemp.nomePersonalizado} (Editado)`,
    parameters: currentTemp.parameters.map(p => ({
      idParameter: p.idParameter,
      parameterName: p.parameterName,
      parameterValue: p.parameterValue || p.exampleValue
    }))
  }
  isCustomizingTemplate.value = true
}

const saveCustomizedTemplate = async () => {
  if (!customTemplateForm.value.nomePersonalizado) {
    alert('Nome personalizado do template é obrigatório.')
    return
  }

  try {
    const payload = {
      idTemplate: customTemplateForm.value.idTemplate,
      nomePersonalizado: customTemplateForm.value.nomePersonalizado,
      parameters: customTemplateForm.value.parameters.map(p => ({
        idParameter: p.idParameter,
        parameterValue: p.parameterValue
      }))
    }

    const res = await fetch(`/api/Template/empresa/${userEmpresaId.value}/customize`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token()}`
      },
      body: JSON.stringify(payload)
    })

    if (res.ok) {
      const data = await res.json()
      
      // Reload templates
      const tempRes = await fetch(`/api/Template/empresa/${userEmpresaId.value}`, {
        headers: { 'Authorization': `Bearer ${token()}` }
      })
      if (tempRes.ok) {
        templates.value = await tempRes.json()
      }

      // Auto-select newly created customized template
      campaignForm.value.idTemplateEmpresa = data.idTemplateEmpresa
      isCustomizingTemplate.value = false
    } else {
      alert('Erro ao customizar template.')
    }
  } catch (err) {
    console.error(err)
    alert('Falha ao salvar template.')
  }
}
</script>

<template>
  <MainLayout>
    <!-- Header -->
    <div class="mb-6 flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
      <div>
        <h2 class="text-3xl md:text-4xl font-bold text-green-900">
          Gestão de Campanhas
        </h2>
        <p class="text-gray-500 mt-1">Crie, agende e monitore suas simulações de phishing no sistema.</p>
      </div>
      <button 
        v-if="!isDevAdmin"
        @click="openCreateModal"
        id="btn-nova-campanha"
        class="bg-green-700 hover:bg-green-800 text-white px-5 py-2.5 rounded-xl font-medium shadow-sm transition-colors cursor-pointer flex items-center gap-2"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4" />
        </svg>
        Nova Campanha
      </button>
    </div>

    <!-- Dev Admin Alert banner -->
    <div v-if="isDevAdmin" class="bg-gray-100 border border-gray-200 text-gray-700 p-4 rounded-2xl mb-6">
      <span class="font-bold text-gray-900">Modo Administrador Global:</span> Você possui visão e privilégios de edição sobre todas as campanhas de clientes do sistema.
    </div>

    <!-- Campaigns List Box -->
    <div class="bg-white rounded-3xl shadow-sm border border-gray-100 overflow-hidden mt-8">
      <div class="p-6 border-b border-gray-100 flex justify-between items-center bg-gray-50/50">
        <h3 class="text-xl font-semibold text-gray-800">Lista de Campanhas</h3>
        <span class="text-sm font-medium text-gray-500">{{ campaigns.length }} campanhas cadastradas</span>
      </div>
      
      <div class="p-6">
        <div v-if="loading" class="text-center py-8 text-gray-500 font-medium">
          Carregando campanhas do sistema...
        </div>
        <div v-else-if="campaigns.length === 0" class="text-center py-8 text-gray-500">
          Nenhuma campanha cadastrada até o momento.
        </div>
        <div v-else class="space-y-4">
          
          <!-- Campaign Item -->
          <div 
            v-for="c in campaigns" 
            :key="c.idCampaign" 
            class="flex flex-col md:flex-row md:items-center justify-between p-5 border border-gray-100 bg-white hover:bg-gray-50 rounded-2xl transition-all gap-4"
          >
            <div class="flex items-start gap-4">
              <div class="w-12 h-12 rounded-xl flex items-center justify-center font-bold flex-shrink-0 bg-green-50 text-green-700">
                C{{ c.idCampaign }}
              </div>
              <div>
                <h4 class="font-bold text-lg text-gray-800">
                  {{ c.nomeCampanha }}
                </h4>
                <div class="text-sm text-gray-500 mt-1 space-y-1">
                  <p>📅 Disparo em: <span class="font-medium text-gray-700">{{ formatDate(c.dt_Disparo) }}</span></p>
                  <p>🎯 Setor Alvo: <span class="font-medium text-gray-700">{{ c.nm_Setor || 'Geral (Todos os Setores)' }}</span></p>
                  <p>📧 Template: <span class="font-medium text-gray-700">{{ c.nomeTemplate || 'Carregando...' }}</span></p>
                  <p v-if="isDevAdmin" class="text-green-500 font-semibold">🏢 Empresa: {{ c.nm_Empresa || 'Desconhecida' }}</p>
                </div>
              </div>
            </div>
            
            <div class="flex flex-wrap md:flex-nowrap items-center gap-4 justify-start md:justify-end">
              <!-- Status Badge -->
              <div>
                <span 
                  class="text-xs px-3 py-1.5 rounded-full font-bold uppercase tracking-wider"
                  :class="[
                    c.status === 'AGENDADO' 
                      ? 'bg-blue-50 text-blue-700 border border-blue-200' 
                      : 'bg-green-50 text-green-700 border border-green-200'
                  ]"
                >
                  {{ c.status }}
                </span>
              </div>

              <!-- Action buttons -->
              <div class="flex gap-2">
                <button
                  @click="openEditModal(c)"
                  :disabled="!isCampaignEditable(c)"
                  :title="!isCampaignEditable(c) ? 'Campanhas disparadas não podem ser editadas' : 'Editar campanha'"
                  class="p-2.5 rounded-xl border border-gray-200 text-gray-600 hover:bg-gray-50 transition-all cursor-pointer disabled:opacity-40 disabled:cursor-not-allowed flex items-center justify-center"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15.232 5.232l3.536 3.536m-2.036-5.036a2.5 2.5 0 113.536 3.536L6.5 21.036H3v-3.572L16.732 3.732z" />
                  </svg>
                </button>
                <button
                  @click="confirmDeleteCampaign(c.idCampaign)"
                  class="p-2.5 rounded-xl border border-red-200 text-red-500 hover:bg-red-50 transition-colors cursor-pointer flex items-center justify-center"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                  </svg>
                </button>
              </div>
            </div>
          </div>

        </div>
      </div>
    </div>

    <!-- MAIN MODAL (CREATE / EDIT CAMPAIGN) -->
    <div 
      v-if="isModalOpen" 
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/60 backdrop-blur-sm transition-opacity overflow-y-auto"
      style="animation: fadeIn 0.2s ease-out;"
    >
      <div 
        class="w-full max-w-xl bg-white border border-gray-100 text-gray-800 rounded-3xl shadow-xl overflow-hidden transition-all my-auto"
        style="animation: scaleIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);"
      >
        <!-- Modal Header -->
        <div class="px-6 py-5 border-b border-gray-100 flex justify-between items-center">
          <h3 class="text-xl font-bold">
            {{ modalMode === 'create' ? 'Agendar Nova Campanha' : 'Editar Campanha' }}
          </h3>
          <button @click="isModalOpen = false" class="text-gray-400 hover:text-gray-600 transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <!-- Modal Body / Form -->
        <div class="p-6 space-y-5 max-h-[75vh] overflow-y-auto">
          <div v-if="apiError" class="bg-red-50 text-red-600 border border-red-200 p-3.5 rounded-xl text-sm font-medium">
            ⚠️ {{ apiError }}
          </div>

          <!-- Campaign Name -->
          <div class="space-y-1.5">
            <label class="text-sm font-semibold block text-gray-700">
              Nome da Campanha *
            </label>
            <input 
              v-model="campaignForm.nomeCampanha"
              type="text"
              placeholder="Ex: Campanha de Conscientização Financeiro"
              class="w-full px-4 py-2.5 bg-white border border-gray-200 text-gray-800 rounded-xl outline-none focus:ring-2 focus:ring-green-700 focus:border-green-700 transition-all font-medium"
            />
          </div>

          <!-- Template Selection -->
          <div class="space-y-1.5">
            <div class="flex justify-between items-center">
              <label class="text-sm font-semibold block text-gray-700">
                Template de Phishing *
              </label>
              <button 
                v-if="campaignForm.idTemplateEmpresa"
                @click="openTemplateCustomizer"
                class="text-xs font-bold text-green-700 hover:text-green-800 transition-colors flex items-center gap-1 cursor-pointer"
              >
                ✏️ Editar Parâmetros deste Template
              </button>
            </div>
            
            <select
              v-model="campaignForm.idTemplateEmpresa"
              class="w-full px-4 py-2.5 bg-white border border-gray-200 text-gray-800 rounded-xl outline-none focus:ring-2 focus:ring-green-700 focus:border-green-700 transition-all font-medium"
            >
              <option v-for="t in templates" :key="t.idTemplateEmpresa" :value="t.idTemplateEmpresa">
                {{ t.nomePersonalizado || t.nomeTemplate }}
              </option>
            </select>
            
            <p v-if="activeTemplateDetails" class="text-xs text-gray-400 mt-1 italic">
              Categoria: {{ activeTemplateDetails.categoria }} | Dificuldade: {{ activeTemplateDetails.nivelDificuldade }}/5
            </p>
          </div>

          <!-- Target Sectors (Multiple Checkboxes) -->
          <div class="space-y-1.5">
            <label class="text-sm font-semibold block text-gray-700">
              Setores Destinatários *
            </label>
            <div class="bg-white border border-gray-200 rounded-xl p-4 space-y-2">
              <label class="flex items-center gap-2 font-medium cursor-pointer text-sm text-gray-800">
                <input 
                  type="checkbox" 
                  :checked="campaignForm.idSetores.length === 0"
                  @change="handleSelectAllSectors"
                  class="rounded border-gray-300 text-green-700 focus:ring-green-500 h-4 w-4 accent-green-700"
                />
                Geral (Todos os Setores)
              </label>
              
              <div class="border-t border-gray-150 my-2"></div>
              
              <div class="grid grid-cols-2 gap-2 max-h-40 overflow-y-auto">
                <label 
                  v-for="s in sectors" 
                  :key="s.idSetor" 
                  class="flex items-center gap-2 cursor-pointer text-sm text-gray-700"
                >
                  <input 
                    type="checkbox" 
                    :value="s.idSetor"
                    v-model="campaignForm.idSetores"
                    class="rounded border-gray-300 text-green-700 focus:ring-green-500 h-4 w-4 accent-green-700"
                  />
                  {{ s.nm_Setor }}
                </label>
              </div>
            </div>
          </div>

          <!-- Launch Date -->
          <div class="space-y-1.5">
            <label class="text-sm font-semibold block text-gray-700">
              Data e Hora do Disparo *
            </label>
            <input 
              v-model="campaignForm.dt_Disparo"
              type="datetime-local"
              class="w-full px-4 py-2.5 bg-white border border-gray-200 text-gray-800 rounded-xl outline-none focus:ring-2 focus:ring-green-700 focus:border-green-700 transition-all font-medium"
            />
          </div>
        </div>

        <!-- Modal Footer -->
        <div class="px-6 py-4 border-t border-gray-100 flex justify-end gap-3">
          <button 
            @click="isModalOpen = false"
            class="px-5 py-2.5 rounded-xl border border-gray-200 text-gray-500 hover:bg-gray-50 transition-colors font-semibold cursor-pointer"
          >
            Cancelar
          </button>
          <button 
            @click="saveCampaign"
            :disabled="formSubmitting"
            class="px-5 py-2.5 rounded-xl bg-green-700 hover:bg-green-800 disabled:bg-gray-300 text-white font-semibold transition-colors flex items-center justify-center gap-2 cursor-pointer"
          >
            {{ formSubmitting ? 'Salvando...' : 'Salvar Campanha' }}
          </button>
        </div>
      </div>
    </div>

    <!-- TEMPLATE CUSTOMIZER SUB-MODAL (INLINE POPUP) -->
    <div 
      v-if="isCustomizingTemplate" 
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/70 backdrop-blur-sm overflow-y-auto"
      style="animation: fadeIn 0.15s ease-out;"
    >
      <div 
        class="w-full max-w-lg bg-white border border-gray-100 text-gray-800 rounded-3xl shadow-2xl overflow-hidden my-auto"
      >
        <div class="px-6 py-5 border-b border-gray-100 flex justify-between items-center">
          <div>
            <h4 class="text-lg font-bold">Customizar Parâmetros do Template</h4>
            <p class="text-xs text-gray-400 mt-0.5">Base: {{ selectedTemplateForEdit?.nomeTemplate }}</p>
          </div>
          <button @click="isCustomizingTemplate = false" class="text-gray-400 hover:text-gray-600 transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <div class="p-6 space-y-4 max-h-[60vh] overflow-y-auto">
          <!-- Custom template name -->
          <div class="space-y-1.5">
            <label class="text-sm font-semibold block text-gray-700">
              Nome Personalizado do Template
            </label>
            <input 
              v-model="customTemplateForm.nomePersonalizado"
              type="text"
              class="w-full px-4 py-2 bg-white border border-gray-200 text-gray-800 rounded-xl outline-none focus:ring-2 focus:ring-green-700 font-medium"
            />
          </div>

          <!-- Template Details Preview -->
          <div class="bg-gray-50 border border-gray-100 p-4 rounded-2xl text-sm space-y-2">
            <p class="font-bold text-gray-700">📧 Assunto Base:</p>
            <p class="text-gray-600">{{ selectedTemplateForEdit?.subject }}</p>
            <p class="font-bold text-gray-700 mt-2">📝 Corpo Base:</p>
            <p class="text-gray-600 font-mono text-xs whitespace-pre-wrap leading-relaxed">{{ selectedTemplateForEdit?.bodyMail }}</p>
          </div>

          <!-- Dynamic parameters forms -->
          <div class="border-t border-gray-100 pt-4 space-y-4">
            <h5 class="text-sm font-bold uppercase tracking-wider text-gray-400">Variáveis do Email</h5>
            
            <div v-for="p in customTemplateForm.parameters" :key="p.idParameter" class="space-y-1">
              <label class="text-sm font-semibold text-gray-700 flex justify-between">
                <span>{{ p.parameterName }}</span>
              </label>
              <input 
                v-model="p.parameterValue"
                type="text"
                class="w-full px-4 py-2 bg-white border border-gray-200 text-gray-800 rounded-xl outline-none focus:ring-2 focus:ring-green-700 font-medium"
              />
            </div>
          </div>
        </div>

        <div class="px-6 py-4 border-t border-gray-100 flex justify-end gap-3">
          <button 
            @click="isCustomizingTemplate = false"
            class="px-5 py-2.5 rounded-xl border border-gray-200 text-gray-500 hover:bg-gray-50 transition-colors font-semibold cursor-pointer"
          >
            Voltar
          </button>
          <button 
            @click="saveCustomizedTemplate"
            class="px-5 py-2.5 bg-green-700 hover:bg-green-800 rounded-xl text-white font-semibold transition-colors flex items-center justify-center gap-2 cursor-pointer"
          >
            Salvar Customização
          </button>
        </div>
      </div>
    </div>

    <!-- DELETE CONFIRMATION MODAL -->
    <div 
      v-if="isDeleteModalOpen" 
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-slate-950/60 backdrop-blur-sm overflow-y-auto"
      style="animation: fadeIn 0.2s ease-out;"
    >
      <div 
        class="w-full max-w-md bg-white border border-gray-100 text-gray-800 rounded-3xl shadow-2xl overflow-hidden text-center p-6 my-auto"
        style="animation: scaleIn 0.3s cubic-bezier(0.16, 1, 0.3, 1);"
      >
        <!-- Warning Icon -->
        <div class="w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 border bg-red-50 border-red-200 text-red-500">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
          </svg>
        </div>

        <h3 class="text-xl font-bold mb-2">Excluir Campanha?</h3>
        <p class="text-sm text-gray-500 mb-6 leading-relaxed">
          Tem certeza de que deseja excluir esta campanha? Todos os alvos cadastrados e os resultados coletados serão apagados permanentemente do banco de dados. Esta ação não pode ser desfeita.
        </p>

        <!-- Buttons -->
        <div class="flex flex-col sm:flex-row gap-3 justify-center">
          <button 
            @click="isDeleteModalOpen = false"
            class="px-5 py-2.5 rounded-xl border border-gray-200 text-gray-500 hover:bg-gray-50 transition-colors font-semibold cursor-pointer w-full sm:w-auto"
          >
            Não, Cancelar
          </button>
          <button 
            @click="executeDeleteCampaign"
            class="px-5 py-2.5 rounded-xl bg-red-600 hover:bg-red-700 text-white font-semibold transition-colors cursor-pointer w-full sm:w-auto"
          >
            Sim, Excluir Campanha
          </button>
        </div>
      </div>
    </div>
  </MainLayout>
</template>

<style scoped>
@keyframes fadeIn {
  from { opacity: 0; }
  to { opacity: 1; }
}

@keyframes scaleIn {
  from { transform: scale(0.95); opacity: 0; }
  to { transform: scale(1); opacity: 1; }
}

input[type="checkbox"] {
  accent-color: #15803d;
}
</style>
