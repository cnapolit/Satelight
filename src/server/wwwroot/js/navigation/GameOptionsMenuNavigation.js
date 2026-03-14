import { MenuNavigation } from "./MenuNavigation.js";
import {
    focusedMenuClass,
    gameOptionsId,
    previousClass,
    selectedClass
} from "./navigationConstants.js";

export class GameOptionsMenuNavigation extends MenuNavigation {
    constructor() {
        super(gameOptionsId, null, null);
    }

    moveLeft() {}
    moveRight() {}

    moveUp() {
        super.moveLeft();
    }

    moveDown() {
        super.moveRight();
    }

    select() {
        super.select();
        this.back();
    }

    options() {
        this.back();
    }

    back() {
        let previousItem = document.querySelector("." + previousClass);
        if (!previousItem) {
            previousItem = document.querySelector("." + selectedClass);
            if (!previousItem) {
                console.error("No previous item found to return focus to.");
                previousItem = document.querySelector(".game-item");
            }
        }
        if (previousItem) {
            previousItem.classList.remove(previousClass);
            previousItem.focus();
            previousItem.parentElement.classList.add(focusedMenuClass);
        }
    }
}
