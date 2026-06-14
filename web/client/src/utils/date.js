export function getTodayDateString() {
    return new Date().toISOString().slice(0, 10);
}

export function parseMonthAndYear(dateStr) {
    const d = new Date(dateStr);
    return {
        month: d.getMonth() + 1,
        year: d.getFullYear(),
    };
}