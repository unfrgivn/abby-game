extends Node
## GameState - Global persistent player state.
## Holds inventory, flags, respawn anchor, and other session data.

signal sticker_acquired(sticker_id: String)
signal sticker_equipped(slot: int, sticker_id: String)
signal currency_changed(new_amount: int)

## Owned sticker IDs
var owned_stickers: Array[String] = []

## Equipped sticker loadout (4 slots, empty string = no sticker)
var equipped_stickers: Array[String] = ["", "", "", ""]

## Currency (gems)
var gems: int = 0

## Discovered notes (by note_id)
var discovered_notes: Array[String] = []

## Unlocked doors (by door_id)
var unlocked_doors: Array[String] = []

## Last respawn anchor ID
var last_anchor_id: String = "home_bed"

## Game flags for quest/story state
var flags: Dictionary = {}

## Encounter completion tracking (encounter_id -> bool for first_win_claimed)
var encounter_first_wins: Dictionary = {}


func _ready() -> void:
	print("[GameState] Initialized.")


## Add a sticker to owned collection (if not already owned)
func add_sticker(sticker_id: String) -> bool:
	if sticker_id in owned_stickers:
		return false
	owned_stickers.append(sticker_id)
	sticker_acquired.emit(sticker_id)
	print("[GameState] Acquired sticker: ", sticker_id)
	return true


## Equip a sticker to a slot (0-3)
func equip_sticker(slot: int, sticker_id: String) -> bool:
	if slot < 0 or slot >= 4:
		push_error("[GameState] Invalid equip slot: " + str(slot))
		return false
	if sticker_id != "" and sticker_id not in owned_stickers:
		push_error("[GameState] Cannot equip unowned sticker: " + sticker_id)
		return false
	
	# Unequip from other slots first
	for i in range(4):
		if i != slot and equipped_stickers[i] == sticker_id:
			equipped_stickers[i] = ""
	
	equipped_stickers[slot] = sticker_id
	sticker_equipped.emit(slot, sticker_id)
	return true


## Add gems
func add_gems(amount: int) -> void:
	gems += amount
	currency_changed.emit(gems)


## Discover a note
func discover_note(note_id: String) -> bool:
	if note_id in discovered_notes:
		return false
	discovered_notes.append(note_id)
	return true


## Unlock a door
func unlock_door(door_id: String) -> bool:
	if door_id in unlocked_doors:
		return false
	unlocked_doors.append(door_id)
	return true


## Check if encounter first win reward was claimed
func is_first_win_claimed(encounter_id: String) -> bool:
	return encounter_first_wins.get(encounter_id, false)


## Mark encounter first win as claimed
func claim_first_win(encounter_id: String) -> void:
	encounter_first_wins[encounter_id] = true


## Set a flag
func set_flag(flag_name: String, value: Variant = true) -> void:
	flags[flag_name] = value


## Get a flag (returns null if not set)
func get_flag(flag_name: String, default: Variant = null) -> Variant:
	return flags.get(flag_name, default)


## Reset to new game state
func reset_to_new_game() -> void:
	owned_stickers.clear()
	equipped_stickers = ["", "", "", ""]
	gems = 0
	discovered_notes.clear()
	unlocked_doors.clear()
	last_anchor_id = "home_bed"
	flags.clear()
	encounter_first_wins.clear()
	print("[GameState] Reset to new game.")


## Serialize state for saving
func to_save_data() -> Dictionary:
	return {
		"owned_stickers": owned_stickers.duplicate(),
		"equipped_stickers": equipped_stickers.duplicate(),
		"gems": gems,
		"discovered_notes": discovered_notes.duplicate(),
		"unlocked_doors": unlocked_doors.duplicate(),
		"last_anchor_id": last_anchor_id,
		"flags": flags.duplicate(true),
		"encounter_first_wins": encounter_first_wins.duplicate(),
	}


## Load state from save data
func from_save_data(data: Dictionary) -> void:
	owned_stickers = Array(data.get("owned_stickers", []), TYPE_STRING, "", null)
	equipped_stickers = Array(data.get("equipped_stickers", ["", "", "", ""]), TYPE_STRING, "", null)
	gems = data.get("gems", 0)
	discovered_notes = Array(data.get("discovered_notes", []), TYPE_STRING, "", null)
	unlocked_doors = Array(data.get("unlocked_doors", []), TYPE_STRING, "", null)
	last_anchor_id = data.get("last_anchor_id", "home_bed")
	flags = data.get("flags", {})
	encounter_first_wins = data.get("encounter_first_wins", {})
	print("[GameState] Loaded from save data.")
