extends Area3D
## ClawMachine - Interactable object that launches the claw machine mini-game.

@export var prize_table_path: String = "res://data/tables/claw_machine_prizes.tres"

const CLAW_GAME_SCENE := preload("res://scenes/arcade/ClawMachineGame.tscn")

var _player_in_range: bool = false
var _prize_table: Resource


func _ready() -> void:
	collision_layer = 8  # Layer 4 = interactables
	collision_mask = 2   # Layer 2 = player
	
	body_entered.connect(_on_body_entered)
	body_exited.connect(_on_body_exited)
	
	# Load prize table
	if ResourceLoader.exists(prize_table_path):
		_prize_table = load(prize_table_path)


func _input(event: InputEvent) -> void:
	if not _player_in_range:
		return
	
	if event.is_action_pressed("interact"):
		start_game()


func _on_body_entered(body: Node3D) -> void:
	if body.is_in_group("player"):
		_player_in_range = true
		print("[ClawMachine] Player in range. Press E to play!")


func _on_body_exited(body: Node3D) -> void:
	if body.is_in_group("player"):
		_player_in_range = false


func start_game() -> void:
	print("[ClawMachine] Starting claw machine game!")
	
	var game_ui := CLAW_GAME_SCENE.instantiate()
	game_ui.prize_table = _prize_table
	
	# Add to scene tree root for UI overlay
	get_tree().root.add_child(game_ui)
	
	# Pause game tree while playing mini-game
	get_tree().paused = true
	
	game_ui.game_finished.connect(_on_game_finished.bind(game_ui))


func _on_game_finished(_prize: Resource, game_ui: Control) -> void:
	# Game will free itself, just unpause
	await get_tree().create_timer(0.1).timeout
	if is_instance_valid(game_ui):
		game_ui.queue_free()
	get_tree().paused = false
