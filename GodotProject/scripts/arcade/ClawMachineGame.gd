extends Control
## ClawMachineGame - Mini-game UI for the arcade claw machine.
## Player moves claw left/right and drops to grab prizes.

signal game_finished(prize: Resource)

@export var prize_table: Resource  # PrizeTable

@onready var claw_sprite: Control = $GameArea/Claw
@onready var prize_label: Label = $PrizeLabel
@onready var instruction_label: Label = $Instructions

const CLAW_SPEED := 200.0
const CLAW_MIN_X := 50.0
const CLAW_MAX_X := 350.0
const DROP_DURATION := 0.8
const GRAB_DURATION := 0.5

var _claw_x: float = 200.0
var _is_dropping: bool = false
var _game_active: bool = true
var _game_state: Node


func _ready() -> void:
	_game_state = get_node_or_null("/root/GameState")
	_claw_x = (CLAW_MIN_X + CLAW_MAX_X) / 2.0
	_update_claw_position()
	prize_label.visible = false
	instruction_label.text = "← → to move, SPACE to drop!"


func _process(delta: float) -> void:
	if not _game_active or _is_dropping:
		return
	
	# Move claw with arrow keys
	if Input.is_action_pressed("move_left") or Input.is_key_pressed(KEY_LEFT):
		_claw_x -= CLAW_SPEED * delta
	if Input.is_action_pressed("move_right") or Input.is_key_pressed(KEY_RIGHT):
		_claw_x += CLAW_SPEED * delta
	
	_claw_x = clamp(_claw_x, CLAW_MIN_X, CLAW_MAX_X)
	_update_claw_position()


func _input(event: InputEvent) -> void:
	if not _game_active:
		# Any key to close after prize shown
		if event is InputEventKey and event.pressed:
			_close_game()
		return
	
	if _is_dropping:
		return
	
	# Space or interact to drop
	if event.is_action_pressed("ui_accept") or event.is_action_pressed("interact"):
		_drop_claw()


func _update_claw_position() -> void:
	if claw_sprite:
		claw_sprite.position.x = _claw_x


func _drop_claw() -> void:
	_is_dropping = true
	instruction_label.text = "Dropping..."
	
	# Animate drop
	var tween := create_tween()
	tween.tween_property(claw_sprite, "position:y", 150.0, DROP_DURATION)
	tween.tween_callback(_grab_prize)
	tween.tween_property(claw_sprite, "position:y", 30.0, GRAB_DURATION)
	tween.tween_callback(_show_prize)


func _grab_prize() -> void:
	# Roll the prize table
	pass  # Prize determined in _show_prize


func _show_prize() -> void:
	_game_active = false
	_is_dropping = false
	
	var prize: Resource = null
	if prize_table and prize_table.has_method("roll_prize"):
		prize = prize_table.roll_prize()
	
	if prize:
		var prize_type: String = prize.get("prize_type") if prize.get("prize_type") else "gems"
		var amount: int = prize.get("amount") if prize.get("amount") else 1
		var prize_id: String = prize.get("prize_id") if prize.get("prize_id") else ""
		
		# Grant the prize
		_grant_prize(prize_type, amount, prize_id)
		
		# Show prize message
		match prize_type:
			"gems":
				prize_label.text = "🎉 You got %d gems!" % amount
			"candy":
				prize_label.text = "🎉 You got a candy bar!"
			"sticker":
				prize_label.text = "🎉 You got a sticker: %s!" % prize_id
			_:
				prize_label.text = "🎉 You won!"
	else:
		prize_label.text = "The claw slipped! Try again?"
	
	prize_label.visible = true
	instruction_label.text = "Press any key to continue..."
	
	game_finished.emit(prize)


func _grant_prize(prize_type: String, amount: int, prize_id: String) -> void:
	if not _game_state:
		return
	
	match prize_type:
		"gems":
			_game_state.gems += amount
			print("[ClawMachine] Granted %d gems. Total: %d" % [amount, _game_state.gems])
		"candy":
			# TODO: Add candy inventory when implemented
			print("[ClawMachine] Granted candy: ", prize_id)
		"sticker":
			if prize_id != "" and prize_id not in _game_state.owned_stickers:
				_game_state.add_sticker(prize_id)
				print("[ClawMachine] Granted sticker: ", prize_id)


func _close_game() -> void:
	queue_free()
