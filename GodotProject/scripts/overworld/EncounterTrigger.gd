extends Area3D
class_name EncounterTrigger
## Visible overworld encounter that triggers battle when player enters.
## Place in world scenes with a collision shape and set encounter_id.

@export var encounter_id: String = ""

## Optional: Hide after first win (enemy defeated)
@export var hide_after_first_win: bool = true

var _triggered := false


func _ready() -> void:
	# Check if should be hidden (already defeated)
	if hide_after_first_win:
		var game_state: Node = get_node_or_null("/root/GameState")
		if game_state and game_state.is_first_win_claimed(encounter_id):
			visible = false
			monitoring = false
			return
	
	# Connect collision signal
	body_entered.connect(_on_body_entered)


func _on_body_entered(body: Node3D) -> void:
	if _triggered:
		return
	
	# Only trigger on player (layer 2)
	if not body.is_in_group("player") and body.name != "Player":
		# Fallback: check collision layer
		if body is CharacterBody3D and body.collision_layer & 2 == 0:
			return
	
	if encounter_id == "":
		push_warning("[EncounterTrigger] No encounter_id set!")
		return
	
	_triggered = true
	print("[EncounterTrigger] Player entered, starting battle: ", encounter_id)
	
	# Get player position for return
	var from_position := body.global_position
	
	# Start battle via SceneRouter
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		scene_router.start_battle(encounter_id, from_position)
	else:
		push_error("[EncounterTrigger] SceneRouter not found!")
		_triggered = false
