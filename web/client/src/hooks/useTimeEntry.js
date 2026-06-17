import { reactive, ref } from 'vue'
import { timeEntryApi } from '../api/timeEntryApi'

export function useTimeEntry() {
    const isLoading = ref(false)
    const error = ref(null)
    const actionError = ref(null)
    const id = ref(null)

    const model = reactive({
        date: '',
        hours: 1,
        description: '',
        taskId: '',
    })

const items = ref([])

const fetchAll = async (params = {}) => {
    isLoading.value = true
    error.value = null
    try {
        const response = await timeEntryApi.getAll(params)
        const data = response.data ?? response
        items.value = Array.isArray(data) ? data : []
    } catch (e) {
        error.value = e
    } finally {
        isLoading.value = false
    }
}

    const create = async () => {
        isLoading.value = true
        actionError.value = null
        try {
            await timeEntryApi.create(model)
            resetForm()
            return true
        } catch (e) {
            actionError.value = e
            return false
        } finally {
            isLoading.value = false
        }
    }

    const update = async () => {
        if (!id.value) return false
        isLoading.value = true
        actionError.value = null
        try {
            await timeEntryApi.update(id.value, model)
            resetForm()
            return true
        } catch (e) {
            actionError.value = e
            return false
        } finally {
            isLoading.value = false
        }
    }

    const remove = async (targetId) => {
        const activeId = targetId || id.value
        if (!activeId) return false
        isLoading.value = true
        actionError.value = null
        try {
            await timeEntryApi.delete(activeId)
            return true
        } catch (e) {
            actionError.value = e
            return false
        } finally {
            isLoading.value = false
        }
    }

    const startEdit = (entry) => {
        id.value = entry.id
        model.date = entry.date
        model.hours = entry.hours
        model.description = entry.description
        model.taskId = entry.taskId
    }

    const resetForm = () => {
        model.date = ''
        model.hours = 1
        model.description = ''
        model.taskId = ''
        id.value = null
    }

    return {
        isLoading,
        error,
        actionError,
        id,
        model,
        items,
        fetchAll,
        create,
        update,
        remove,
        startEdit,
        resetForm,
    }
}