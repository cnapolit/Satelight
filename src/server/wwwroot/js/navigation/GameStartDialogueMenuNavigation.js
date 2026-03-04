import { MenuNavigation } from "./MenuNavigation.js";
import { gameDetailsClass, topBarClass } from "./navigationConstants.js";
import { changeMenu } from "./navigationUtils.js";

export class GameStartDialogueMenuNavigation extends MenuNavigation {
    constructor() {
        super("game-start-dialogue", null, null);
    }

    moveLeft() {}
    moveRight() {}
    moveUp() {}
    moveDown() {}
    select() {}
    options() {}

    back() {
        changeMenu(gameDetailsClass, true);
        const logo = document.querySelector("#game-logo");
        logo.classList.remove("center");
        const topBar = document.querySelector("#" + topBarClass);
        topBar.classList.remove("hide");
    }
}
