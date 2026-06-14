import { createApp } from 'vue'
import './style.css'
import 'vue3-toastify/dist/index.css'
import Vue3Toastify from 'vue3-toastify'
import App from './App.vue'

const app = createApp(App)

app.use(Vue3Toastify, {
    autoClose: 3000,
    hideProgressBar: true,
})

app.mount('#app')
