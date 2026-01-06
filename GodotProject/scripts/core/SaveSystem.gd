extends Node
## SaveSystem - Handles save/load with versioned JSON and atomic writes.
## Saves to user:// directory for cross-platform compatibility.

const SAVE_VERSION := 1
const SAVE_FILE_NAME := "cloverhollow_save.json"
const SAVE_FILE_PATH := "user://" + SAVE_FILE_NAME
const TEMP_SAVE_PATH := "user://" + SAVE_FILE_NAME + ".tmp"

signal save_completed(success: bool)
signal load_completed(success: bool)


func _ready() -> void:
	print("[SaveSystem] Initialized. Save path: ", SAVE_FILE_PATH)


## Save current game state to disk (atomic write)
func save_game() -> bool:
	var game_state: Node = get_node_or_null("/root/GameState")
	if not game_state:
		push_error("[SaveSystem] GameState autoload not found!")
		save_completed.emit(false)
		return false
	
	var save_data := {
		"version": SAVE_VERSION,
		"timestamp": Time.get_unix_time_from_system(),
		"game_state": game_state.to_save_data(),
	}
	
	var json_string := JSON.stringify(save_data, "\t")
	
	# Write to temp file first (atomic write pattern)
	var temp_file := FileAccess.open(TEMP_SAVE_PATH, FileAccess.WRITE)
	if not temp_file:
		push_error("[SaveSystem] Failed to open temp file: ", FileAccess.get_open_error())
		save_completed.emit(false)
		return false
	
	temp_file.store_string(json_string)
	temp_file.close()
	
	# Rename temp to final (atomic on most filesystems)
	var err := DirAccess.rename_absolute(
		ProjectSettings.globalize_path(TEMP_SAVE_PATH),
		ProjectSettings.globalize_path(SAVE_FILE_PATH)
	)
	if err != OK:
		push_error("[SaveSystem] Failed to rename temp save: ", err)
		save_completed.emit(false)
		return false
	
	print("[SaveSystem] Game saved successfully.")
	save_completed.emit(true)
	return true


## Load game state from disk
func load_game() -> bool:
	if not FileAccess.file_exists(SAVE_FILE_PATH):
		print("[SaveSystem] No save file found. Starting new game.")
		load_completed.emit(false)
		return false
	
	var file := FileAccess.open(SAVE_FILE_PATH, FileAccess.READ)
	if not file:
		push_error("[SaveSystem] Failed to open save file: ", FileAccess.get_open_error())
		load_completed.emit(false)
		return false
	
	var json_string := file.get_as_text()
	file.close()
	
	var json := JSON.new()
	var parse_result := json.parse(json_string)
	if parse_result != OK:
		push_error("[SaveSystem] Failed to parse save JSON: ", json.get_error_message())
		load_completed.emit(false)
		return false
	
	var save_data: Dictionary = json.data
	
	# Version check and migration
	var version: int = save_data.get("version", 0)
	if version > SAVE_VERSION:
		push_error("[SaveSystem] Save file is from a newer version!")
		load_completed.emit(false)
		return false
	
	if version < SAVE_VERSION:
		save_data = _migrate_save_data(save_data, version)
	
	# Apply to GameState
	var game_state: Node = get_node_or_null("/root/GameState")
	if not game_state:
		push_error("[SaveSystem] GameState autoload not found!")
		load_completed.emit(false)
		return false
	
	var game_data: Dictionary = save_data.get("game_state", {})
	game_state.from_save_data(game_data)
	
	print("[SaveSystem] Game loaded successfully.")
	load_completed.emit(true)
	return true


## Check if a save file exists
func has_save() -> bool:
	return FileAccess.file_exists(SAVE_FILE_PATH)


## Delete the save file
func delete_save() -> bool:
	if not has_save():
		return true
	var err := DirAccess.remove_absolute(ProjectSettings.globalize_path(SAVE_FILE_PATH))
	if err != OK:
		push_error("[SaveSystem] Failed to delete save: ", err)
		return false
	print("[SaveSystem] Save deleted.")
	return true


## Migrate old save data to current version
func _migrate_save_data(data: Dictionary, from_version: int) -> Dictionary:
	print("[SaveSystem] Migrating save from v", from_version, " to v", SAVE_VERSION)
	# Add migration logic here as needed
	# Example:
	# if from_version < 2:
	#     data["game_state"]["new_field"] = default_value
	data["version"] = SAVE_VERSION
	return data
