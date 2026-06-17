<template>
    <Card title="Проводки времени">
        <div v-if="isLoading && items.length === 0" class="flex items-center justify-center py-8 text-gray-400">
            <span class="animate-spin mr-2">⏳</span> Загрузка...
        </div>

        <div v-else-if="fetchError" class="text-xs text-red-600 bg-red-50 p-3 rounded-xl border border-red-200">
            Ошибка загрузки: {{ getErrorMessage(fetchError) }}
            <button @click="loadEntries" class="ml-2 underline font-semibold">Повторить</button>
        </div>

        <template v-else>
            <div class="space-y-3 p-3 bg-gray-50 rounded-xl border border-gray-200 text-xs">
                <div class="flex gap-1 justify-between items-center">
                    <span class="font-bold text-gray-600">Период:</span>
                    <div class="flex bg-gray-200 p-0.5 rounded">
                        <button @click="setFilter('all')" :class="filterMode === 'all' ? 'bg-indigo-600 text-white' : 'text-gray-600'" class="px-2 py-0.5 rounded text-[10px]">Все</button>
                        <button @click="setFilter('day')" :class="filterMode === 'day' ? 'bg-indigo-600 text-white' : 'text-gray-600'" class="px-2 py-0.5 rounded text-[10px]">День</button>
                        <button @click="setFilter('month')" :class="filterMode === 'month' ? 'bg-indigo-600 text-white' : 'text-gray-600'" class="px-2 py-0.5 rounded text-[10px]">Месяц</button>
                    </div>
                </div>

                <div v-if="filterMode !== 'all'" class="flex items-center justify-between">
                    <span class="font-bold text-gray-600">Выбрать дату:</span>
                    <input type="date" v-model="selectedDate" class="border p-1 rounded bg-white" />
                </div>

                <div v-if="filterMode === 'day'" class="mt-2 p-2 border-l-4 rounded text-[11px]" :class="dailyHoursInfo.class">
                    <p>{{ dailyHoursInfo.text }}</p>
                </div>
            </div>

            <button @click="openAddForm" class="w-full text-center py-2 bg-indigo-600 hover:bg-indigo-700 text-white text-xs rounded-xl font-bold transition-colors">
                + Внести рабочее время
            </button>

            <div v-if="showForm" class="p-3 bg-white border-2 border-indigo-600 rounded-xl space-y-3">
                <h3 class="text-xs font-bold text-gray-700">
                    {{ editingId ? 'Редактировать запись' : 'Новая запись времени' }}
                </h3>

                <div v-if="formError" class="text-[10px] text-red-700 bg-red-50 p-2 rounded border-l-2 border-red-500 font-semibold">
                    {{ formError }}
                </div>

                <form @submit.prevent="handleSavePosting" class="space-y-2 text-xs">
                    <div>
                        <label class="block text-[9px] font-bold text-gray-500 mb-0.5">Задача *</label>
                        <select v-model="form.taskId" required :disabled="isTaskFieldDisabled" class="w-full border p-1.5 rounded bg-gray-50 disabled:bg-gray-200" :class="{ 'border-red-400': fieldErrors?.taskId }">
                            <option value="">-- Выбрать задачу --</option>
                            <option v-for="t in availableTasks" :key="t.id" :value="t.id">
                                {{ t.name }} {{ !t.isActive ? '(Неактивная)' : '' }}
                            </option>
                        </select>
                    </div>
                    <div class="grid grid-cols-2 gap-2">
                        <div>
                            <label class="block text-[9px] font-bold text-gray-500 mb-0.5">Дата *</label>
                            <input type="date" v-model="form.date" required class="w-full border p-1.5 rounded bg-gray-50" />
                        </div>
                        <div>
                            <label class="block text-[9px] font-bold text-gray-500 mb-0.5">Часы *</label>
                            <input type="number" min="1" max="24" step="1" v-model.number="form.hours" required class="w-full border p-1.5 rounded bg-gray-50" :class="{ 'border-red-400': fieldErrors?.hours }" />
                            <span v-if="fieldErrors?.hours" class="text-red-600 text-[10px]">{{ fieldErrors.hours }}</span>
                        </div>
                    </div>
                    <div>
                        <label class="block text-[9px] font-bold text-gray-500 mb-0.5">Описание работы *</label>
                        <input type="text" v-model="form.description" required placeholder="Что сделали..." class="w-full border p-1.5 rounded bg-gray-50" :class="{ 'border-red-400': fieldErrors?.description }" />
                        <span v-if="fieldErrors?.description" class="text-red-600 text-[10px]">{{ fieldErrors.description }}</span>
                    </div>
                    <div class="flex justify-end gap-1.5 pt-1">
                        <button type="button" @click="closeForm" class="px-2.5 py-1 border rounded text-[10px]">Отмена</button>
                        <button type="submit" :disabled="saving" class="bg-indigo-600 text-white px-3 py-1 rounded text-[10px] font-bold disabled:bg-indigo-300">
                            {{ saving ? '...' : 'Сохранить' }}
                        </button>
                    </div>
                </form>
            </div>

            <ul v-if="items.length > 0" class="flex flex-col gap-2 max-h-[20rem] overflow-y-auto">
                <li v-for="w in items" :key="w.id" class="border p-3 border-gray-200 rounded-xl bg-gray-50 text-xs">
                    <div class="flex justify-between items-start">
                        <div>
                            <div class="flex items-center gap-1.5">
                                <span class="text-[9px] bg-indigo-50 text-indigo-700 px-1.5 py-0.5 rounded font-bold">📅 {{ w.date }}</span>
                                <span class="font-black text-gray-900">{{ w.hours }} ч.</span>
                            </div>
                            <p class="font-bold text-gray-800 mt-1">
                                {{ getTaskName(w.taskId) || 'Неизвестно' }}
                                <span v-if="!isTaskActive(w.taskId)" class="text-[9px] text-red-600 font-bold ml-1">(Неактивная)</span>
                            </p>
                            <p class="text-[10px] text-gray-400">
                                Проект: {{ getProjectNameForTask(w.taskId) || '-' }}
                            </p>
                            <p class="text-[10px] text-gray-600 italic mt-1">«{{ w.description }}»</p>
                        </div>

                        <div class="flex flex-col gap-2 items-end shrink-0">
                            <button
                                v-if="isTaskActive(w.taskId)"
                                type="button"
                                @click="openEditForm(w)"
                                class="text-indigo-600 hover:text-indigo-800 font-bold text-[10px]"
                            >
                                <Pencil size="12" />
                            </button>
                            <span v-else class="text-[9px] text-gray-400 italic font-medium" title="Редактировать задачу нельзя, так как она архивирована">🔒 Блок</span>

                            <button type="button" @click="handleDelete(w)" class="text-gray-400 hover:text-red-500">
                                <Trash2 size="12" />
                            </button>
                        </div>
                    </div>
                </li>
            </ul>
            <p v-else class="text-center py-4 text-gray-400 text-xs">Нет записей за выбранный период</p>
        </template>
    </Card>
