import { previousClass } from "./navigationConstants.js";
import { changeMenu, getActiveElement } from "./navigationUtils.js";

export class MenuNavigation {
    #id;
    #menuClass;
    #upperMenu;
    #lowerMenu;

    constructor(menuClass, upperMenu, lowerMenu) {
        this.#id = menuClass;
        this.#menuClass = menuClass;
        this.#upperMenu = upperMenu;
        this.#lowerMenu = lowerMenu;
    }

    get MenuClass() { return this.#menuClass; }
    get Id()        { return this.#id; }
    get Menu()      { return document.querySelector("#" + this.#id); }

    moveUp()    { changeMenu(this.#upperMenu); }
    moveDown()  { changeMenu(this.#lowerMenu); }
    moveLeft()  { return this.changeFocus(false); }
    moveRight() { return this.changeFocus(true); }

    select() {
        const activeElement = getActiveElement();
        if (!activeElement) {
            return;
        }
        activeElement.click();
        return activeElement;
    }

    back() {}

    options() {
        const activeElement = getActiveElement();
        if (!activeElement) {
            return;
        }
        activeElement.classList.add(previousClass);
        activeElement.dispatchEvent(new MouseEvent("contextmenu", {}));
    }

    changeFocus(increment) {
        const focusedItem = getActiveElement();
        return focusedItem ? this.focusNewItem(focusedItem, increment) : null;
    }

    focusNewItem(focusedItem, increment) {
        const newItem = this.getNewItem(focusedItem, increment);
        if (newItem === focusedItem) {
            return null;
        }

        newItem.focus();
        return focusedItem;
    }

    getNewItem(focusedItem, increment) {
        const children = document.getElementById(this.#id).children;
        const len = children.length;
        for (let i = 0; i < len; i++) {
            if (children[i] === focusedItem) {
                return this.getNextItem(children, i, increment, len);
            }
        }
        return null;
    }

    getNextItem(items, index, increment) {
        let newIndex = index;
        let newItem;
        const step = increment ? 1 : -1;
        do {
            newIndex = this.mod(newIndex + step, items.length);
            newItem = items[newIndex];
        } while (!newItem.attributes.getNamedItem("tabindex") && newIndex !== index);
        return newItem;
    }

    mod(n, m) { return ((n % m) + m) % m; }
}
