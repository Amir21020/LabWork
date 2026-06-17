export function getErrorMessage(error) {
    if (!error) return 'Неизвестная ошибка'

    const data = error?.response?.data
    if (data?.Status === 'ValidationFailed' && Array.isArray(data?.Errors)) {
        return data.Errors.map(e => e.message).join('; ')
    }

    return data?.title || error.message || 'Ошибка соединения с сервером'
}
