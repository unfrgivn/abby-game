extends Resource
class_name EnemyDef
## Definition for an enemy type.
## Use as a .tres file in res://data/enemies/

@export var id: String = ""
@export var name: String = ""
@export_range(1, 500) var max_hp: int = 20
@export_range(1, 20) var speed: int = 5
@export_range(1, 50) var attack_power: int = 5

## AI behavior pattern for PoC
@export_enum("Random", "Aggressive", "Defensive") var ai_pattern: String = "Random"
