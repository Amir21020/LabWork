<script setup>
import { ref, onMounted } from 'vue'
import { ToastifyContainer } from 'vue3-toastify'
import { ConfirmDialog, AppHeader, ProjectsSection, TasksSection, PostingsSection } from './components'
import { useProject } from './hooks/useProject'
import { useTask } from './hooks/useTask'

const {
    items: projects,
    fetchAll: fetchProjects,
} = useProject()

const {
    items: tasks,
    fetchAll: fetchTasks,
} = useTask()

const isLoading = ref(true)

onMounted(async () => {
    await Promise.all([fetchProjects(), fetchTasks()])
    isLoading.value = false
})
</script>

<template>
    <div class="max-w-7xl mx-auto p-4 md:p-6">
        <AppHeader />

        <div v-if="isLoading" class="flex select-none items-center justify-center py-20 text-gray-400">
            <span class="animate-spin mr-2 text-xl">⏳</span>
            <span class="text-lg">Загрузка данных...</span>
        </div>

        <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <ProjectsSection />
            <TasksSection :projects="projects" />
            <PostingsSection :tasks="tasks" :projects="projects" />
        </div>
    </div>

    <ConfirmDialog />
    <ToastifyContainer />
</template>
