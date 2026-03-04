
import { createMenuNavigations } from "./Menu.js";
import { gamesListClass } from "./navigation/navigationConstants.js";

let previousGamepad = null;
let keyboardInitialized = false;
export function init() {
  previousGamepad = {
      axes:[0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
      buttons:Array.from({length:18},() => ({ pressed:false, touched:false, value:0 }))
  };
  initKeyboardSupport();
  const focusedMenu = document.getElementById(gamesListClass);
  focusedMenu.classList.add("focused-menu");
  const game = focusedMenu.children[0];
  game.focus();
  // game.classList.add("focused-item");
  game.classList.add("selected-item");
  const deviceInfo = JSON.parse(webOSSystem.deviceInfo);
  const isSimulator = deviceInfo.modelName.toLowerCase().includes("simulator");
  const gamePadLoop = isSimulator ? mainSimulatorLoop : mainTvLoop;
  requestAnimationFrame(gamePadLoop);
  // setTimeout(HandleInput, 100);
}


function buttonPressed(button) {
  return button.pressed || button.value > 0;
}

function getOperaInputDictionary() {

}

function getSimulatorInputDictionary() {

}

function getWebOsInputDictionary() {

}

// const cross = 0;
const cross = 1;
// const circle = 1;
const circle = 2;
// const triangle = 2;
const triangle = 3;
// const square = 3;
const square = 0;
const l1 = 4;
const r1 = 5;
//[3]
const l2 = 6;
// [4]
const r2 = 7;
const share = 8;
const options = 9;
const l3 = 10;
const r3 = 11;
const up = 16;
const down = 13;
const left = 14;
const right = 15;
// const guide = 16;
const guide = 12;
const touchpad = 13;
const mute = 14;
const leftAnalogX = 0;
const leftAnalogY = 1;
const rightAnalogX = 2;
const rightAnalogY = 3;

const upFlag    = 1;
const downFlag  = 2;
const rightFlag = 4;
const leftFlag  = 8;
const idleDpad = 1.2857143878936768;
const notches = [
  { value:                  -1, dir: upFlag               },
  { value: 0.14285719394683838, dir: downFlag             },
  { value: -0.4285714030265808, dir: rightFlag            },
  { value:  0.7142857313156128, dir: leftFlag             },
  { value: -0.7142857313156128, dir: rightFlag | upFlag   },
  { value: -0.1428571343421936, dir: rightFlag | downFlag },
  { value:                   1, dir: leftFlag  | upFlag   },
  { value:  0.4285714626312256, dir: leftFlag  | downFlag }
];

function readDpadFromHatAxis(gp, axisIndex = 9) {
  initDpad(gp);

  const v =  getDpadAxis(gp, axisIndex);
  if (dpadIsIdle(v)) return false;

  const { best, bestDist } = findBestDpad(v);

  if (dpadPressed(bestDist)) {
    updateDpad(gp, best.dir);
  }
  return true;
}

function initDpad(gp) {
  gp.up    = false;
  gp.down  = false;
  gp.left  = false;
  gp.right = false;
}

function updateDpad(gp, dir) {
  gp.up    = GetDpadButton(dir,    upFlag);
  gp.down  = GetDpadButton(dir,  downFlag);
  gp.left  = GetDpadButton(dir,  leftFlag);
  gp.right = GetDpadButton(dir, rightFlag);
}

function GetDpadButton(dir, flag) {
  return (dir & flag) === flag;
}

function hasDpadAxis(gp, axisIndex) {
  return gp.axes && gp.axes.length > axisIndex;
}

function getDpadAxis(gp, axisIndex) {
  return hasDpadAxis(gp, axisIndex) ? gp.axes[axisIndex] : idleDpad;
}

function findBestDpad(v) {
  let best = notches[0];
  let bestDist = getDist(v, 0);
  for (let i = 1; i < notches.length; i++) {
    const d = getDist(v, i);
    if (d < bestDist) { bestDist = d; best = notches[i]; }
  }
  return { best, bestDist };
}

function getDist(v, i) {
  return Math.abs(v - notches[i].value);
}

function dpadIsIdle(v) {
  const idleEps = 0.15;
  return Math.abs(v - idleDpad) < idleEps;
}

function dpadPressed(dist) {
  const pressEps = 0.2;
  return dist < pressEps;
}

const directionalsEmbedInAxes = true;

const GAMEPAD_INTERVAL = 0;       // every frame (~16ms)
const UI_INTERVAL = 16;           // every frame
const CLOCK_INTERVAL = 60000;     // every 60 seconds

let lastGamepadTime = 0;
let lastUITime = 0;
let lastClockTime = 0;
function mainSimulatorLoop(timestamp) {
  update(timestamp, checkSimulatorDpads);
  requestAnimationFrame(mainSimulatorLoop);
}
function mainTvLoop(timestamp) {
  update(timestamp, checkTvDpads);
  requestAnimationFrame(mainTvLoop);
}

function update(timestamp, checkDpads) {
  if (timestamp - lastGamepadTime >= GAMEPAD_INTERVAL) {
    pollGamepad(checkDpads);
    lastGamepadTime = timestamp;
  }

  // if (timestamp - lastUITime >= UI_INTERVAL) {
  //     updateAnimations(timestamp);
  //     lastUITime = timestamp;
  // }

  // if (timestamp - lastClockTime >= CLOCK_INTERVAL) {
  //     updateClock();
  //     lastClockTime = timestamp;
  // }
}

function pollGamepad(checkDpads) {
  const gp = navigator.getGamepads()[0];
  if (!gp) return;

  for (let i = 0; i < gp.buttons.length; i++) {
    var button = gp.buttons[i];
    if (buttonPressed(button) && !buttonPressed(previousGamepad.buttons[i])) {
      handleInput(i);
    }
  }

  checkDpads(gp);
  // previousGamepad.axes = gp.axes.slice(0);
  // previousGamepad.buttons = gp.buttons.map(b => ({ pressed:b.pressed, touched:b.touched, value:b.value }));
  previousGamepad = gp;
}

function GetAxisIndex(newAxis, oldAxis, negIndex, posIndex) {
  return newAxis > 0 ? posIndex
       : newAxis < 0 ? negIndex
       : oldAxis > 0 ? posIndex
                     : negIndex;
}

function checkTvDpads(gp) {
  gp.left  = gp.axes[6] === -1;
  gp.right = gp.axes[6] ===  1;
  gp.up    = gp.axes[7] === -1;
  gp.down  = gp.axes[7] ===  1;

  CheckDpadButton(gp.up, previousGamepad.up, up);
  CheckDpadButton(gp.down, previousGamepad.down, down);
  CheckDpadButton(gp.left, previousGamepad.left, left);
  CheckDpadButton(gp.right, previousGamepad.right, right);
}

function checkSimulatorDpads(gp) {
  if (!directionalsEmbedInAxes || !readDpadFromHatAxis(gp)) return;

  CheckDpadButton(gp.up, previousGamepad.up, up);
  CheckDpadButton(gp.down, previousGamepad.down, down);
  CheckDpadButton(gp.left, previousGamepad.left, left);
  CheckDpadButton(gp.right, previousGamepad.right, right);
}

function CheckDpadButton(dpad, previousDpad, index) {
  if (dpad && !previousDpad) {
    handleInput(index);
  }
}

const menuNavigations = createMenuNavigations();

function initKeyboardSupport() {
  if (keyboardInitialized) {
    return;
  }
  window.addEventListener("keydown", handleKeyboardInput);
  keyboardInitialized = true;
}

function handleKeyboardInput(evt) {
  if (evt.repeat || isTypingTarget(evt.target)) {
    return;
  }

  const index = mapKeyboardEventToInput(evt);
  if (index === null) {
    return;
  }

  evt.preventDefault();
  handleInput(index);
}

function isTypingTarget(target) {
  if (!target || !(target instanceof HTMLElement)) {
    return false;
  }

  if (target.isContentEditable) {
    return true;
  }

  const tag = target.tagName;
  return tag === "INPUT" || tag === "TEXTAREA" || tag === "SELECT";
}

function mapKeyboardEventToInput(evt) {
  switch (evt.key) {
    case "ArrowUp": return up;
    case "ArrowDown": return down;
    case "ArrowLeft": return left;
    case "ArrowRight": return right;
    case "Enter":
    case " ": 
    case "Spacebar":
    case "x":
      return cross;
    case "Escape":
    case "Home":
      return guide;
    case "Backspace":
      return circle;
    case "Alt":
      return options;
    default:
      return null;
  }
}

function getFocusedMenu() {
  let focusedMenu = document.activeElement.parentElement;
  if (focusedMenu == null && gameState !== gameState_running) {
    focusedMenu = document.getElementById(gamesListClass);
    focusedMenu.classList.add("focused-menu");
    let game = focusedMenu.children[0];
    game.focus();
    game.classList.add("selected-game");
  }
  return focusedMenu;
}

const gameState_stopped = 0;
const gameState_queued = 1;
const gameState_running = 2;
let gameState = gameState_stopped;
function handleInput(index) {
  const focusedMenu = getFocusedMenu();
  if (!focusedMenu) {
    return;
  }
  const nav = menuNavigations[focusedMenu.id];
  if (!nav) {
    return;
  }
  switch(index) {
      case up:      nav.moveUp();    break;
      case down:    nav.moveDown();  break;
      case left:    nav.moveLeft();  break;
      case right:   nav.moveRight(); break;
      case cross:   nav.select();    break;
      case circle:  nav.back();      break;
      case options: nav.options();   break;
      case guide:
        if (nav.Id === "overlay") {
          nav.back();
        }
        else {
          revealOverlay(focusedMenu);
        }
        break;
      default: break;
  }
}

function revealOverlay(focusedMenu) {
  if (focusedMenu) {
    focusedMenu.classList.remove("focused-menu");
    focusedMenu.classList.add("previous-menu");
  }

  const overlayMenu = document.getElementById("overlay");
  overlayMenu.classList.add("focused-menu");
  overlayMenu.classList.remove("hide");
  const menuItem = overlayMenu.children[0];
  menuItem.focus();

}

function checkThumbsticks(gp) {
  let length = gp.axes.length > 3 ? 3 : gp.axes.length;
  for (let i = 0; i < length; i++) {
    var newAxis = gp.axes[i];
    var greaterThanDeadzoneMin = newAxis > -0.1;
    var withinDeadzone = greaterThanDeadzoneMin && newAxis < 0.1;
    gp.axes[i] = newAxis = withinDeadzone         ? 0
                          : greaterThanDeadzoneMin ? newAxis - 0.1
                                                  : newAxis + 0.1;

    let oldAxis = previousGamepad.axes[i];
    if (newAxis !== previousAxis) {
      let negIndex, posIndex;
      switch(i) {
        case leftAnalogX:
        case rightAnalogX:
            negIndex = left;
            posIndex = right;
            break;
        case leftAnalogY:
        case rightAnalogY:
            negIndex = up;
            posIndex = down;
            break;
      }
      let index = GetAxisIndex(newAxis, oldAxis, negIndex, posIndex);
      bufferPush({ button:{ pressed:newAxis !== 0, touched:false, value:newAxis }, index:index });
    }
  }
}
