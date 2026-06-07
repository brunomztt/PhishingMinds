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

const chartOptions = {
  responsive: true,
  maintainAspectRatio: false
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