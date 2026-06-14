import { reactive } from 'vue'
import { useCrud } from './useCrud'
import { taskApi } from '../api/taskApi'

const { isLoading, error, actionError, editingId, items, fetchAll, ...crud } = useCrud(taskApi)

const model = reactive({
    name: '',
    projectId: '',
    isActive: true,
})

export function useTask() {
    const startEdit = (task) => {
        editingId.value = task.id
        model.name = task.name
        model.projectId = task.projectId
        model.isActive = task.isActive
    }

    const resetForm = () => {
        model.name = ''
        model.projectId = ''
        model.isActive = true
        editingId.value = null
    }

    const create = async () => {
        const ok = await crud.create(model)
        if (ok) resetForm()
        return ok
    }

    const update = async () => {
        const ok = await crud.update(editingId.value, model)
        if (ok) resetForm()
        return ok
    }

    return {
        isLoading,
        error,
        actionError,
        id: editingId,
        model,
        items,
        fetchAll,
        create,
        update,
        remove: crud.remove,
        startEdit,
        resetForm,
    }
}
