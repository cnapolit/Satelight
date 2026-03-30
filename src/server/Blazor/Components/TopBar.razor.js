
let clockInterval;

function updateClock() {
    const now = new Date();
    const hours   = String(now.getHours()).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    document.getElementById('clock-text').textContent = `${hours}:${minutes}`;
}

export function initClock() {
    updateClock();
    clockInterval = setInterval(updateClock, 60000);
}

export function disposeClock() {
    clearInterval(clockInterval);
}
