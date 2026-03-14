import { MenuNavigation } from "./MenuNavigation.js";
import { hideClass, backgroundId, logoId, gameDetailsId, topBarId, gameStartDialogueId, centeredClass } from "./navigationConstants.js";
import { changeMenu } from "./navigationUtils.js";
function revealElement(id) {
    const element = document.querySelector("#" + id);
    if (element) {
      element.classList.remove(hideClass);
    }
}

export class GameStartDialogueMenuNavigation extends MenuNavigation {
    constructor() {
        super(gameStartDialogueId, null, null);
    }

    moveLeft() {}
    moveRight() {}
    moveUp() {}
    moveDown() {}
    select() {}
    options() {}

    back() {
        changeMenu(gameDetailsId, true);
        revealElement(topBarId);
        revealElement(gameStartDialogueId);
        revealElement(backgroundId);
        const logo = document.querySelector("#" + logoId);
        logo.classList.remove(centeredClass);
        logo.classList.remove(hideClass);
    }
}
