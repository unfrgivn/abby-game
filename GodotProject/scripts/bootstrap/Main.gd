extends Node
## Main bootstrap scene - entry point for the game.
## This scene never unloads; content scenes are loaded via SceneRouter.

func _ready() -> void:
	print("[Main] Bootstrap ready. Loading initial world scene...")
	# For now, directly load the World scene. 
	# Later this will go through SceneRouter autoload.
	var world_scene := preload("res://scenes/world/World.tscn")
	var world_instance := world_scene.instantiate()
	add_child(world_instance)
	print("[Main] World scene loaded.")
