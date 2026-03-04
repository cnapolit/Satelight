#!/usr/bin/env bash
set -euo pipefail

SIM_APP_PATH="/Users/cameronn/Documents/Grpc/PlayniteServer/SDK/TV/Simulator/webOS_TV_6.0_Simulator_1.4.1/webOS_TV_6.0_Simulator_1.4.1.app"
HOSTED_APP_PATH="/Users/cameronn/Documents/git/Remote-nite/HostedWebApp"

kill_sim() {
  kill -9 "$(pgrep -f "webOS_TV_6.0_Simulator /")" >/dev/null 2>&1 || true
}

on_term() {
  kill_sim
  exit 0
}

trap on_term INT TERM

# Close previous session
if pgrep -f "webOS_TV_6.0_Simulator" >/dev/null 2>&1; then
  kill_sim
  sleep 1
fi

# Launch (or bring up) the simulator/app
ares-launch --simulator 6.0 "$HOSTED_APP_PATH" >/dev/null 2>&1 || true

# Signal VS Code that the background task is "ready"
echo "WEBOS_SIM_WATCH: started"

# Wait until the Simulator process is gone
while pgrep -f "webOS_TV_6.0_Simulator" >/dev/null 2>&1; do
  sleep 1
done

echo "WEBOS_SIM_WATCH: exited"
exit 0