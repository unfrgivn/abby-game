extends Node
class_name BattleDirector
## BattleDirector - Bridges EncounterDef → BattleState → BattleHUD.
## Manages battle flow, player input, enemy AI, and scene transitions.

const PLAYER_MAX_HP := 50
const PLAYER_SPEED := 10

var _encounter_id: String = ""
var _encounter_def: EncounterDef = null
var _battle_state: BattleState = null
var _hud: BattleHUD = null

var _equipped_stickers: Array[StickerDef] = []
var _is_processing_turn := false


func _ready() -> void:
	_hud = $BattleHUD
	
	# Get encounter from SceneRouter
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		_encounter_id = scene_router.get_current_encounter_id()
	
	# Load encounter definition
	var data_registry: Node = get_node_or_null("/root/DataRegistry")
	if data_registry and _encounter_id != "":
		_encounter_def = data_registry.get_encounter(_encounter_id)
	
	# Load player's equipped stickers
	_load_equipped_stickers()
	
	# Initialize battle
	_start_battle()
	
	# Connect HUD signals
	_hud.sticker_selected.connect(_on_sticker_selected)
	_hud.defend_pressed.connect(_on_defend_pressed)
	_hud.run_pressed.connect(_on_run_pressed)


func _load_equipped_stickers() -> void:
	_equipped_stickers.clear()
	var game_state: Node = get_node_or_null("/root/GameState")
	var data_registry: Node = get_node_or_null("/root/DataRegistry")
	
	if not game_state or not data_registry:
		# Fallback: use starter stickers for debug
		for id in ["bonk", "glitter_bandage", "pocket_sand", ""]:
			if id != "":
				var sticker := data_registry.get_sticker(id) if data_registry else null
				_equipped_stickers.append(sticker)
			else:
				_equipped_stickers.append(null)
		return
	
	for sticker_id in game_state.equipped_stickers:
		if sticker_id != "":
			var sticker := data_registry.get_sticker(sticker_id)
			_equipped_stickers.append(sticker)
		else:
			_equipped_stickers.append(null)


func _start_battle() -> void:
	_battle_state = BattleState.new()
	
	# Build enemy list from encounter
	var enemies: Array[Dictionary] = []
	var data_registry: Node = get_node_or_null("/root/DataRegistry")
	
	if _encounter_def and data_registry:
		for enemy_id in _encounter_def.enemy_ids:
			var enemy_def := data_registry.get_enemy(enemy_id)
			if enemy_def:
				enemies.append({
					"id": enemy_id,
					"name": enemy_def.name,
					"max_hp": enemy_def.max_hp,
					"speed": enemy_def.speed,
				})
	
	# Fallback for debug mode
	if enemies.is_empty():
		enemies.append({
			"id": "debug_enemy",
			"name": "Debug Raccoon",
			"max_hp": 25,
			"speed": 6,
		})
	
	# Setup battle
	_battle_state.setup(PLAYER_MAX_HP, PLAYER_SPEED, enemies)
	
	# Connect signals
	_battle_state.damage_dealt.connect(_on_damage_dealt)
	_battle_state.healing_done.connect(_on_healing_done)
	_battle_state.battle_won.connect(_on_battle_won)
	_battle_state.battle_lost.connect(_on_battle_lost)
	
	# Initialize HUD
	_update_hud()
	_hud.clear_log()
	_hud.log_event("Battle started!")
	
	var first := _battle_state.get_current_combatant_id()
	if first == "player":
		_hud.log_event("Your turn!")
		_hud.set_commands_enabled(true)
	else:
		_hud.log_event("Enemy attacks!")
		_hud.set_commands_enabled(false)
		_do_enemy_turn()


func _update_hud() -> void:
	# Update HP bars
	var player := _battle_state.get_player()
	if player:
		_hud.set_player_hp(player.current_hp, player.max_hp)
	
	var enemies := _battle_state.get_enemies()
	if not enemies.is_empty():
		var enemy := enemies[0]
		_hud.set_enemy_hp(enemy.current_hp, enemy.max_hp, enemy.name)
	
	# Update sticker buttons
	for i in range(4):
		var sticker: StickerDef = _equipped_stickers[i] if i < _equipped_stickers.size() else null
		if sticker:
			var cooldown := player.get_cooldown(sticker.id) if player else 0
			_hud.set_sticker_slot(i, sticker.name, cooldown)
		else:
			_hud.set_sticker_slot(i, "", 0)


