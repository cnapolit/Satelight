import { MenuNavigation } from "./MenuNavigation.js";
import { filtersClass, topBarClass } from "./navigationConstants.js";

export class TopBarMenuNavigation extends MenuNavigation {
    constructor() {
        super(topBarClass, null, filtersClass);
    }

    options() {}
}
