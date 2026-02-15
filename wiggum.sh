#!/usr/bin/env bash
set -euo pipefail

# Usage: ./wiggum.sh [max_loops]
# Defaults: MAX_LOOPS=50, SLEEP_SECONDS=5
MAX_LOOPS="${1:-${MAX_LOOPS:-50}}"
SLEEP_SECONDS="${SLEEP_SECONDS:-5}"

echo "Select CLI tool:"
echo "1) claude"
echo "2) opencode"
read -p "Enter choice (1 or 2): " choice
if [[ "$choice" == "1" ]]; then
  cli_cmd="claude"
  cli_args="-p --output-format stream-json"
else
  cli_cmd="opencode"
  cli_args="run"
fi

loops=0

while (( loops < MAX_LOOPS )); do
  loops=$((loops + 1))
  echo "---- Loop ${loops}/${MAX_LOOPS} ----"

  # Stream output live and capture it for completion detection.
  tmpfile=$(mktemp)
  $cli_cmd $cli_args "Read @agent/prompt.md and execute. Commit the changes after a task is complete." 2>&1 | tee "$tmpfile"
  output=$(cat "$tmpfile")
  rm -f "$tmpfile"

  if grep -q "<promise>COMPLETED</promise>" <<<"$output"; then
    echo "Completed detected. Exiting."
    exit 0
  fi

  sleep "$SLEEP_SECONDS"
done

echo "Max loops reached without completion."
exit 1
