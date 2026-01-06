extends Resource
class_name EncounterDef
## Definition for an overworld encounter.
## Use as a .tres file in res://data/encounters/

@export var id: String = ""

## Enemy IDs for this encounter
@export var enemy_ids: Array[String] = []

## Sticker ID to reward on first win (empty = no reward)
@export var first_win_sticker_reward_id: String = ""

## Gems rewarded on victory
@export_range(0, 100) var gems_reward: int = 5
