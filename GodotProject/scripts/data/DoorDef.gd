extends Resource
class_name DoorDef
## Definition for a hidden door revealed by the Blacklight Lantern.

## Stable ID for persistence
@export var id: String = ""

## Display name (for UI hints)
@export var name: String = ""

## Target scene path to load when door is used
@export var target_scene: String = ""

## Target anchor ID within the target scene (spawn point)
@export var target_anchor_id: String = ""
