# 01 Art Direction

## Target vibe
Kid imagination; playful exploration; calm energy; “everything is a friendly playground.”

## Non-negotiable style pillars
1. Chunky toy-like forms; simple readable silhouettes.
2. Flat painted materials; minimal texture detail.
3. Bright palette; controlled contrast so it stays chill.
4. Soft sunlight; no dramatic lighting.
5. Sticker-and-craft energy; cardboard, tape, doodles, signs.

## Global style stamp (append to prompts)
Stylized cozy 3D game art; top-down 3/4 view at ~60° tilt; chunky toy-like shapes; flat painted materials; minimal texture detail; bright but calm palette; soft sunlight; clean silhouettes; playful kid-imagination vibe; no realism; no gritty.

## Palette (do not expand early)
Use the 16-color palette in `PALETTE.json`.
Rules:
- Most surfaces are 1–2 flat colors.
- Gradients are rare; prefer shape over shading.
- Accents (yellow/coral/purple) are for interactables and rewards.

## Shape language
- Everything is rounded and safe.
- Bevel edges using geometry, not texture.
- Avoid thin parts that disappear from a top-down camera.

## Materials
Default: matte, non-metallic, slightly rough.
- No realistic surface noise.
- No complex normals.
- If you need texture: low-frequency hand-painted wash only.

## Lighting rules
- One directional sun; soft shadows.
- Ambient slightly warm.
- Time-of-day: late morning to afternoon.
- Fog: optional; very subtle.

## “No” list
- Photorealism
- Grit, grime, horror vibes
- Hyper-detailed textures
- Busy patterns
- Realistic anatomy

## Benchmark checks
If you zoom out to gameplay camera:
- Can you identify objects in 0.5 seconds?
- Can you tell what is interactable instantly?
- Does it still look calm and friendly?
