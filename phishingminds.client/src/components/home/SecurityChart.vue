<script setup>
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
} from 'chart.js'

import { Line } from 'vue-chartjs'
import { computed } from 'vue'

ChartJS.register(
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend
)

const props = defineProps({
  evolucao: {
    type: Array,
    default: () => []
  }
})

console.log('DADOS DO GRAFICO')
console.log(props.evolucao)

const chartData = computed(() => ({
  labels: props.evolucao.map(x => x.nomeCampanha),

  datasets: [
    {
      label: 'Score de Segurança',
      data: props.evolucao.map(x => x.score),
      tension: 0.3,
      pointRadius: 8
    }
  ]
}))

const emit = defineEmits([
  'campaign-click'
])

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,

  onClick(event, elements) {
    if (!elements.length)
      return

    const index = elements[0].index

    emit(
      'campaign-click',
      props.evolucao[index]
    )
  },


  plugins: {
    tooltip: {
      callbacks: {

        title(context) {
          return context[0].label
        },

        label(context) {
          const campanha =
            props.evolucao[
              context.dataIndex
            ]

          return [
            `Score: ${campanha.score}`,
            `Links clicados: ${campanha.linksClicados}`,
            `Credenciais enviadas: ${campanha.credenciaisEnviadas}`
          ]
        },

        afterLabel(context) {
          const campanha =
            props.evolucao[
              context.dataIndex
            ]

          const data =
            new Date(
              campanha.dt_Disparo
            ).toLocaleDateString(
              'pt-BR'
            )

          return [
            '',
            `Data: ${data}`,
            `Setores: ${campanha.setores}`
          ]
        }
      }
    }
  }
}
</script>

<template>
  <div class="h-[300px]">
    <Line
      :data="chartData"
      :options="chartOptions"
    />
  </div>
</template>