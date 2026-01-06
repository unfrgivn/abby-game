extends Node
## SceneRouter - Centralized scene loading and routing.
## Handles transitions between World and Battle scenes.

signal scene_changed(scene_name: String)
signal battle_started(encounter_id: String)
signal battle_ended(victory: bool)

## Scene path registry
const SCENES := {
	"world": "res://scenes/world/World.tscn",
	"battle": "res://scenes/battle/BattleScene.tscn",
}

## Current loaded content scene (child of Main)
var _current_scene: Node = null
var _current_scene_name: String = ""

## State for returning from battle
var _pre_battle_scene: String = ""
var _pre_battle_position: Vector3 = Vector3.ZERO
var _current_encounter_id: String = ""


func _ready() -> void:
	print("[SceneRouter] Initialized.")


## Get the Main node (bootstrap root)
func _get_main() -> Node:
	return get_node_or_null("/root/Main")


## Load a scene by name, replacing current content
func goto_scene(scene_name: String) -> bool:
	if scene_name not in SCENES:
		push_error("[SceneRouter] Unknown scene: ", scene_name)
		return false
	
	var main := _get_main()
	if not main:
		push_error("[SceneRouter] Main node not found!")
		return false
	
	# Remove current scene
	if _current_scene and is_instance_valid(_current_scene):
		_current_scene.queue_free()
		_current_scene = null
	
	# Load and instantiate new scene
	var scene_path: String = SCENES[scene_name]
	var packed_scene := load(scene_path) as PackedScene
	if not packed_scene:
		push_error("[SceneRouter] Failed to load scene: ", scene_path)
		return false
	
	_current_scene = packed_scene.instantiate()
	main.add_child(_current_scene)
	_current_scene_name = scene_name
	
	print("[SceneRouter] Loaded scene: ", scene_name)
	scene_changed.emit(scene_name)
	return true


## Start a battle from an encounter
func start_battle(encounter_id: String, from_position: Vector3 = Vector3.ZERO) -> bool:
	# Store return state
	_pre_battle_scene = _current_scene_name
	_pre_battle_position = from_position
	_current_encounter_id = encounter_id
	
	print("[SceneRouter] Starting battle: ", encounter_id)
	battle_started.emit(encounter_id)
	
	return goto_scene("battle")


## End battle and return to previous scene
func end_battle(victory: bool) -> bool:
	print("[SceneRouter] Battle ended. Victory: ", victory)
	battle_ended.emit(victory)
	
	var result := goto_scene(_pre_battle_scene if _pre_battle_scene != "" else "world")
	
	# Restore player position after scene loads
	if result and _pre_battle_position != Vector3.ZERO:
		# Defer to next frame to let scene initialize
		call_deferred("_restore_player_position")
	
	_current_encounter_id = ""
	return result


## Restore player position after returning from battle
func _restore_player_position() -> void:
	if not _current_scene:
		return
	
	var player := _current_scene.get_node_or_null("Player")
	if player and player is Node3D:
		player.global_position = _pre_battle_position
		print("[SceneRouter] Restored player position: ", _pre_battle_position)


## Get the current encounter ID (for battle scene to query)
func get_current_encounter_id() -> String:
	return _current_encounter_id


## Get current scene name
func get_current_scene_name() -> String:
	return _current_scene_name


## Debug: Force reload current scene
func reload_current_scene() -> bool:
	if _current_scene_name == "":
		return false
	return goto_scene(_current_scene_name)
