# Wilds of Cloverhollow

This repository contains the **Godot** project and design documents for a cozy, top-down 3D adventure game built for **macOS + iOS**. The project is designed to be developed with the help of AI agents (OpenCode), using a clear product spec and repeatable workflows.

## Project overview

Wilds of Cloverhollow is a family-friendly adventure with exploration, tool-driven secrets, and **turn-based Sticker battles**.

You play as Fae, a 10-year-old kid navigating school life and a magical “secret quest” to calm chaotic critters and stop mischief spreading through town. The narrative leans on kid humor, imagination, and gentle stakes.

### Core vibe
- **Kid imagination:** everyday objects become adventure tools.
- **Playful exploration:** safe spaces, readable navigation, plenty of small secrets.
- **Calm energy:** cozy atmosphere with non-scary challenges.
- **Sticker & scrapbook identity:** UI and progression are framed as collectible stickers in a journal.

## Gameplay & features

### Overworld
- **Top-down 3D view:** fixed-tilt “living diorama” camera.
- **Blacklight Lantern:** reveal hidden notes and doors (PoC scope).
- **Journal system:** collects discovered notes and clues.
- **Visible encounters:** enemies appear in-world; touching them transitions into battle.

### Battles
- **Turn-based battles (EarthBound / classic Final Fantasy cadence):** fast, readable command menus.
- **Sticker moves:** collectible “Sticker” moves equipped into a small loadout; battles use the equipped stickers.
- **Non-violent framing:** you “calm” chaos critters; no death.

## Style kit

The visual style is a “Living Toy / Playable Cartoon” look with soft shapes, handmade craft details, and sticker UI.

See `docs/style-kit/` for palette, camera rules, UI metaphors, and asset specs.

## Technical setup

- Godot project folder: `./GodotProject`
- Automation scripts: `./scripts`
- Setup instructions: `docs/setup.md`

