import {gameStartDialogueId, backgroundId, centeredClass, focusedMenuClass, gameDetailsId, hideClass, logoId, topBarId} from '/js/navigation/navigationConstants.js'
function hideElement(id) {
  const element = document.querySelector("#" + id);
  if (element) {
    element.classList.add(hideClass);
  }
}

function revealElement(id) {
  const element = document.querySelector("#" + id);
  if (element) {
    element.classList.remove(hideClass);
  }
}


export function revealHdmiVideo() {
  hideElement(gameDetailsId);
  hideElement(backgroundId);
  hideElement(topBarId);
  hideElement(logoId);
  hideElement(gameStartDialogueId);
  const focusedMenu = document.querySelector("." + focusedMenuClass);
  if (focusedMenu) {
    focusedMenu.classList.remove(focusedMenuClass);
  }
}

export function hideHdmiVideo() {
  const hdmiNotFocused = document.querySelector("." + focusedMenuClass);
  if (hdmiNotFocused) {
    return
  }

  revealElement(backgroundId);
  revealElement(topBarId);
  revealElement(logoId);
  const logo = document.querySelector("#" + logoId);
  if (logo) {
    logo.classList.remove(hideClass);
    logo.classList.remove(centeredClass);
  }

  const gameDetails = document.querySelector("#" + gameDetailsId);
  if (gameDetails) {
    gameDetails.classList.remove(hideClass);
    const playButton = gameDetails.children[0];
    playButton.focus();
  }
}