</template>
<script setup>
import { ref, computed, watchEffect } from 'vue'
import { WORK_LIMITS } from '../constants/work'
import { Pencil, Trash2 } from '@lucide/vue'
import Card from './Card.vue' 
import { useTimeEntry } from '../hooks/useTimeEntry'
import { toast } from 'vue3-toastify'
import { getErrorMessage, getDefaultErrorMessage, getFieldErrors } from '../utils/error'
import { useConfirm } from '../hooks/useConfirm'
import { getTodayDateString, parseMonthAndYear } from '../utils/date'

const props = defineProps({
    tasks: {
        type: Array,
        default: () => [],
    },
    projects: {
        type: Array,
        default: () => [],
    },
})

const { isLoading, error, actionError, id, model, items, fetchAll, create, update, remove, startEdit, resetForm } = useTimeEntry()
const { confirm } = useConfirm()

const saving = ref(false)
const fetchError = computed(() => error.value)

const filterMode = ref('day')

const selectedDate = ref(getTodayDateString())
const showForm = ref(false)
const formError = ref(null)
const fieldErrors = ref(null)

const form = ref({
    taskId: '',
    hours: 1,
    date: '',
    description: '',
})

const editingId = ref(null)


const loadEntries = async () => {
    const params = {}
    if (filterMode.value === 'day') {
        params.date = selectedDate.value
    } else if (filterMode.value === 'month') {
        const { month, year } = parseMonthAndYear(selectedDate.value)
        params.month = month
        params.year = year
    }
    await fetchAll(params)
}

watchEffect(() => {
    loadEntries()
})

const setFilter = (mode) => {
    filterMode.value = mode
}

const isTaskActive = (taskId) => {
    const task = props.tasks.find(t => t.id === taskId)
    return task ? task.isActive : false
}

