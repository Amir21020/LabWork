<template>
    <Card title="Проекты">
        <div v-if="isLoading && items.length === 0" class="flex items-center justify-center py-8 text-gray-400">
            <span class="animate-spin mr-2">⏳</span> Загрузка...
        </div>

        <div v-else-if="fetchError" class="text-xs text-red-600 bg-red-50 p-3 rounded-xl border border-red-200">
            Ошибка загрузки: {{ fetchError.message }}
            <button @click="fetchAll" class="ml-2 underline font-semibold">Повторить</button>
        </div>

        <template v-else>
            <form @submit.prevent="handleSave" class="space-y-2 bg-gray-50 p-3 rounded-xl border border-dashed border-gray-300">
                <div class="flex gap-2">
                    <input v-model="model.name" placeholder="Название" class="border w-1/2 p-1.5 text-xs rounded border-gray-300 bg-white" required />
                    <input v-model="model.code" placeholder="Код" class="border w-1/2 p-1.5 text-xs rounded border-gray-300 bg-white" required />
                </div>

                <div v-if="editingId" class="flex gap-2">
                    <button type="submit" :disabled="saving" class="w-1/2 text-center py-1 bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white text-xs rounded font-semibold transition-colors">
                        {{ saving ? '...' : 'Сохранить' }}
                    </button>
                    <button type="button" @click="handleCancel" class="w-1/2 text-center py-1 bg-gray-500 hover:bg-gray-600 text-white text-xs rounded font-semibold transition-colors">
                        Отмена
                    </button>
                </div>
                <button v-else type="submit" :disabled="saving" class="w-full text-center py-1 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-300 text-white text-xs rounded font-semibold transition-colors">
                    {{ saving ? '...' : '+ Добавить проект' }}
                </button>
            </form>

            <ul v-if="items.length > 0" class="flex flex-col gap-2 max-h-60 overflow-y-auto">
                <li v-for="p in items" :key="p.id" class="border flex items-center justify-between p-3 border-gray-200 rounded-xl bg-gray-50 text-xs">
                    <div class="flex flex-col">
                        <h4 class="font-bold text-gray-800">{{ p.name }}</h4>
                        <p class="text-gray-400">Код: {{ p.code }}</p>
                    </div>
                    <div class="flex items-center gap-2 shrink-0">
                        <button
                            type="button"
                            @click="handleToggleActive(p)"
                            class="px-2 py-0.5 rounded text-[10px] font-bold shrink-0"
                            :class="p.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
                        >
                            {{ p.isActive ? 'Активен' : 'Неактивен' }}
                        </button>
                        <button type="button" @click="startEdit(p)" class="text-gray-400 hover:text-indigo-600 shrink-0">
                            <Pencil size="14" />
                        </button>
                        <button type="button" @click="handleDelete(p)" class="text-gray-400 hover:text-red-500 shrink-0">
                            <Trash2 size="14" />
                        </button>
                    </div>
                </li>
            </ul>
            <p v-else class="text-center py-4 text-gray-400 text-xs">Нет проектов</p>
        </template>
    </Card>
</template>

<script setup>
import { ref, computed } from 'vue'
import { Pencil, Trash2 } from '@lucide/vue'
import { toast } from 'vue3-toastify'
import { getErrorMessage } from '../utils/error'
import { Card } from './index'
import { useProject } from '../hooks/useProject'
import { useConfirm } from '../hooks/useConfirm'
import { projectApi } from '../api/projectApi'

const { isLoading, error, actionError, id, model, items, fetchAll, create, update, remove, startEdit, resetForm } = useProject()
const { confirm } = useConfirm()

const saving = ref(false)
const editingId = computed(() => id.value)
const fetchError = computed(() => error.value)

const handleSave = async () => {
    saving.value = true
    let ok
    if (id.value) {
        ok = await update()
        if (ok) toast.success('Проект обновлён')
    } else {
        ok = await create()
        if (ok) toast.success('Проект создан')
    }
    if (!ok && actionError.value) {
        toast.error(getErrorMessage(actionError.value))
    }
    saving.value = false
}

const handleCancel = () => {
    resetForm()
}

const handleToggleActive = async (p) => {
    saving.value = true
    try {
        await projectApi.update(p.id, {
            name: p.name,
            code: p.code,
            isActive: !p.isActive,
        })
        await fetchAll()
        toast.success(`Проект ${p.isActive ? 'архивирован' : 'активирован'}`)
    } catch (e) {
        toast.error(getErrorMessage(e))
    } finally {
        saving.value = false
    }
}

const handleDelete = async (p) => {
    const ok = await confirm(`Удалить проект «${p.name}»?`)
    if (!ok) return
    const removed = await remove(p.id)
    if (removed) {
        toast.success('Проект удалён')
    } else if (actionError.value) {
        toast.error(getErrorMessage(actionError.value))
    }
}
</script>
