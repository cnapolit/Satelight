
export function revealImage(containerId, showSelector, hideSelector, url) {
    const container = document.getElementById(containerId);
    if (!container) return;

    const show = container.querySelector(showSelector);
    const hide = container.querySelector(hideSelector);

    if (!url) {
        if (show) show.style.opacity = "0";
        if (hide) hide.style.opacity = "0";
        return;
    }

    show.onload = () => {
        show.style.opacity = "1";
        if (hide) hide.style.opacity = "0";
    };
    show.src = url;
}
