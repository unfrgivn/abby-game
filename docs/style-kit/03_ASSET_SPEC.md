# 03 Asset Spec (3D)

## Units and scale
- 1 world unit = 1 meter (Godot default scale)
- Player height: 1.0 m (Classic Chibi)
- Doorway props: ~1.5 m (Wide and accessible)

## Player proportions (Chibi SD Style)
- **Ratio:** 1:2 Head-to-Body ratio.
- **Head:** ~50% of total height. Big and expressive.
- **Hair:** Chunky tufts, messy bun (Fae's signature). No individual strands.
- **Body:** Small, compact torso.
- **Limbs:** Rounded "noodle" limbs or soft cylinders. No sharp elbows/knees.
- **Hands/Feet:** Oversized for readability. Mittens or minimal finger detail.
- **Face:** Wide set dot eyes, simple mouth, small blush stickers.

## Poly budgets (guidance)
Prioritize silhouette smoothness.
- **Player:** 4k–8k tris (Focus on round head/hair and backpack details).
- **NPC:** 2k–5k tris.
- **Trees:** 500–2k tris (Blobby canopies).
- **Small props:** 100–1k tris.

## Textures & Materials
- **Default Material:** "Digital Watercolor" / Soft Cel-Shade.
  - **Technique:** Flat base colors with soft gradient overlays.
  - **Outlines:** Colored outlines (brown/purple) inverted hull or rim shader. Avoid harsh black lines.
  - Roughness: High (0.8).
  - Metallic: 0.
- **Fae's Backpack:** Special "Holo-Sparkle" shader.
  - Iridescent gradients.
  - Glitter texture mask.
- **Faces:** Decal sheet or separate material for expression swapping.

## Naming conventions
`cat_item_variant_size`
Examples:
- `char_fae_base`
- `prop_tree_puff_l`
- `prop_sign_chunky_s`
- `vfx_sparkle_star`

## Collisions
- Use primitive Capsules and Spheres.
- Soft edges everywhere.

## LOD
- Aggressive LODs for mobile, but keep the silhouette "round" even at distance.
