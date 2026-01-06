extends Node
## DataRegistry - Loads and provides access to all game data resources.
## Scans data folders on startup and indexes by ID.

var _stickers: Dictionary = {}  # id -> StickerDef
var _enemies: Dictionary = {}   # id -> EnemyDef
var _encounters: Dictionary = {}  # id -> EncounterDef


func _ready() -> void:
	_load_all()


func _load_all() -> void:
	_stickers = _load_resources_from_folder("res://data/stickers/", "StickerDef")
	_enemies = _load_resources_from_folder("res://data/enemies/", "EnemyDef")
	_encounters = _load_resources_from_folder("res://data/encounters/", "EncounterDef")
	
	print("[DataRegistry] Loaded stickers: ", _stickers.keys())
	print("[DataRegistry] Loaded enemies: ", _enemies.keys())
	print("[DataRegistry] Loaded encounters: ", _encounters.keys())


func _load_resources_from_folder(folder_path: String, expected_class: String) -> Dictionary:
	var result := {}
	var dir := DirAccess.open(folder_path)
	
	if not dir:
		push_warning("[DataRegistry] Could not open folder: ", folder_path)
		return result
	
	dir.list_dir_begin()
	var file_name := dir.get_next()
	
	while file_name != "":
		if not dir.current_is_dir() and (file_name.ends_with(".tres") or file_name.ends_with(".res")):
			var full_path := folder_path + file_name
			var resource := load(full_path)
			
			if resource and resource.get("id") != null:
				var id: String = resource.id
				if id != "":
					result[id] = resource
				else:
					push_warning("[DataRegistry] Resource has empty id: ", full_path)
			else:
				push_warning("[DataRegistry] Invalid resource: ", full_path)
		
		file_name = dir.get_next()
	
	dir.list_dir_end()
	return result


## Get a sticker by ID. Returns null if not found.
func get_sticker(sticker_id: String) -> Resource:
	return _stickers.get(sticker_id, null)


## Get all sticker IDs
func get_all_sticker_ids() -> Array[String]:
	var ids: Array[String] = []
	for key in _stickers.keys():
		ids.append(key)
	return ids


## Get an enemy by ID. Returns null if not found.
func get_enemy(enemy_id: String) -> Resource:
	return _enemies.get(enemy_id, null)


## Get an encounter by ID. Returns null if not found.
func get_encounter(encounter_id: String) -> Resource:
	return _encounters.get(encounter_id, null)


## Get all encounter IDs
func get_all_encounter_ids() -> Array[String]:
	var ids: Array[String] = []
	for key in _encounters.keys():
		ids.append(key)
	return ids
