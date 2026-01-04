# 02 Camera & Controls (Toy Box View)

## Camera Philosophy: "The Living Diorama"
The camera should feel like looking down into a magical toy box. It is stable, readable, and emphasizes the "chunky" nature of the world.

## Core Camera Specs
- **Projection**: Perspective (FOV 40–50 for a flattened, toy-like look without being orthographic)
- **Tilt Angle**: 60° fixed (The sweet spot for reading top-down faces while seeing depth)
- **Distance**: Close enough to see character expressions (Player = ~15% screen height)
- **Smoothing**: High damping; the camera feels like a heavy, steady hand guiding the view.
- **Look-ahead**: Minimal. The world is safe; we don't need to see far ahead.

## "Toy Box" Occlusion Rules
Since we look down, tall objects are dangerous.
1.  **Trees**: Use the "Sphere Tree" standard (short trunks, high canopy).
2.  **Buildings**: Front facades only (dollhouse style) or short single-story structures with sloped roofs.
3.  **Fading**: Anything blocking the view must dither-fade instantly. No "cutout" circles; just soft transparency.

## Movement Feel: "Bouncy & Tactile"
Fae moves like a high-quality vinyl toy, not a simulation.
-   **Run**: A rhythmic bobbing motion (up/down) rather than a lean-forward sprint.
-   **Stop**: A slight squash-and-settle when stopping (no sliding).
-   **Turn**: Snappy rotation. No slow arcs.
-   **Collision**: Soft bounces off walls, never "sticking" or sliding along geometry.

## Interaction: Sticker Overlay
The UI lives *in* the world, not on a HUD.
-   **Icons**: White-outlined "stickers" pop up over interactables.
-   **Selection**: The nearest object gets a thick white outline (shader based).
-   **Feedback**: Tapping an object triggers a "squish" scale animation on the object itself.

## Touch Controls
-   **Stick**: Floating virtual joystick.
-   **Buttons**: Big, round, candy-colored buttons for Action/Tool.
-   **Haptics**: Light taps on every footstep (if supported) to ground the "toy" feeling.
