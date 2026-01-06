extends Node
## Main bootstrap scene - entry point for the game.
## This scene never unloads; content scenes are loaded via SceneRouter.

## Sticker Book UI (lazy loaded)
var _sticker_book: Control = null
const STICKER_BOOK_SCENE := preload("res://scenes/ui/StickerBook.tscn")

## Journal UI (lazy loaded)
var _journal: Control = null
const JOURNAL_SCENE := preload("res://scenes/ui/Journal.tscn")

## Starter stickers granted on new game
const STARTER_STICKERS := ["bonk", "glitter_bandage", "pocket_sand"]


func _ready() -> void:
	print("[Main] Bootstrap ready.")
	
	_setup_new_game_if_needed()
	
	# Load initial world scene via SceneRouter
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		scene_router.goto_scene("world")
	else:
		push_error("[Main] SceneRouter not found! Falling back to direct load.")
		var world_scene := preload("res://scenes/world/World.tscn")
		add_child(world_scene.instantiate())


## Grant starter stickers if this is a new game
func _setup_new_game_if_needed() -> void:
	var game_state: Node = get_node_or_null("/root/GameState")
	if not game_state:
		return
	
	# If player has no stickers, grant starters
	if game_state.owned_stickers.is_empty():
		print("[Main] New game detected. Granting starter stickers...")
		for sticker_id in STARTER_STICKERS:
			game_state.add_sticker(sticker_id)
		
		# Auto-equip first 3 stickers
		for i in range(min(3, STARTER_STICKERS.size())):
			game_state.equip_sticker(i, STARTER_STICKERS[i])
		
		print("[Main] Starter stickers granted: ", STARTER_STICKERS)


func _input(event: InputEvent) -> void:
	# J key opens Journal
	if event.is_action_pressed("journal"):
		_toggle_journal()
	
	# I key opens Sticker Book
	if event.is_action_pressed("open_sticker_book"):
		_toggle_sticker_book()
	
	# Debug: Press B to start a test battle
	if event is InputEventKey and event.pressed and event.keycode == KEY_B:
		_debug_start_battle()


## Toggle Journal UI
func _toggle_journal() -> void:
	# Close sticker book if open
	if _sticker_book and _sticker_book.visible:
		_sticker_book.hide()
	
	if _journal and _journal.visible:
		_journal.toggle()
		return
	
	if not _journal:
		_journal = JOURNAL_SCENE.instantiate()
		add_child(_journal)
	
	_journal.toggle()


## Toggle Sticker Book UI
func _toggle_sticker_book() -> void:
	# Close journal if open
	if _journal and _journal.visible:
		_journal.toggle()
	
	if _sticker_book and _sticker_book.visible:
		_sticker_book.hide()
		return
	
	if not _sticker_book:
		_sticker_book = STICKER_BOOK_SCENE.instantiate()
		add_child(_sticker_book)
		_sticker_book.closed.connect(_on_sticker_book_closed)
	
	_sticker_book.show()
	_sticker_book._refresh_ui()


func _on_sticker_book_closed() -> void:
	pass  # Could resume gameplay if paused


func _debug_start_battle() -> void:
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		print("[Main] DEBUG: Starting test battle...")
		scene_router.start_battle("park_raccoon_01")
	else:
		push_error("[Main] SceneRouter not found!")