func _on_sticker_selected(slot: int) -> void:
	if _is_processing_turn or _battle_state.is_battle_over():
		return
	if not _battle_state.is_player_turn():
		return
	
	var sticker: StickerDef = _equipped_stickers[slot] if slot < _equipped_stickers.size() else null
	if not sticker:
		return
	
	_is_processing_turn = true
	_hud.set_commands_enabled(false)
	
	# Find target enemy
	var enemies := _battle_state.get_enemies()
	var target_id := enemies[0].id if not enemies.is_empty() else ""
	
	# Use sticker
	var result := _battle_state.use_sticker(
		"player",
		sticker.id,
		sticker.power,
		sticker.type,
		sticker.targeting,
		sticker.cooldown_turns,
		target_id
	)
	
	if result["success"]:
		_hud.log_event("Fae used %s!" % sticker.name)
	else:
		_hud.log_event("Can't use %s: %s" % [sticker.name, result.get("error", "unknown")])
		_is_processing_turn = false
		_hud.set_commands_enabled(true)
		return
	
	_update_hud()
	
	# End turn
	_battle_state.end_turn()
	_update_hud()
	
	if not _battle_state.is_battle_over():
		# Enemy turn
		await get_tree().create_timer(0.5).timeout
		_do_enemy_turn()
	
	_is_processing_turn = false


func _on_defend_pressed() -> void:
	if _is_processing_turn or _battle_state.is_battle_over():
		return
	if not _battle_state.is_player_turn():
		return
	
	_is_processing_turn = true
	_hud.set_commands_enabled(false)
	
	_battle_state.defend("player")
	_hud.log_event("Fae defends!")
	
	_battle_state.end_turn()
	_update_hud()
	
	if not _battle_state.is_battle_over():
		await get_tree().create_timer(0.5).timeout
		_do_enemy_turn()
	
	_is_processing_turn = false


func _on_run_pressed() -> void:
	if _is_processing_turn or _battle_state.is_battle_over():
		return
	
	_hud.log_event("Fae ran away!")
	await get_tree().create_timer(0.5).timeout
	_exit_battle(false)


func _do_enemy_turn() -> void:
	if _battle_state.is_battle_over():
		return
	
	var current_id := _battle_state.get_current_combatant_id()
	var combatant := _battle_state.get_combatant(current_id)
	
	if combatant and not combatant.is_player:
		# Simple AI: attack player
		var data_registry: Node = get_node_or_null("/root/DataRegistry")
		var enemy_def: EnemyDef = null
		if data_registry:
			enemy_def = data_registry.get_enemy(current_id)
		
		var attack_power := enemy_def.attack_power if enemy_def else 8
		
		_battle_state.use_sticker(
			current_id,
			"enemy_attack",
			attack_power,
			"Attack",
			"SingleEnemy",
			0,
			"player"
		)
		
		_hud.log_event("%s attacks!" % combatant.name)
		_update_hud()
		
		_battle_state.end_turn()
		_update_hud()
	
	# Check if back to player turn
	if not _battle_state.is_battle_over() and _battle_state.is_player_turn():
		_hud.log_event("Your turn!")
		_hud.set_commands_enabled(true)


func _on_damage_dealt(target_id: String, amount: int, _source_id: String) -> void:
	var combatant := _battle_state.get_combatant(target_id)
	var name := combatant.name if combatant else target_id
	_hud.log_event("%s took %d damage!" % [name, amount])


func _on_healing_done(target_id: String, amount: int, _source_id: String) -> void:
	var combatant := _battle_state.get_combatant(target_id)
	var name := combatant.name if combatant else target_id
	_hud.log_event("%s healed %d HP!" % [name, amount])


func _on_battle_won() -> void:
	_hud.show_victory()
	_grant_rewards()
	await get_tree().create_timer(2.0).timeout
	_exit_battle(true)


func _on_battle_lost() -> void:
	_hud.show_defeat()
	await get_tree().create_timer(2.0).timeout
	_exit_battle(false)


func _grant_rewards() -> void:
	if not _encounter_def:
		return
	
	var game_state: Node = get_node_or_null("/root/GameState")
	if not game_state:
		return
	
	# Grant gems
	if _encounter_def.gems_reward > 0:
		game_state.add_gems(_encounter_def.gems_reward)
		_hud.log_event("Got %d gems!" % _encounter_def.gems_reward)
	
	# Grant first-win sticker
	var reward_id := _encounter_def.first_win_sticker_reward_id
	if reward_id != "" and not game_state.is_first_win_claimed(_encounter_id):
		if game_state.add_sticker(reward_id):
			var data_registry: Node = get_node_or_null("/root/DataRegistry")
			var sticker := data_registry.get_sticker(reward_id) if data_registry else null
			var sticker_name := sticker.name if sticker else reward_id
			_hud.log_event("Got new sticker: %s!" % sticker_name)
		game_state.claim_first_win(_encounter_id)


func _exit_battle(victory: bool) -> void:
	print("[BattleDirector] Exiting battle. Victory: ", victory)
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		scene_router.end_battle(victory)
	else:
		push_error("[BattleDirector] SceneRouter not found!")
