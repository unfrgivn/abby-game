extends Control
## Journal - UI panel showing discovered notes from Blacklight Lantern scanning.

@onready var note_list: ItemList = $Panel/VBox/NoteList
@onready var note_title: Label = $Panel/VBox/DetailPanel/Title
@onready var note_body: RichTextLabel = $Panel/VBox/DetailPanel/Body
@onready var empty_label: Label = $Panel/VBox/EmptyLabel

var _game_state: Node
var _discovered_notes: Array = []  # Array of loaded NoteDef resources


func _ready() -> void:
	_game_state = get_node_or_null("/root/GameState")
	visible = false
	_setup_ui()


func _setup_ui() -> void:
	if note_list:
		note_list.item_selected.connect(_on_note_selected)


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("journal"):
		toggle()


func toggle() -> void:
	visible = not visible
	if visible:
		refresh()
		# Pause game while journal is open
		get_tree().paused = true
	else:
		get_tree().paused = false


func refresh() -> void:
	_load_discovered_notes()
	_populate_list()
	_update_empty_state()
	_clear_detail()


func _load_discovered_notes() -> void:
	_discovered_notes.clear()
	
	if not _game_state:
		return
	
	var note_ids: Array = _game_state.discovered_notes
	for note_id in note_ids:
		var note_def := _load_note_def(note_id)
		if note_def:
			_discovered_notes.append(note_def)


func _load_note_def(note_id: String) -> Resource:
	# Try loading from data/notes/ directory
	var path := "res://data/notes/%s.tres" % note_id
	if ResourceLoader.exists(path):
		return load(path)
	return null


func _populate_list() -> void:
	if not note_list:
		return
	
	note_list.clear()
	for note_def in _discovered_notes:
		var title: String = note_def.get("title") if note_def.get("title") else "Unknown Note"
		note_list.add_item(title)


func _update_empty_state() -> void:
	var has_notes := _discovered_notes.size() > 0
	
	if note_list:
		note_list.visible = has_notes
	if empty_label:
		empty_label.visible = not has_notes
	if has_node("Panel/VBox/DetailPanel"):
		get_node("Panel/VBox/DetailPanel").visible = has_notes


func _clear_detail() -> void:
	if note_title:
		note_title.text = ""
	if note_body:
		note_body.text = ""


func _on_note_selected(index: int) -> void:
	if index < 0 or index >= _discovered_notes.size():
		return
	
	var note_def: Resource = _discovered_notes[index]
	var title: String = note_def.get("title") if note_def.get("title") else ""
	var body: String = note_def.get("body") if note_def.get("body") else ""
	
	if note_title:
		note_title.text = title
	if note_body:
		note_body.text = body
