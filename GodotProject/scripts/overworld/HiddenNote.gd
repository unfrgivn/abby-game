extends Area3D
## HiddenNote - A note that is invisible until revealed by the Blacklight Lantern.
## When revealed, shows a popup and adds to GameState.discovered_notes.

signal revealed(note_id: String)

@export var note_def: Resource  # NoteDef

var is_revealed: bool = false
var _visual: Node3D
var _game_state: Node


func _ready() -> void:
	_game_state = get_node_or_null("/root/GameState")
	
	# Set collision layer to 5 (hidden objects) for lantern detection
	collision_layer = 16  # 2^4 = layer 5
	collision_mask = 0
	
	# Check if already discovered
	if note_def and _game_state:
		var note_id: String = note_def.get("id") if note_def.get("id") else ""
		if note_id in _game_state.discovered_notes:
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


func reveal() -> void:
	if is_revealed:
		return
	
	if not note_def:
		push_warning("[HiddenNote] No note_def assigned!")
		return
	
	var note_id: String = note_def.get("id") if note_def.get("id") else ""
	if note_id == "":
		push_warning("[HiddenNote] note_def has no id!")
		return
	
	is_revealed = true
	_show_visual()
	
	# Register discovery with GameState
	if _game_state:
		_game_state.discover_note(note_id)
	
	revealed.emit(note_id)
	print("[HiddenNote] Revealed: ", note_id)
	
	# Show popup (basic version - just prints for now)
	var title: String = note_def.get("title") if note_def.get("title") else "Note"
	var body: String = note_def.get("body") if note_def.get("body") else ""
	_show_note_popup(title, body)


func _show_note_popup(title: String, body: String) -> void:
	# TODO: Show actual UI popup. For now, just print.
	print("[HiddenNote] === NOTE DISCOVERED ===")
	print("[HiddenNote] Title: ", title)
	print("[HiddenNote] Body: ", body)
	print("[HiddenNote] ======================")
