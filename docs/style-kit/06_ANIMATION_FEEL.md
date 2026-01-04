# 06 Animation Feel

## Core Vibe: "Living Toy"
Animation should feel like a high-quality toy come to life or a playable cartoon. It is **bouncy, snappy, and exaggerated**. 

Avoid "floaty" realistic blends. Prioritize strong poses and snappy timing.

## Principles
1.  **Head Weight**: Fae's head is 45% of her mass. It drags slightly on acceleration and overshoots on stop.
2.  **Squash & Stretch**: Essential for the "soft vinyl" toy feel.
    - **Jump**: Squash flat -> Stretch long -> Tuck in air -> Squash on land.
    - **Impact**: Deform the mesh on hits.
3.  **Snappy Timing**: Fast transitions between states (Idle -> Run). Avoid slow blend trees.

## Player Actions (Fae)
- **Idle**: 
    - Gentle vertical breathing (scale Y 100% -> 103%).
    - Blink logic: Random double-blinks.
- **Run (The "Trot")**:
    - High vertical bobbing.
    - Arms pump high (above shoulders).
    - Feet lift high (exaggerated steppies).
- **Interact / Bonk**:
    - 2-frame anticipation, 1-frame strike.
    - **Hit Stop**: Freeze frame for 0.05s on contact.

## Enemy Actions (Chaos Raccoon)
- **Telegraphs**: 
    - Wobbly "wind-up" before attacking (shake position, not just rotation).
    - "Exclamation" emote appears above head.
- **Dizzy**:
    - Spinning stars (holographic texture).
    - Eyes swirl texture swap.
- **Defeat**:
    - No ragdolls.
    - "Poof" into a cloud of round smoke particles and leaves.

## Technical Specs
- **Framerate**: Game runs at 60fps+.
- **Curves**: Use `BackOut` or `ElasticOut` for UI and prop movements.
- **Bones**: Minimal rigs. Use scale on bones to fake squash/stretch if blendshapes aren't available.

## VFX Support
- **Dust Puffs**: Round white spheres (unlit) on footfalls.
- **Swing Trails**: "Holo" gradient ribbon behind the lantern/weapon.
- **Impacts**: 2D "Comic Book" star shapes (Billboarded).
