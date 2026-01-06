extends Resource
class_name StickerDef
## Definition for a Sticker (battle move).
## Use as a .tres file in res://data/stickers/

@export var id: String = ""
@export var name: String = ""
@export_multiline var description: String = ""

@export_enum("Attack", "Support", "Utility") var type: String = "Attack"
@export_enum("SingleEnemy", "AllEnemies", "Self", "Ally") var targeting: String = "SingleEnemy"
@export_range(0, 100) var power: int = 10
@export_range(0, 5) var cooldown_turns: int = 0
