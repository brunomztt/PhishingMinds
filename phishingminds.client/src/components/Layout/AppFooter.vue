<template>
  <footer class="bg-[#1b3324] text-white pt-16 pb-8 border-t border-[#112318]">
    <div class="max-w-7xl mx-auto px-10">

      <div class="grid grid-cols-1 md:grid-cols-4 gap-12 lg:gap-16">

        <div class="md:col-span-1">
          <div class="flex items-center gap-2 mb-6">
            <div class="w-32 h-32 flex-shrink-0">
              <img src="@/assets/img/logo.svg" alt="Logo Phishing Minds" class="w-full h-full object-contain" />
            </div>
          </div>
          <p class="text-[13px] text-gray-400 leading-relaxed pr-4">
            Protegendo empresas contra ataques de phishing através de treinamento inteligente, simulações realistas e análises detalhadas. O elo mais forte da sua segurança.
          </p>
        </div>

        <div>
          <h4 class="text-xs font-bold tracking-wider text-white mb-6 uppercase">Produto</h4>
          <ul class="space-y-4 text-[13px] text-gray-400">
            <li><a href="#" class="hover:text-green-400 transition-colors">Recursos</a></li>
            <li><a href="#" class="hover:text-green-400 transition-colors">Preços</a></li>
            <li><a href="#" class="hover:text-green-400 transition-colors">Integrações</a></li>
            <li><a href="#" class="hover:text-green-400 transition-colors">API</a></li>
          </ul>
        </div>

        <div>
          <h4 class="text-xs font-bold tracking-wider text-white mb-6 uppercase">Empresa</h4>
          <ul class="space-y-4 text-[13px] text-gray-400">
            <li><a href="#" class="hover:text-green-400 transition-colors">Sobre Nós</a></li>
            <li><a href="#" class="hover:text-green-400 transition-colors">Blog</a></li>
            <li><a href="#" class="hover:text-green-400 transition-colors">Carreiras</a></li>
            <li><a href="#" class="hover:text-green-400 transition-colors">Contato</a></li>
          </ul>
        </div>

        <div>
          <h4 class="text-xs font-bold tracking-wider text-white mb-6 uppercase">Fique Atualizado</h4>
          <p class="text-[13px] text-gray-400 mb-4">
            Receba dicas de segurança e novidades no seu e-mail.
          </p>

          <div class="relative mb-8">
            <input type="email"
                   placeholder="Seu melhor e-mail"
                   class="w-full bg-[#24402e] text-white text-sm placeholder-gray-400 border border-[#2c4d38] rounded-lg py-2.5 pl-4 pr-12 focus:outline-none focus:ring-1 focus:ring-green-500 transition-all" />
            <button class="absolute right-1.5 top-1.5 bottom-1.5 bg-[#4c8462] hover:bg-[#5b9a74] transition-colors rounded-md px-3 flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="2" stroke="white" class="w-4 h-4">
                <path stroke-linecap="round" stroke-linejoin="round" d="M13.5 4.5 21 12m0 0-7.5 7.5M21 12H3" />
              </svg>
            </button>
          </div>

          <div class="space-y-3 text-[13px] text-gray-400">
            <div class="flex items-center gap-3">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-4 h-4 text-green-500">
                <path stroke-linecap="round" stroke-linejoin="round" d="M21.75 6.75v10.5a2.25 2.25 0 0 1-2.25 2.25h-15a2.25 2.25 0 0 1-2.25-2.25V6.75m19.5 0A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25m19.5 0v.243a2.25 2.25 0 0 1-1.07 1.916l-7.5 4.615a2.25 2.25 0 0 1-2.36 0L3.32 8.91a2.25 2.25 0 0 1-1.07-1.916V6.75" />
              </svg>
              <span>contato@phishingminds.com</span>
            </div>
            <div class="flex items-center gap-3">
              <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" stroke-width="1.5" stroke="currentColor" class="w-4 h-4 text-green-500">
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 10.5a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
                <path stroke-linecap="round" stroke-linejoin="round" d="M19.5 10.5c0 7.142-7.5 11.25-7.5 11.25S4.5 17.642 4.5 10.5a7.5 7.5 0 1 1 15 0Z" />
              </svg>
              <span>Curitiba, PR - Brasil</span>
            </div>
          </div>
        </div>
      </div>

      <div class="mt-16 pt-8 border-t border-[#2a4735] flex flex-col md:flex-row justify-between items-center gap-4">
        <p class="text-[12px] text-gray-500">
          © 2026 Phishing Minds. Todos os direitos reservados.
        </p>

        <div class="flex gap-6 text-[12px] text-gray-500">
          <a href="#" class="hover:text-gray-300 transition-colors">Política de Privacidade</a>
          <a href="#" class="hover:text-gray-300 transition-colors">Termos de Uso</a>
          <a href="#" class="hover:text-gray-300 transition-colors">LGPD</a>
        </div>
      </div>

    </div>
  </footer>
</template>

<script setup>
  // Nenhuma lógica complexa necessária no momento, apenas o layout visual.
</script>
