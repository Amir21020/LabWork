import { ref } from 'vue'

const isOpen = ref(false)
const message = ref('')
const title = ref('')
let resolvePromise = null

export function useConfirm() {
    const confirm = (msg, dialogTitle = 'Подтверждение') => {
        message.value = msg
        title.value = dialogTitle
        isOpen.value = true
        return new Promise((resolve) => {
            resolvePromise = resolve
        })
    }

    const onConfirm = () => {
        isOpen.value = false
        resolvePromise?.(true)
        resolvePromise = null
    }

    const onCancel = () => {
        isOpen.value = false
        resolvePromise?.(false)
        resolvePromise = null
    }

    return { isOpen, title, message, confirm, onConfirm, onCancel }
}
