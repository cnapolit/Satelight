
export function revealLogo(showClass, hideClass) {
    const show = document.querySelector(`.${showClass}`);
    const hide = document.querySelector(`.${hideClass}`);

    function reveal() {
        show.classList.remove("hide");
        if (hide) hide.classList.add("hide");
    }

    if (!show) return;

    if (show.tagName !== "IMG" || show.complete) {
        reveal();
    } else {
        show.onload = reveal;
    }
}
