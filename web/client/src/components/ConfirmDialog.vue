<template>
    <Teleport to="body">
        <Transition name="confirm">
            <div v-if="isOpen" class="fixed inset-0 z-50 flex items-center justify-center bg-black/40" @click.self="onCancel">
                <div class="bg-white rounded-2xl shadow-2xl p-6 mx-4 max-w-sm w-full border border-gray-200">
                    <h3 class="text-lg font-bold text-gray-800 mb-2">{{ title }}</h3>
                    <p class="text-sm text-gray-600 mb-6">{{ message }}</p>
                    <div class="flex justify-end gap-2">
                        <button
                            @click="onCancel"
                            class="px-4 py-2 text-sm font-semibold rounded-xl border border-gray-300 text-gray-600 hover:bg-gray-50 transition-colors"
                        >
                            Отмена
                        </button>
                        <button
                            @click="onConfirm"
                            class="px-4 py-2 text-sm font-semibold rounded-xl bg-red-600 text-white hover:bg-red-700 transition-colors"
                        >
                            Удалить
                        </button>
                    </div>
                </div>
            </div>
        </Transition>
    </Teleport>
</template>

<script setup>
import { useConfirm } from '../hooks/useConfirm'

const { isOpen, title, message, onConfirm, onCancel } = useConfirm()
</script>

<style scoped>
.confirm-enter-active {
    transition: all 0.2s ease-out;
}
.confirm-leave-active {
    transition: all 0.15s ease-in;
}
.confirm-enter-from,
.confirm-leave-to {
    opacity: 0;
}
.confirm-enter-from > div,
.confirm-leave-to > div {
    transform: scale(0.95);
}
</style>
