import { GameMenuNavigation } from "./navigation/GameMenuNavigation.js";
import { FilterMenuNavigation } from "./navigation/FilterMenuNavigation.js";
import { TopBarMenuNavigation } from "./navigation/TopBarMenuNavigation.js";
import { GameDetailsMenuNavigation } from "./navigation/GameDetailsMenuNavigation.js";
import { GameOptionsMenuNavigation } from "./navigation/GameOptionsMenuNavigation.js";
import { GuideMenuNavigation } from "./navigation/GuideMenuNavigation.js";
import { GameStartDialogueMenuNavigation } from "./navigation/GameStartDialogueMenuNavigation.js";

export function createMenuNavigations() {
    const gameMenuNav = new GameMenuNavigation();
    const topBarMenuNav = new TopBarMenuNavigation();
    const filterMenuNav = new FilterMenuNavigation();
    const gameDetailsMenuNav = new GameDetailsMenuNavigation();
    const GameOptionsMenuNav = new GameOptionsMenuNavigation();
    const gameStartDialogueMenuNav = new GameStartDialogueMenuNavigation();
    const guideMenuNav = new GuideMenuNavigation();
    return {
        [gameMenuNav.Id]: gameMenuNav,
        [topBarMenuNav.Id]: topBarMenuNav,
        [filterMenuNav.Id]: filterMenuNav,
        [gameDetailsMenuNav.Id]: gameDetailsMenuNav,
        [GameOptionsMenuNav.Id]: GameOptionsMenuNav,
        [gameStartDialogueMenuNav.Id]: gameStartDialogueMenuNav,
        [guideMenuNav.Id]: guideMenuNav
    };
}
