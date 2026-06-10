# Mobile Downhill Arcade Game

A fast-paced mobile racing application built in Unity, featuring real-time AI competition, custom gesture detection, and native hardware camera integration.

## 🛠️ Core Features & Engineering

* **Two-Level Race System vs AI:** Features linear track progression across two distinct maps where the player competes against a pathfinding AI racer.
* **Economic & Buff Systems:** Collectible coin triggers handle score updates, and dynamic item boxes reward players with active power-ups.
* **Native Camera Profile Integration:** Uses mobile hardware permissions to capture a real-time photo via the device camera, dynamically converting the texture data into a runtime player profile avatar.

## 🎮 Mobile Input Interaction Architecture

The game utilizes structured touch input polling and mobile hardware API integration to deliver responsive physics handling:

* **Movement Logic (Tap & Hold):** Evaluates touch screen-space coordinates partitioned by viewport halves. Holding the left or right side applies continuous horizontal forces to the player's rigid body.
* **Braking Mechanic (Swipe Down & Hold):** Detects a downward vertical delta vector across initial touch points, applying counter-velocity vectors to slow down momentum.
* **Double Tap (Boost):** Monitors localized touch-input timestamps; a secondary tap within a tight millisecond window triggers a forward impulse vector.
* **Shake Detection (Power-Up Trigger):** Evaluates hardware accelerometer changes. Exceeding a designated G-force threshold fires the execution event for the stored power-up.
