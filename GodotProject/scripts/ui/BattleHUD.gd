extends Control
class_name BattleHUD
## Battle UI showing HP bars, sticker buttons, and event log.

signal sticker_selected(slot: int)
signal defend_pressed()
signal run_pressed()

@onready var player_hp_bar: ProgressBar = %PlayerHPBar
@onready var player_hp_label: Label = %PlayerHPLabel
@onready var enemy_hp_bar: ProgressBar = %EnemyHPBar
@onready var enemy_hp_label: Label = %EnemyHPLabel
@onready var enemy_name_label: Label = %EnemyNameLabel

@onready var sticker_buttons: Array[Button] = [
	%Sticker1,
	%Sticker2,
	%Sticker3,
	%Sticker4,
]

@onready var defend_button: Button = %DefendButton
@onready var run_button: Button = %RunButton
@onready var event_log: RichTextLabel = %EventLog

var _sticker_cooldowns: Array[int] = [0, 0, 0, 0]


func _ready() -> void:
	add_to_group("battle_hud")
	
	# Connect sticker buttons
	for i in range(4):
		var btn := sticker_buttons[i]
		btn.pressed.connect(_on_sticker_pressed.bind(i))
	
	defend_button.pressed.connect(func(): defend_pressed.emit())
	run_button.pressed.connect(func(): run_pressed.emit())


## Update player HP display
func set_player_hp(current: int, max_hp: int) -> void:
	player_hp_bar.max_value = max_hp
	player_hp_bar.value = current
	player_hp_label.text = "%d / %d" % [current, max_hp]


## Update enemy HP display
func set_enemy_hp(current: int, max_hp: int, enemy_name: String = "Enemy") -> void:
	enemy_hp_bar.max_value = max_hp
	enemy_hp_bar.value = current
	enemy_hp_label.text = "%d / %d" % [current, max_hp]
	enemy_name_label.text = enemy_name


## Set up sticker buttons with names and cooldowns
func set_sticker_slot(slot: int, sticker_name: String, cooldown: int) -> void:
	if slot < 0 or slot >= 4:
		return
	
	var btn := sticker_buttons[slot]
	_sticker_cooldowns[slot] = cooldown
	
	if sticker_name == "":
		btn.text = "(Empty)"
		btn.disabled = true
	elif cooldown > 0:
		btn.text = "%s\n[CD: %d]" % [sticker_name, cooldown]
		btn.disabled = true
	else:
		btn.text = sticker_name
		btn.disabled = false


## Enable or disable all command buttons
func set_commands_enabled(enabled: bool) -> void:
	for btn in sticker_buttons:
		# Only enable if not on cooldown and not empty
		if enabled and not btn.text.begins_with("(Empty)") and not "[CD:" in btn.text:
			btn.disabled = false
		else:
			btn.disabled = not enabled or btn.text.begins_with("(Empty)") or "[CD:" in btn.text
	
	defend_button.disabled = not enabled
	run_button.disabled = not enabled


## Add message to event log
func log_event(message: String) -> void:
	event_log.append_text(message + "\n")
	# Auto-scroll to bottom
	await get_tree().process_frame
	event_log.scroll_to_line(event_log.get_line_count())


## Clear event log
func clear_log() -> void:
	event_log.clear()


## Show victory message
func show_victory() -> void:
	set_commands_enabled(false)
	log_event("\n[color=green]✨ VICTORY! ✨[/color]")


## Show defeat message
func show_defeat() -> void:
	set_commands_enabled(false)
	log_event("\n[color=red]😴 You got tired...[/color]")


func _on_sticker_pressed(slot: int) -> void:
	sticker_selected.emit(slot)
