# Wilds of Cloverhollow

This repository contains the Unity project and design documents for a cozy, top-down 3D adventure game. The project is built with Unity (URP) and is designed to be developed with the help of AI agents.

## Project Overview

The game is a family-friendly adventure with light RPG and puzzle elements, inspired by classics like Zelda, Animal Crossing, and Earthbound. The player takes on the role of Fae, a 10-year-old kid who must stop a mischievous friend from causing chaos in their small town, all while balancing school life. The narrative explores themes of imagination, friendship, and the magic of everyday life.

### Core Vibe
- **Kid Imagination:** A world seen through the eyes of a child, where everyday objects become tools for adventure.
- **Playful Exploration:** A safe and inviting world that encourages curiosity.
- **Calm Energy:** A cozy and relaxing atmosphere with non-scary challenges.

## Art & Style

The game's visual style is defined as "Living Toy" or "Playable Cartoon," with a strong emphasis on a handmade, craft-like aesthetic.

### Style Pillars
1.  **Chibi / SD Proportions:** Characters have a 1:2 head-to-body ratio, giving them a cute, toy-like appearance.
2.  **Soft & Round Shapes:** The world is built with beveled edges, spherical foliage, and smooth terrain to create a friendly and approachable environment.
3.  **Digital Watercolor & Cel-Shade:** Materials are flat-painted with soft gradients and colored outlines, avoiding harsh black lines.
4.  **Sticker & Craft Details:** UI and in-world elements feature holographic stickers, tape, and cardboard construction to enhance the "kid-made" feel.
5.  **Calm Brightness:** The palette consists of pastel-adjacent, saturated colors and soft sunlight, creating a bright and cheerful mood.

The UI follows a "Sticker & Scrapbook" metaphor, feeling like the main character's personal journal.

## Gameplay & Features

The core gameplay loop revolves around exploration, puzzle-solving, and light combat.

### Key Mechanics
-   **Top-Down 3D View:** The camera is fixed at a 60-degree tilt, providing a "living diorama" perspective that is both readable and immersive.
-   **Blacklight Lantern:** A key tool used to reveal hidden paths, secret notes, and magical symbols.
-   **Journal System:** Fae's journal collects discovered notes and clues, guiding the player through a "soft quest" system.
-   **Light Combat:** Friendly combat encounters with "chaos critters" that can be calmed rather than defeated.
-   **Animal Companions:** Players can befriend and call upon animal pals who assist in puzzles and battles.
-   **Clubhouse Building:** A customizable clubhouse that players can build and decorate throughout the game.

## Proof-of-Concept (PoC)

The current development focus is on a tight, polished proof-of-concept that demonstrates the core loop of the game.

### PoC Scope
-   **Core Loop:** Explore the starting town, use the blacklight to solve a small note trail, engage in a simple combat encounter, and play an arcade mini-game.
-   **Content:** The PoC includes the main town (Cloverhollow) and two interior locations (School and Arcade).
-   **Systems:** The PoC will establish a robust and data-driven architecture for input, saving, scene management, and interaction that can be extended for the full game.

## Technical Setup

The project is set up to be built with a pinned Unity LTS version and follows specific path and naming conventions to ensure consistency.

-   **Unity Project Path:** `./UnityProject`
-   **Testing:** The repository includes scripts for running playmode and editmode tests in batchmode.
-   **Asset Specs:** Detailed specifications for character proportions, poly budgets, and material properties are provided in the `docs/style-kit`.

For detailed setup instructions, see `docs/setup.md`.
