extends Control
class_name StickerBook
## Sticker Book UI - displays owned stickers and manages 4-slot loadout.

signal closed()

@onready var inventory_grid: GridContainer = %InventoryGrid
@onready var loadout_slots: Array[Button] = [
	%LoadoutSlot1,
	%LoadoutSlot2,
	%LoadoutSlot3,
	%LoadoutSlot4,
]
@onready var detail_panel: PanelContainer = %DetailPanel
@onready var detail_name: Label = %DetailName
@onready var detail_type: Label = %DetailType
@onready var detail_description: Label = %DetailDescription
@onready var detail_stats: Label = %DetailStats
@onready var close_button: Button = %CloseButton

var _selected_sticker_id: String = ""
var _selected_loadout_slot: int = -1

# Button template for inventory items
const STICKER_BUTTON_SIZE := Vector2(120, 80)


func _ready() -> void:
	close_button.pressed.connect(_on_close_pressed)
	
	# Connect loadout slots
	for i in range(4):
		loadout_slots[i].pressed.connect(_on_loadout_slot_pressed.bind(i))
	
	_refresh_ui()
	_hide_detail_panel()


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("ui_cancel"):
		_on_close_pressed()
		get_viewport().set_input_as_handled()


## Refresh the entire UI from GameState
func _refresh_ui() -> void:
	_refresh_inventory()
	_refresh_loadout()


## Refresh inventory grid with owned stickers
func _refresh_inventory() -> void:
	# Clear existing buttons
	for child in inventory_grid.get_children():
		child.queue_free()
	
	var game_state: Node = get_node_or_null("/root/GameState")
	var data_registry: Node = get_node_or_null("/root/DataRegistry")
	
	if not game_state or not data_registry:
		return
	
	for sticker_id in game_state.owned_stickers:
		var sticker_def: Resource = data_registry.get_sticker(sticker_id)
		if sticker_def:
			var btn := _create_sticker_button(sticker_def)
			inventory_grid.add_child(btn)


## Create a button for a sticker in inventory
func _create_sticker_button(sticker_def: Resource) -> Button:
	var btn := Button.new()
	btn.custom_minimum_size = STICKER_BUTTON_SIZE
	btn.text = sticker_def.name
	btn.tooltip_text = sticker_def.description
	
	# Check if equipped
	var game_state: Node = get_node_or_null("/root/GameState")
	if game_state and sticker_def.id in game_state.equipped_stickers:
		var slot: int = game_state.equipped_stickers.find(sticker_def.id)
		btn.text += "\n[Slot %d]" % (slot + 1)
	
	btn.pressed.connect(_on_inventory_sticker_pressed.bind(sticker_def.id))
	return btn


## Refresh loadout slot display
func _refresh_loadout() -> void:
	var game_state: Node = get_node_or_null("/root/GameState")
	var data_registry: Node = get_node_or_null("/root/DataRegistry")
	
	if not game_state or not data_registry:
		return
	
	for i in range(4):
		var sticker_id: String = game_state.equipped_stickers[i]
		if sticker_id != "":
			var sticker_def: Resource = data_registry.get_sticker(sticker_id)
			if sticker_def:
				loadout_slots[i].text = sticker_def.name
			else:
				loadout_slots[i].text = sticker_id
		else:
			loadout_slots[i].text = "(Empty)"
		
		# Highlight selected slot
		if i == _selected_loadout_slot:
			loadout_slots[i].modulate = Color(1.2, 1.2, 0.8)
		else:
			loadout_slots[i].modulate = Color.WHITE


## Show sticker detail panel
func _show_detail_panel(sticker_id: String) -> void:
	var data_registry: Node = get_node_or_null("/root/DataRegistry")
	if not data_registry:
		return
	
	var sticker_def: Resource = data_registry.get_sticker(sticker_id)
	if not sticker_def:
		return
	
	detail_name.text = sticker_def.name
	detail_type.text = "Type: %s | Target: %s" % [sticker_def.type, sticker_def.targeting]
	detail_description.text = sticker_def.description
	
	var cd_text := ""
	if sticker_def.cooldown_turns > 0:
		cd_text = " | Cooldown: %d turns" % sticker_def.cooldown_turns
	detail_stats.text = "Power: %d%s" % [sticker_def.power, cd_text]
	
	detail_panel.visible = true


## Hide detail panel
func _hide_detail_panel() -> void:
	detail_panel.visible = false
	_selected_sticker_id = ""


## Handle inventory sticker tap
func _on_inventory_sticker_pressed(sticker_id: String) -> void:
	_selected_sticker_id = sticker_id
	_show_detail_panel(sticker_id)
	
	# If a loadout slot is selected, equip immediately
	if _selected_loadout_slot >= 0:
		_equip_sticker_to_slot(_selected_loadout_slot, sticker_id)
		_selected_loadout_slot = -1


## Handle loadout slot tap
func _on_loadout_slot_pressed(slot: int) -> void:
	if _selected_loadout_slot == slot:
		# Deselect
		_selected_loadout_slot = -1
	else:
		_selected_loadout_slot = slot
		
		# If a sticker was already selected in inventory, equip it
		if _selected_sticker_id != "":
			_equip_sticker_to_slot(slot, _selected_sticker_id)
			_selected_sticker_id = ""
			_selected_loadout_slot = -1
	
	_refresh_loadout()


## Equip a sticker to a slot
func _equip_sticker_to_slot(slot: int, sticker_id: String) -> void:
	var game_state: Node = get_node_or_null("/root/GameState")
	if not game_state:
		return
	
	game_state.equip_sticker(slot, sticker_id)
	_refresh_ui()
	_hide_detail_panel()


## Unequip a sticker from a slot
func unequip_slot(slot: int) -> void:
	var game_state: Node = get_node_or_null("/root/GameState")
	if not game_state:
		return
	
	game_state.equip_sticker(slot, "")
	_refresh_ui()


## Handle close button
func _on_close_pressed() -> void:
	closed.emit()
	hide()
