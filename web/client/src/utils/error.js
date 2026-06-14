export function getErrorMessage(error) {
    if (!error) return 'Неизвестная ошибка'
    return error?.response?.data?.title || error.message || 'Ошибка соединения с сервером'
}