const getTaskName = (taskId) => {
    const task = props.tasks.find(t => t.id === taskId)
    return task ? task.name : null
}

const getProjectNameForTask = (taskId) => {
    const task = props.tasks.find(t => t.id === taskId)
    if (!task) return null
    const project = props.projects.find(p => p.id === task.projectId)
    return project ? project.name : null
}

const availableTasks = computed(() => {
    return props.tasks.filter(t => {
        if (t.isActive) return true
        if (editingId.value && t.id === form.value.taskId) return true
        return false
    })
})

const isTaskFieldDisabled = computed(() => {
    if (!editingId.value) return false
    const task = props.tasks.find(t => t.id === form.value.taskId)
    return task ? !task.isActive : false
})

const getDailySum = (dateStr, excludeId = null) => {
    return items.value
        .filter(p => p.date === dateStr && p.id !== excludeId)
        .reduce((sum, p) => sum + p.hours, 0)
}

const dailyHoursInfo = computed(() => {
    if (filterMode.value !== 'day') return { class: '', text: '' }
    const total = getDailySum(selectedDate.value)
    
    if (total < WORK_LIMITS.STANDARD_DAILY_HOURS) {
        return {
            class: 'bg-amber-100 text-amber-800 border-amber-300',
            text: `🟡 Внесено ${total} ч. из ${WORK_LIMITS.STANDARD_DAILY_HOURS} ч. (Недостаточно)`,
        }
    } else if (total === WORK_LIMITS.STANDARD_DAILY_HOURS) {
        return {
            class: 'bg-green-100 text-green-800 border-green-300',
            text: `🟢 Внесено ${total} ч. из ${WORK_LIMITS.STANDARD_DAILY_HOURS} ч. (Достаточно)`,
        }
    } else {
        return {
            class: 'bg-red-100 text-red-800 border-red-300',
            text: `🔴 Внесено ${total} ч. из ${WORK_LIMITS.STANDARD_DAILY_HOURS} ч. (Избыточно)`,
        }
    }
})
const openAddForm = () => {
    editingId.value = null
    form.value = {
        taskId: '',
        hours: 1,
        date: selectedDate.value,
        description: '',
    }
    formError.value = null
    fieldErrors.value = null
    showForm.value = true
}

const openEditForm = (entry) => {
    editingId.value = entry.id
    form.value = {
        taskId: entry.taskId,
        hours: entry.hours,
        date: entry.date,
        description: entry.description,
    }
    formError.value = null
    fieldErrors.value = null
    showForm.value = true
}

const closeForm = () => {
    showForm.value = false
    editingId.value = null
    formError.value = null
    fieldErrors.value = null
}

const handleSavePosting = async () => {
    formError.value = null
    fieldErrors.value = null
    saving.value = true

    const currentSum = getDailySum(form.value.date, editingId.value)
    if (currentSum + form.value.hours > 24) {
        formError.value = `Сумма часов за ${form.value.date} не может превышать 24. Уже внесено: ${currentSum} ч.`
        saving.value = false
        return
    }

    model.date = form.value.date
    model.hours = form.value.hours
    model.description = form.value.description
    model.taskId = form.value.taskId

    if (editingId.value) {
        id.value = editingId.value
        const ok = await update()
        if (ok) {
            toast.success('Запись обновлена')
            closeForm()
            await loadEntries()
        } else if (actionError.value) {
            fieldErrors.value = getFieldErrors(actionError.value)
            if (!fieldErrors.value) {
                toast.error(getErrorMessage(actionError.value))
            } else {
                toast.error(getDefaultErrorMessage())
            }
        }
    } else {
        const ok = await create()
        if (ok) {
            toast.success('Запись создана')
            closeForm()
            await loadEntries()
        } else if (actionError.value) {
            fieldErrors.value = getFieldErrors(actionError.value)
            if (!fieldErrors.value) {
                toast.error(getErrorMessage(actionError.value))
            } else {
                toast.error(getDefaultErrorMessage())
            }
        }
    }
    saving.value = false
}

const handleDelete = async (w) => {
    const ok = await confirm(`Удалить запись на ${w.date} (${w.hours} ч.)?`)
    if (!ok) return
    const removed = await remove(w.id)
    if (removed) {
        toast.success('Запись удалена')
        await loadEntries()
    } else if (actionError.value) {
        toast.error(getErrorMessage(actionError.value))
    }
}
</script>