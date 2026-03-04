function hideElement(className) {
  const element = document.querySelector(className);
  if (element) {
    element.classList.add("hide");
  }
}


export function revealHdmiVideo() {
  hideElement("#game-details");
  hideElement("#blurred-background");
  hideElement("#top-bar");
  hideElement("#filters");
  hideElement("#logo");
  const focusedMenu = document.querySelector(".focused-menu");
  if (focusedMenu) {
    focusedMenu.classList.remove("focused-menu");
  }
}
