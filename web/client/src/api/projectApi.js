import { apiClient } from './baseApi'

export const projectApi = {
    getAll: () => apiClient.get('/project'),
    create: (model) => apiClient.post('/project', model),
    update: (id, model) => apiClient.put(`/project/${id}`, model),
    delete: (id) => apiClient.delete(`/project/${id}`)
}