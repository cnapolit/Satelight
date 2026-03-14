import { MenuNavigation } from "./MenuNavigation.js";
import {
    filtersId,
    gamesListId,
    selectedClass,
    topBarId
} from "./navigationConstants.js";
import { getActiveElement } from "./navigationUtils.js";

export class FilterMenuNavigation extends MenuNavigation {
    constructor() {
        super(filtersId, topBarId, gamesListId);
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
