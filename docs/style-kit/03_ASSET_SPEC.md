# 03 Asset Spec (3D)

## Units and scale
- 1 Unity unit = 1 meter
- Player height: 1.2 m (toy-like)
- Doorway props: ~1.6 m (still feels big and inviting)

## Player proportions
- Head: 40%–45% of total height
- Torso: 35%–40%
- Legs: 20%–25%
- Hands/feet: oversized (mittens/boots)
- Face: dot eyes + simple mouth; minimal features

## Poly budgets (guidance, not law)
Keep it simple; prioritize silhouette.
- Player: 2k–6k tris
- NPC: 1.5k–5k tris
- Trees: 200–1.5k tris
- Small props: 50–800 tris
- Landmarks: 2k–10k tris (split into chunks if needed)

## Textures
Default: none (flat colors or vertex colors).
If required:
- 512–1024 px for hero props
- 256–512 px for small props
Avoid:
- high-frequency noise
- realistic photo textures

## Materials
- Matte, non-metallic
- Roughness: mid to high
- Metallic: 0
- Optional subtle rim light (very faint)

## Naming conventions
`cat_item_variant_size`
Examples:
- `prop_tree_round_m`
- `prop_sign_arrow_s`
- `npc_frog_shopkeeper`
- `collect_sticker_star`

## Collisions
- Use primitive colliders whenever possible
- Keep collisions forgiving; players should not get “stuck” on corners

## LOD
Only add LOD if performance demands it.
First: solve with fewer objects, simpler meshes, baked lighting.
