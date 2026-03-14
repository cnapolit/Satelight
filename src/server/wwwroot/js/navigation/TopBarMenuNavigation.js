import { MenuNavigation } from "./MenuNavigation.js";
import { filtersId, topBarId } from "./navigationConstants.js";

export class TopBarMenuNavigation extends MenuNavigation {
    constructor() {
        super(topBarId, null, filtersId);
    }

    options() {}
}
