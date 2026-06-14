import { apiClient } from './baseApi'

export const timeEntryApi = {
    getAll: (params) => apiClient.get('/timeentry', { params }),
    create: (model) => apiClient.post('/timeentry', model),
    update: (id, model) => apiClient.put(`/timeentry/${id}`, model),
    delete: (id) => apiClient.delete(`/timeentry/${id}`)
}
