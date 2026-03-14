import {
    filtersId,
    focusedMenuClass,
    hideClass,
    selectedClass
} from "./navigationConstants.js";

export function getActiveElement() {
    let activeElement = document.activeElement;
    if (activeElement !== document.body) {
        return activeElement;
    }

    let focusedMenu = document.querySelector("." + focusedMenuClass);
    if (focusedMenu && focusedMenu.childElementCount === 0) {
        // refocus menu with items
        focusedMenu.classList.remove(focusedMenuClass);
        focusedMenu = null;
    }

    if (!focusedMenu || focusedMenu.childElementCount === 0) {
        focusedMenu = document.querySelector("#" + filtersId);
        focusedMenu.classList.add(focusedMenuClass);
    }

    let focusedItem = document.querySelector("." + selectedClass);
    if (!focusedItem) {
        focusedItem = document.children[0];
    }
    focusedItem.focus();
    return null;
}

export function changeMenu(newMenuClass, hide = false) {
    if (newMenuClass === null) return;

    const currentFocusedMenu = document.querySelector("." + focusedMenuClass);

    const newMenu = document.querySelector("#" + newMenuClass);
    if (newMenu === null) return;

    currentFocusedMenu.classList.remove(focusedMenuClass);
    newMenu.classList.add(focusedMenuClass);

    if (hide) {
        currentFocusedMenu.classList.add(hideClass);
        newMenu.classList.remove(hideClass);
    }

    let selectedItem = newMenu.querySelector("." + selectedClass);
    if (!selectedItem) {
        selectedItem = newMenu.children[0];
    }
    selectedItem.focus();
}
