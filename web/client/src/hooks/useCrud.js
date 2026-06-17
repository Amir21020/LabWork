import { ref } from 'vue'

export function useCrud(api) {
    const isLoading = ref(false)
    const error = ref(null)
    const actionError = ref(null)
    const editingId = ref(null)
    const items = ref([])

    const fetchAll = async () => {
        isLoading.value = true
        error.value = null
        try {
            const response = await api.getAll()
            const data = response.data ?? response
            items.value = Array.isArray(data) ? data : []
        } catch (e) {
            error.value = e
        } finally {
            isLoading.value = false
        }
    }

    const create = async (model) => {
        isLoading.value = true
        actionError.value = null
        try {
            await api.create(model)
            await fetchAll()
            return true
        } catch (e) {
            actionError.value = e
            return false
        } finally {
            isLoading.value = false
        }
    }

    const update = async (id, model) => {
        if (!id) return false
        isLoading.value = true
        actionError.value = null
        try {
            await api.update(id, model)
            await fetchAll()
            return true
        } catch (e) {
            actionError.value = e
            return false
        } finally {
            isLoading.value = false
        }
    }

    const remove = async (targetId) => {
        const activeId = targetId || editingId.value
        if (!activeId) return false
        isLoading.value = true
        actionError.value = null
        try {
            await api.delete(activeId)
            await fetchAll()
            return true
        } catch (e) {
            actionError.value = e
            return false
        } finally {
            isLoading.value = false
        }
    }

    return {
        isLoading, error, actionError, editingId, items,
        fetchAll, create, update, remove,
    }
}
