<template>
    <Card title="Задачи">
        <div v-if="isLoading && items.length === 0" class="flex items-center justify-center py-8 text-gray-400">
            <span class="animate-spin mr-2">⏳</span> Загрузка...
        </div>

        <div v-else-if="fetchError" class="text-xs text-red-600 bg-red-50 p-3 rounded-xl border border-red-200">
            Ошибка загрузки: {{ getErrorMessage(fetchError) }}
            <button @click="fetchAll" class="ml-2 underline font-semibold">Повторить</button>
        </div>

        <template v-else>
            <form @submit.prevent="handleSave" class="space-y-2 bg-gray-50 p-3 rounded-xl border border-dashed border-gray-300">
                <div>
                    <input v-model="model.name" placeholder="Название задачи" class="w-full border p-1.5 text-xs rounded border-gray-300 bg-white" :class="{ 'border-red-400': fieldErrors?.name }" required />
                    <span v-if="fieldErrors?.name" class="text-red-600 text-[10px]">{{ fieldErrors.name }}</span>
                </div>
                <div>
                    <select v-model="model.projectId" :disabled="isEditing" class="w-full border p-1.5 text-xs rounded border-gray-300 bg-white disabled:bg-gray-200" :required="!isEditing">
                        <option value="">-- Выберите проект --</option>
                        <option v-for="p in activeProjects" :key="p.id" :value="p.id">{{ p.name }}</option>
                    </select>
                </div>

                <div v-if="isEditing" class="flex gap-2">
                    <button type="submit" :disabled="saving" class="w-1/2 text-center py-1 bg-green-600 hover:bg-green-700 disabled:bg-green-300 text-white text-xs rounded font-semibold transition-colors">
                        {{ saving ? '...' : 'Сохранить' }}
                    </button>
                    <button type="button" @click="handleCancel" class="w-1/2 text-center py-1 bg-gray-500 hover:bg-gray-600 text-white text-xs rounded font-semibold transition-colors">
                        Отмена
                    </button>
                </div>
                <button v-else type="submit" :disabled="saving" class="w-full text-center py-1 bg-indigo-600 hover:bg-indigo-700 disabled:bg-indigo-300 text-white text-xs rounded font-semibold transition-colors">
                    {{ saving ? '...' : '+ Добавить задачу' }}
                </button>
            </form>

            <ul v-if="items.length > 0" class="flex flex-col gap-2 max-h-60 overflow-y-auto">
                <li v-for="t in items" :key="t.id" class="border flex items-center justify-between p-3 border-gray-200 rounded-xl bg-gray-50 text-xs">
                    <div class="flex flex-col min-w-0 flex-1">
                        <h4 class="font-bold text-gray-800 truncate">{{ t.name }}</h4>
                        <p class="text-gray-400">Проект: {{ getProjectName(t.projectId) || 'Неизвестно' }}</p>
                    </div>
                    <div class="flex items-center gap-2 shrink-0">
                        <button
                            type="button"
                            @click="handleToggleActive(t)"
                            class="px-2 py-0.5 rounded text-[10px] font-bold"
                            :class="t.isActive ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'"
                        >
                            {{ t.isActive ? 'Активна' : 'Неактивна' }}
                        </button>
                        <button type="button" @click="startEdit(t)" class="text-gray-400 hover:text-indigo-600">
                            <Pencil size="14" />
                        </button>
                        <button type="button" @click="handleDelete(t)" class="text-gray-400 hover:text-red-500">
                            <Trash2 size="14" />
                        </button>
                    </div>
                </li>
            </ul>
            <p v-else class="text-center py-4 text-gray-400 text-xs">Нет задач</p>
        </template>
    </Card>
</template>

<script setup>
import { ref, computed } from 'vue'
import { Pencil, Trash2 } from '@lucide/vue'
import Card from './Card.vue'
import { toast } from 'vue3-toastify'
import { getErrorMessage, getDefaultErrorMessage, getFieldErrors } from '../utils/error'
import { useTask } from '../hooks/useTask'
import { useConfirm } from '../hooks/useConfirm'
import { taskApi } from '../api/taskApi'

const props = defineProps({
    projects: {
        type: Array,
        default: () => [],
    },
})

const { isLoading, error, actionError, id, model, items, fetchAll, create, update, remove, startEdit, resetForm } = useTask()
const { confirm } = useConfirm()

const saving = ref(false)
const fieldErrors = ref(null)
const isEditing = computed(() => id.value !== null)
const fetchError = computed(() => error.value)

const getProjectName = (projectId) => {
    const project = props.projects.find(p => p.id === projectId)
    return project ? project.name : null
}

const handleSave = async () => {
    saving.value = true
    fieldErrors.value = null
    let ok
    if (id.value) {
        ok = await update()
        if (ok) toast.success('Задача обновлена')
    } else {
        ok = await create()
        if (ok) toast.success('Задача создана')
    }
    if (!ok && actionError.value) {
        fieldErrors.value = getFieldErrors(actionError.value)
        if (!fieldErrors.value) {
            toast.error(getErrorMessage(actionError.value))
        } else {
            toast.error(getDefaultErrorMessage())
        }
    }
    saving.value = false
}

const activeProjects = computed(() => {
    return props.projects.filter(p => p.isActive || (isEditing.value && p.id === model.projectId))
})

const handleCancel = () => {
    fieldErrors.value = null
    resetForm()
}

const handleToggleActive = async (t) => {
    saving.value = true
    try {
        await taskApi.update(t.id, {
            name: t.name,
            isActive: !t.isActive,
        })
        await fetchAll()
        toast.success(`Задача ${t.isActive ? 'неактивная' : 'активная'}`)
    } catch (e) {
        toast.error(getErrorMessage(e))
    } finally {
        saving.value = false
    }
}

const handleDelete = async (t) => {
    const ok = await confirm(`Удалить задачу «${t.name}»?`)
    if (!ok) return
    const removed = await remove(t.id)
    if (removed) {
        toast.success('Задача удалена')
    } else if (actionError.value) {
        toast.error(getErrorMessage(actionError.value))
    }
}
</script>