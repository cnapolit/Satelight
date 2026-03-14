import { MenuNavigation } from "./MenuNavigation.js";
import {
    backgroundId,
    gamesBackgroundId,
    centeredClass,
    filtersId,
    focusedMenuClass,
    gameDetailsId,
    gamesListId,
    gameStartDialogueId,
    hideClass,
    logoId,
    selectedClass,
    topBarId
} from "./navigationConstants.js";
import { changeMenu, getActiveElement } from "./navigationUtils.js";

export class GameDetailsMenuNavigation extends MenuNavigation {
    constructor() {
        super(gameDetailsId, null, null);
    }

    moveUp() {
        if (!this.scroll(-300)) {
            super.moveUp();
        }
    }

    moveDown() {
        if (!this.scroll(300)) {
            super.moveDown();
        }
    }

    scroll(amount) {
        const focusedItem = getActiveElement();
        let isScrollable = focusedItem && focusedItem.id === "details";
        if (isScrollable) {
            focusedItem.scrollTop += amount;
        }
        return isScrollable;
    }

    select() {
        const activeElement = super.select();
        if (!activeElement || activeElement.id !== "play-button" || activeElement.children[0].textContent !== "PLAY") {
            return;
        }
        changeMenu(gameStartDialogueId, true);
        const logo = document.querySelector("#" + logoId);
        logo.classList.add(centeredClass);
        const topBar = document.querySelector("#" + topBarId);
        topBar.classList.add(hideClass);
    }

    back() {
        const focusedItem = document.querySelector("#" + gameDetailsId);
        focusedItem.classList.add(hideClass);
        focusedItem.classList.remove(focusedMenuClass);

        const blurredBackground = document.querySelector("#" + gamesBackgroundId);
        blurredBackground.classList.remove(hideClass);

        const background = document.querySelector("#" + backgroundId);
        background.classList.remove(gameDetailsId);

        const filters = document.querySelector("#" + filtersId);
        filters.classList.remove(hideClass);

        const topBar = document.querySelector("#" + topBarId);
        topBar.classList.add(hideClass);

        const gamesList = document.querySelector("#" + gamesListId);
        gamesList.classList.add(focusedMenuClass);
        const selectedItem = gamesList.querySelector("." + selectedClass);
        selectedItem.focus();
    }
}
