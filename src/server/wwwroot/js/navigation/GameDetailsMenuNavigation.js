import { MenuNavigation } from "./MenuNavigation.js";
import {
    filtersClass,
    focusedMenuClass,
    gameDetailsClass,
    gamesListClass,
    hideClass,
    selectedClass,
    topBarClass
} from "./navigationConstants.js";
import { changeMenu, getActiveElement } from "./navigationUtils.js";

export class GameDetailsMenuNavigation extends MenuNavigation {
    constructor() {
        super(gameDetailsClass, null, null);
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
        changeMenu("game-start-dialogue", true);
        const logo = document.querySelector("#game-logo");
        logo.classList.add("center");
        const topBar = document.querySelector("#" + topBarClass);
        topBar.classList.add("hide");
    }

    back() {
        const focusedItem = document.querySelector("#" + gameDetailsClass);
        focusedItem.classList.add(hideClass);
        focusedItem.classList.remove(focusedMenuClass);

        const blurredBackground = document.querySelector("#blurred-background");
        blurredBackground.classList.remove(hideClass);

        const background = document.querySelector("#games-background");
        background.classList.remove("game-details");

        const filters = document.querySelector("#" + filtersClass);
        filters.classList.remove(hideClass);

        const gamesList = document.querySelector("#" + gamesListClass);
        gamesList.classList.add(focusedMenuClass);
        const selectedItem = gamesList.querySelector("." + selectedClass);
        selectedItem.focus();
    }
}
