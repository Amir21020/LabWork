<script setup>
defineProps({
    title: {
        type: String,
        required: true
    }
})

</script>
<template>
    <div class="p-5 bg-white shadow-sm rounded-2xl border border-gray-300 flex flex-col gap-4">
        <div class="text-lg font-bold text-gray-700">{{ title }}</div>
        <slot />
    </div>
</template>