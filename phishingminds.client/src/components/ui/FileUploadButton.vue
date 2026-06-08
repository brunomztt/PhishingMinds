<script setup>
defineProps({
  text: {
    type: String,
    default: 'Selecionar Arquivo'
  }
})

const emit = defineEmits(['change'])

const handleChange = (event) => {
  emit('change', event)
}
</script>

<template>
  <div>
    <input
      id="file-upload"
      type="file"
      class="hidden"
      @change="handleChange"
      accept=".xlsx,.xls"
    />

    <label
      for="file-upload"
      class="flex items-center justify-center gap-2 w-full cursor-pointer bg-green-700 hover:bg-green-800 text-white py-3 px-4 rounded-xl transition-colors font-medium shadow-sm"
    >
      <svg xmlns="http://www.w3.org/2000/svg"
        class="h-5 w-5"
        fill="none"
        viewBox="0 0 24 24"
        stroke="currentColor"
      >
        <path
          stroke-linecap="round"
          stroke-linejoin="round"
          stroke-width="2"
          d="M7 16a4 4 0 01-.88-7.903A5 5 0 0115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12"
        />
      </svg>

      {{ text }}
    </label>
  </div>
</template>