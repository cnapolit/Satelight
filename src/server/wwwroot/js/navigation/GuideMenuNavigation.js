import { MenuNavigation } from "./MenuNavigation.js";
import {
    focusedMenuClass,
    hideClass,
    selectedClass
} from "./navigationConstants.js";

export class GuideMenuNavigation extends MenuNavigation {
    constructor() {
        super("overlay", null, null);
    }

    back() {
        const guidMenu = document.getElementById(this.Id);
        guidMenu.classList.add(hideClass);

        const menu = document.querySelector(".previous-menu");
        menu.classList.remove("previous-menu");
        menu.classList.add(focusedMenuClass);
        let item = menu.querySelector("." + selectedClass);
        if (!item) {
            item = menu.children[0];
        }
        item.focus();
    }
}