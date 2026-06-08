<script setup>
  import { ref, onMounted } from 'vue'
  import MainLayout from '@/layouts/MainLayout.vue'

  const loading = ref(true)
  const necessitaTreinamento = ref(false)
  const concluido = ref(false)
  const aprovado = ref(false)

  const perguntas = [
    {
      texto: 'Você deve clicar em qualquer link recebido por e-mail?',
      correta: false
    },
    {
      texto: 'É importante verificar o remetente do e-mail antes de abrir links?',
      correta: true
    },
    {
      texto: 'Você deve compartilhar sua senha por e-mail se alguém pedir?',
      correta: false
    },
    {
      texto: 'Mensagens urgentes e alarmantes podem ser tentativas de phishing?',
      correta: true
    },
    {
      texto: 'Ao suspeitar de phishing você deve comunicar o setor responsável?',
      correta: true
    }
  ]

  const respostas = ref(Array(perguntas.length).fill(null))

  onMounted(async () => {
    try {
      const user = JSON.parse(localStorage.getItem('user'))

      const response = await fetch(
        `/api/Pessoa/necessita-treinamento/${user.idUser}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        }
      )

      const data = await response.json()

      necessitaTreinamento.value =
        data.necessitaTreinamento
    }
    catch (err) {
      console.error(err)
    }
    finally {
      loading.value = false
    }
  })

  const finalizarTreinamento = async () => {
    if (respostas.value.some(x => x === null)) {
      alert('Responda todas as perguntas.')
      return
    }

    aprovado.value = respostas.value.every(
      (resposta, index) =>
        resposta === perguntas[index].correta
    )
    if (aprovado.value) {
      const user = JSON.parse(
        localStorage.getItem('user')
      )

      await fetch(
        '/api/Treinamento/concluir',
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization:
              `Bearer ${localStorage.getItem('token')}`
          },
          body: JSON.stringify({
            idUser: user.idUser
          })
        }
      )
    }

    concluido.value = true
  }
</script>

<template>
  <MainLayout>
    <div>
      <h1 class="text-5xl font-bold text-green-900">
        Treinamentos
      </h1>

      <p class="text-gray-500 text-xl mt-2 mb-8">
        Capacitações obrigatórias para reforçar a segurança da informação.
      </p>

      <!-- Loading -->

      <div v-if="loading"
           class="bg-white rounded-3xl p-8 shadow-sm">
        <h2 class="text-2xl font-semibold text-gray-800 mb-3">
          Verificando necessidade de treinamento...
        </h2>

        <p class="text-gray-500">
          Carregando...
        </p>
      </div>

      <!-- Não precisa -->

      <div v-else-if="!necessitaTreinamento"
           class="bg-white rounded-3xl p-8 shadow-sm">
        <h2 class="text-3xl font-bold text-green-700 mb-4">
          Tudo em dia!
        </h2>

        <p class="text-gray-600">
          Você não possui treinamentos pendentes no momento.
          Continue atento às boas práticas de segurança.
        </p>
      </div>

      <!-- Resultado -->

      <div v-else-if="concluido"
           class="bg-white rounded-3xl p-8 shadow-sm">
        <div v-if="aprovado">
          <h2 class="text-3xl font-bold text-green-700 mb-4">
            Treinamento concluído
          </h2>

          <p class="text-gray-600">
            Parabéns! Você concluiu o treinamento com sucesso.
          </p>
        </div>

        <div v-else>
          <h2 class="text-3xl font-bold text-red-600 mb-4">
            Treinamento não aprovado
          </h2>

          <p class="text-gray-600">
            Você precisa acertar todas as perguntas para ser aprovado.
            Revise os conceitos e tente novamente.
          </p>
        </div>
      </div>

      <!-- Questionário -->

      <div v-else
           class="bg-white rounded-3xl p-8 shadow-sm">
        <h2 class="text-3xl font-bold text-red-600 mb-2">
          Treinamento Obrigatório
        </h2>

        <p class="text-gray-500 mb-8">
          Você ultrapassou o limite de incidentes relacionados a phishing.
          Responda corretamente todas as perguntas para concluir o treinamento.
        </p>

        <div v-for="(pergunta, index) in perguntas"
             :key="index"
             class="mb-8 border-b border-gray-100 pb-6">
          <p class="font-semibold text-lg mb-4">
            {{ index + 1 }}. {{ pergunta.texto }}
          </p>

          <div class="flex gap-8">
            <label class="flex items-center gap-2 cursor-pointer">
              <input v-model="respostas[index]"
                     type="radio"
                     :name="'pergunta-' + index"
                     :value="true" />
              Sim
            </label>

            <label class="flex items-center gap-2 cursor-pointer">
              <input v-model="respostas[index]"
                     type="radio"
                     :name="'pergunta-' + index"
                     :value="false" />
              Não
            </label>
          </div>
        </div>

        <button @click="finalizarTreinamento"
                class="bg-green-700 hover:bg-green-800 text-white px-8 py-3 rounded-xl font-semibold transition">
          Finalizar Treinamento
        </button>
      </div>
    </div>
  </MainLayout>
</template>
