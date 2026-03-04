import { MenuNavigation } from "./MenuNavigation.js";
import {
    filtersClass,
    gamesListClass,
    selectedClass,
    topBarClass
} from "./navigationConstants.js";
import { getActiveElement } from "./navigationUtils.js";

export class FilterMenuNavigation extends MenuNavigation {
    constructor() {
        super(filtersClass, topBarClass, gamesListClass);
    }

    select() {
        const filter = getActiveElement();
        if (!filter) {
            return;
        }

        super.select();
        if (filter.classList.contains(selectedClass)) {
            filter.classList.remove(selectedClass);
        } else {
            filter.classList.add(selectedClass);
        }
    }

    options() {}
}
