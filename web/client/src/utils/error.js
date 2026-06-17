export function getErrorMessage(error) {
    if (!error) return 'Неизвестная ошибка'

    const data = error?.response?.data

    if (data?.Status === 'ValidationFailed' && Array.isArray(data?.Errors)) {
        return data.Errors.map(e => e.message).join('; ')
    }

    if (data?.errors && typeof data.errors === 'object') {
        return Object.values(data.errors)
            .flat()
            .join('; ')
    }

    return data?.detail || data?.title || error.message || 'Ошибка соединения с сервером'
}

export function getDefaultErrorMessage() {
    return 'Проверьте правильность заполнения полей'
}

export function getFieldErrors(error) {
    if (!error) return null

    const data = error?.response?.data
    if (!data) return null

    const status = data.Status || data.status
    const errors = data.Errors || data.errors

    if (status === 'ValidationFailed' && Array.isArray(errors)) {
        const result = {}
        for (const e of errors) {
            const fieldName = e.field || e.Field
            const message = e.message || e.Message
            
            if (fieldName) {
                const key = fieldName.charAt(0).toLowerCase() + fieldName.slice(1)
                if (!result[key]) {
                    result[key] = message
                }
            }
        }
        return result
    }

    if (errors && typeof errors === 'object' && !Array.isArray(errors)) {
        const result = {}
        for (const [field, messages] of Object.entries(errors)) {
            const key = field.charAt(0).toLowerCase() + field.slice(1)
            if (Array.isArray(messages) && messages.length > 0) {
                result[key] = messages[0]
            } else if (typeof messages === 'string') {
                result[key] = messages
            }
        }
        return result
    }

    return null
}