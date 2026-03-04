import { MenuNavigation } from "./MenuNavigation.js";
import {
    filtersClass,
    focusedMenuClass,
    gameDetailsClass,
    gamesListClass,
    hideClass,
    selectedClass
} from "./navigationConstants.js";
import { getActiveElement } from "./navigationUtils.js";

const delay = 500;
export class GameMenuNavigation extends MenuNavigation {
    #timeSelected = new Date().getTime();
    #selectedItem = null;
    #isActive = false;
    constructor() {
        super(gamesListClass, filtersClass, null);
    }

    moveLeft() { this.moveItem(false); }
    moveRight() { this.moveItem(true); }

    moveItem(increment) {
        const oldItem = this.changeFocus(increment);
        if (!oldItem) {
            return;
        }
        
        document.activeElement.classList.add(selectedClass);
        oldItem.classList.remove(selectedClass);
        this.#timeSelected = new Date().getTime();
        this.#selectedItem = getActiveElement();
        if (!this.#isActive) {
            this.#isActive = true;
            setTimeout(() => this.clickItem(), delay);
        }
    }

    clickItem() {
        if (new Date().getTime() - this.#timeSelected > delay) {
            this.#selectedItem.click();
            this.#isActive = false;
        }
        else {
            setTimeout(() => this.clickItem(), delay);
        }
    }

    select() {
        const focusedItem = getActiveElement();
        if (!focusedItem) {
            return;
        }
        focusedItem.click();

        const blurredBackground = document.querySelector("#blurred-background");
        blurredBackground.classList.add(hideClass);

        const games = document.querySelector("#" + gamesListClass);
        games.classList.remove(focusedMenuClass);

        const filters = document.querySelector("#" + filtersClass);
        filters.classList.add(hideClass);

        const background = document.querySelector("#games-background");
        background.classList.add("game-details");

        const gameDetails = document.querySelector("#" + gameDetailsClass);
        gameDetails.classList.add(focusedMenuClass);
        gameDetails.classList.remove(hideClass);
        focusedItem.click();
        gameDetails.children[0].focus();
    }
}
