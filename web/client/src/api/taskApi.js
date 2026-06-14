import { apiClient } from './baseApi'

export const taskApi = {
    getAll: () => apiClient.get('/task'),
    create: (model) => apiClient.post('/task', model),
    update: (id, model) => apiClient.put(`/task/${id}`, model),
    delete: (id) => apiClient.delete(`/task/${id}`)
}
