
function hideElement(elementId) {
    const element = document.getElementById(elementId);
    element.classList.add("hide");
}

export function showGame() {
    hideElement("game-start-dialogue");
    hideElement("games-background");
    hideElement("logo");
    document.activeElement.blur();
    document.activeElement.parentElement.classList.remove("focused-menu")
}