<template>
  <header class="mb-6 bg-white p-4 rounded-xl border border-gray-300 shadow-sm flex items-center justify-between">
    <div class="flex items-center gap-2">
      <h1 class="text-xl font-black text-indigo-600">{{ title }}</h1>
    </div>

    <div>
      <slot name="actions" />
    </div>
  </header>
</template>

<script setup>
defineProps({
  title: {
    type: String,
    default: 'Тайм-трекер рабочего времени'
  }
})
</script>