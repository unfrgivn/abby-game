extends Area3D
## HiddenDoor - A door that is invisible until revealed by the Blacklight Lantern.
## Once revealed, becomes interactable and can transition to another scene.

signal revealed(door_id: String)
signal door_used(door_id: String)

@export var door_def: Resource  # DoorDef

var is_revealed: bool = false
var _visual: Node3D
var _game_state: Node
var _scene_router: Node
var _player_in_range: bool = false


func _ready() -> void:
	_game_state = get_node_or_null("/root/GameState")
	_scene_router = get_node_or_null("/root/SceneRouter")
	
	# Set collision layer to 5 (hidden objects) for lantern detection
	collision_layer = 16  # 2^4 = layer 5
	collision_mask = 2  # Layer 2 = player
	
	body_entered.connect(_on_body_entered)
	body_exited.connect(_on_body_exited)
	
	# Check if already unlocked
	if door_def and _game_state:
		var door_id: String = door_def.get("id") if door_def.get("id") else ""
		if door_id in _game_state.unlocked_doors:
			is_revealed = true
			_show_visual()
	
	if not is_revealed:
		_hide_visual()


func _hide_visual() -> void:
	_visual = _find_visual_child()
	if _visual:
		_visual.visible = false


func _show_visual() -> void:
	_visual = _find_visual_child()
	if _visual:
		_visual.visible = true


func _find_visual_child() -> Node3D:
	for child in get_children():
		if child is MeshInstance3D or child is Sprite3D:
			return child
	return null


func _input(event: InputEvent) -> void:
	if not is_revealed or not _player_in_range:
		return
	
	if event.is_action_pressed("interact"):
		use_door()


func _on_body_entered(body: Node3D) -> void:
	if body.is_in_group("player"):
		_player_in_range = true


func _on_body_exited(body: Node3D) -> void:
	if body.is_in_group("player"):
		_player_in_range = false


func reveal() -> void:
	if is_revealed:
		return
	
	if not door_def:
		push_warning("[HiddenDoor] No door_def assigned!")
		return
	
	var door_id: String = door_def.get("id") if door_def.get("id") else ""
	if door_id == "":
		push_warning("[HiddenDoor] door_def has no id!")
		return
	
	is_revealed = true
	_show_visual()
	
	# Register unlock with GameState
	if _game_state:
		_game_state.unlock_door(door_id)
	
	revealed.emit(door_id)
	print("[HiddenDoor] Revealed: ", door_id)


func use_door() -> void:
	if not door_def:
		return
	
	var door_id: String = door_def.get("id") if door_def.get("id") else ""
	var target_scene: String = door_def.get("target_scene") if door_def.get("target_scene") else ""
	var target_anchor: String = door_def.get("target_anchor_id") if door_def.get("target_anchor_id") else ""
	
	door_used.emit(door_id)
	print("[HiddenDoor] Using door: ", door_id, " -> ", target_scene)
	
	if target_scene != "" and _scene_router:
		# TODO: Implement SceneRouter.goto_scene(target_scene, target_anchor)
		print("[HiddenDoor] Would transition to: ", target_scene, " at anchor: ", target_anchor)
