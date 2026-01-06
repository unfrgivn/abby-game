extends Node
## Main bootstrap scene - entry point for the game.
## This scene never unloads; content scenes are loaded via SceneRouter.

func _ready() -> void:
	print("[Main] Bootstrap ready.")
	
	# Load initial world scene via SceneRouter
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		scene_router.goto_scene("world")
	else:
		push_error("[Main] SceneRouter not found! Falling back to direct load.")
		var world_scene := preload("res://scenes/world/World.tscn")
		add_child(world_scene.instantiate())


func _input(event: InputEvent) -> void:
	# Debug: Press B to start a test battle
	if event is InputEventKey and event.pressed and event.keycode == KEY_B:
		_debug_start_battle()


func _debug_start_battle() -> void:
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		print("[Main] DEBUG: Starting test battle...")
		scene_router.start_battle("debug_encounter")
	else:
		push_error("[Main] SceneRouter not found!")
