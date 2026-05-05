<template>
  <button :class="[
      'px-6 py-2 rounded-xl font-medium transition',
      tipo === 'primario'
        ? 'bg-green-600 text-white hover:bg-green-700'
        : 'bg-white text-gray-800 border hover:bg-gray-100'
    ]">
    <slot />
  </button>
</template>

<script setup>
defineProps({
  tipo: String
})
</script>
