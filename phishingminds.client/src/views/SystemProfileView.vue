<script setup>
import MainLayout from '../layouts/MainLayout.vue'
</script>

<template>
  <MainLayout>
    <div class="mb-6">
      <h2 class="text-3xl md:text-4xl font-bold text-green-900">Meu Perfil</h2>
      <p class="text-gray-500 mt-1">Gerencie suas informações pessoais e configurações de segurança.</p>
    </div>

    <div class="bg-white rounded-3xl shadow-sm overflow-hidden max-w-3xl mt-8">
      <div class="p-8 border-b border-gray-100 flex flex-col sm:flex-row items-center gap-6 text-center sm:text-left">
        <div class="w-24 h-24 bg-green-700 rounded-full flex-shrink-0 flex items-center justify-center text-white text-3xl font-bold shadow-md">
          A
        </div>
        <div>
          <h3 class="text-2xl font-bold text-gray-800">Administrador Sistema</h3>
          <p class="text-gray-500">admin@phishingminds.com</p>
          <span class="inline-block mt-2 text-xs font-bold uppercase tracking-wider text-green-800 bg-green-100 px-3 py-1 rounded-full">
            Global Admin
          </span>
        </div>
      </div>

      <div class="p-8">
        <form class="space-y-6">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">Nome Completo</label>
              <input type="text" value="Administrador Sistema" class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" />
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-2">E-mail</label>
              <input type="email" value="admin@phishingminds.com" class="w-full bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" />
            </div>
          </div>

          <div class="pt-6 border-t border-gray-100">
            <h4 class="text-lg font-semibold text-gray-800 mb-4">Alterar Senha</h4>
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Senha Atual</label>
                <input type="password" placeholder="••••••••" class="w-full md:w-1/2 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" />
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Nova Senha</label>
                <input type="password" placeholder="••••••••" class="w-full md:w-1/2 bg-gray-50 border border-gray-200 rounded-xl px-4 py-3 outline-none focus:border-green-500 transition-colors" />
              </div>
            </div>
          </div>

          <div class="pt-6 flex justify-end">
            <button type="button" class="bg-green-700 hover:bg-green-800 text-white px-6 py-3 rounded-xl font-medium shadow-sm transition-colors w-full sm:w-auto">
              Salvar Alterações
            </button>
          </div>
        </form>
      </div>
    </div>
  </MainLayout>
</template>
