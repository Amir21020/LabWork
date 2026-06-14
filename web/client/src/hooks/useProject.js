import { reactive } from 'vue'
import { useCrud } from './useCrud'
import { projectApi } from '../api/projectApi'

const { isLoading, error, actionError, editingId, items, fetchAll, ...crud } = useCrud(projectApi)

const model = reactive({
    name: '',
    code: '',
})

export function useProject() {
    const startEdit = (project) => {
        editingId.value = project.id
        model.name = project.name
        model.code = project.code
    }

    const resetForm = () => {
        model.name = ''
        model.code = ''
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
