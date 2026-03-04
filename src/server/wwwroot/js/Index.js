import { init } from "./Gamepad.js";

window.browse = window.browse || {};

window.browse.init = (dotNetRef) => {
    window.browse.dotNetRef = dotNetRef;
};
window.onload = function() {
    init();
}