extends RefCounted
class_name BattleState
## Pure battle rules engine - no Node dependencies for testability.
## Manages HP, turn order, cooldowns, and win/lose conditions.

signal turn_started(combatant_id: String)
signal damage_dealt(target_id: String, amount: int, source_id: String)
signal healing_done(target_id: String, amount: int, source_id: String)
signal sticker_used(user_id: String, sticker_id: String)
signal cooldown_started(combatant_id: String, sticker_id: String, turns: int)
signal battle_won()
signal battle_lost()

## Combatant data structure
class Combatant:
	var id: String
	var name: String
	var max_hp: int
	var current_hp: int
	var speed: int
	var is_player: bool
	var sticker_cooldowns: Dictionary = {}  # sticker_id -> turns remaining
	
	func _init(p_id: String, p_name: String, p_max_hp: int, p_speed: int, p_is_player: bool) -> void:
		id = p_id
		name = p_name
		max_hp = p_max_hp
		current_hp = p_max_hp
		speed = p_speed
		is_player = p_is_player
	
	func is_alive() -> bool:
		return current_hp > 0
	
	func take_damage(amount: int) -> int:
		var actual := mini(amount, current_hp)
		current_hp -= actual
		return actual
	
	func heal(amount: int) -> int:
		var actual := mini(amount, max_hp - current_hp)
		current_hp += actual
		return actual
	
	func start_cooldown(sticker_id: String, turns: int) -> void:
		if turns > 0:
			sticker_cooldowns[sticker_id] = turns
	
	func is_on_cooldown(sticker_id: String) -> bool:
		return sticker_cooldowns.get(sticker_id, 0) > 0
	
	func get_cooldown(sticker_id: String) -> int:
		return sticker_cooldowns.get(sticker_id, 0)
	
	func tick_cooldowns() -> void:
		var to_remove: Array[String] = []
		for sticker_id in sticker_cooldowns:
			sticker_cooldowns[sticker_id] -= 1
			if sticker_cooldowns[sticker_id] <= 0:
				to_remove.append(sticker_id)
		for sticker_id in to_remove:
			sticker_cooldowns.erase(sticker_id)


## All combatants in battle
var _combatants: Dictionary = {}  # id -> Combatant

## Turn order (list of combatant IDs sorted by speed)
var _turn_order: Array[String] = []

## Current turn index
var _current_turn_index: int = 0

## Battle state
var _is_battle_over: bool = false
var _player_won: bool = false


## Initialize battle with player and enemies
func setup(player_hp: int, player_speed: int, enemies: Array[Dictionary]) -> void:
	_combatants.clear()
	_turn_order.clear()
	_current_turn_index = 0
	_is_battle_over = false
	_player_won = false
	
	# Add player
	var player := Combatant.new("player", "Fae", player_hp, player_speed, true)
	_combatants["player"] = player
	
	# Add enemies
	for i in range(enemies.size()):
		var e: Dictionary = enemies[i]
		var enemy_id: String = e.get("id", "enemy_%d" % i)
		var enemy := Combatant.new(
			enemy_id,
			e.get("name", "Enemy"),
			e.get("max_hp", 20),
			e.get("speed", 5),
			false
		)
		_combatants[enemy_id] = enemy
	
	# Calculate turn order (higher speed goes first)
	_calculate_turn_order()


## Calculate turn order based on speed
func _calculate_turn_order() -> void:
	var alive: Array[Combatant] = []
	for id in _combatants:
		var c: Combatant = _combatants[id]
		if c.is_alive():
			alive.append(c)
	
	# Sort by speed descending
	alive.sort_custom(func(a: Combatant, b: Combatant) -> bool:
		if a.speed != b.speed:
			return a.speed > b.speed
		# Tie-breaker: player goes first
		return a.is_player
	)
	
	_turn_order.clear()
	for c in alive:
		_turn_order.append(c.id)


## Get the current combatant's ID
func get_current_combatant_id() -> String:
	if _turn_order.is_empty():
		return ""
	return _turn_order[_current_turn_index % _turn_order.size()]


## Get combatant by ID
func get_combatant(id: String) -> Combatant:
	return _combatants.get(id, null)


## Get player combatant
func get_player() -> Combatant:
	return _combatants.get("player", null)


## Get all enemy combatants
func get_enemies() -> Array[Combatant]:
	var enemies: Array[Combatant] = []
	for id in _combatants:
		var c: Combatant = _combatants[id]
		if not c.is_player and c.is_alive():
			enemies.append(c)
	return enemies


## Check if it's the player's turn
func is_player_turn() -> bool:
	return get_current_combatant_id() == "player"


## Check if battle is over
func is_battle_over() -> bool:
	return _is_battle_over


## Check if player won
func did_player_win() -> bool:
	return _player_won


## Apply a sticker attack from one combatant to target(s)
func use_sticker(user_id: String, sticker_id: String, sticker_power: int, sticker_type: String, 
				 sticker_targeting: String, sticker_cooldown: int, target_id: String = "") -> Dictionary:
	var user: Combatant = _combatants.get(user_id, null)
	if not user or not user.is_alive():
		return {"success": false, "error": "Invalid user"}
	
	if user.is_on_cooldown(sticker_id):
		return {"success": false, "error": "Sticker on cooldown"}
	
	sticker_used.emit(user_id, sticker_id)
	
	var result := {
		"success": true,
		"effects": []
	}
	
	# Apply effect based on type
	match sticker_type:
		"Attack":
			result["effects"] = _apply_attack(user_id, sticker_power, sticker_targeting, target_id)
		"Support":
			result["effects"] = _apply_support(user_id, sticker_power, sticker_targeting, target_id)
		"Utility":
			# For PoC, utility just does reduced damage
			result["effects"] = _apply_attack(user_id, sticker_power / 2, sticker_targeting, target_id)
	
	# Start cooldown
	if sticker_cooldown > 0:
		user.start_cooldown(sticker_id, sticker_cooldown)
		cooldown_started.emit(user_id, sticker_id, sticker_cooldown)
	
	return result


## Apply attack damage
func _apply_attack(user_id: String, power: int, targeting: String, target_id: String) -> Array:
	var effects: Array = []
	var targets := _get_targets(user_id, targeting, target_id, false)
	
	for t in targets:
		var damage := t.take_damage(power)
		damage_dealt.emit(t.id, damage, user_id)
		effects.append({"type": "damage", "target": t.id, "amount": damage})
	
	return effects


## Apply support healing
func _apply_support(user_id: String, power: int, targeting: String, target_id: String) -> Array:
	var effects: Array = []
	var targets := _get_targets(user_id, targeting, target_id, true)
	
	for t in targets:
		var healed := t.heal(power)
		healing_done.emit(t.id, healed, user_id)
		effects.append({"type": "heal", "target": t.id, "amount": healed})
	
	return effects


## Get targets based on targeting type
func _get_targets(user_id: String, targeting: String, target_id: String, friendly: bool) -> Array[Combatant]:
	var targets: Array[Combatant] = []
	var user: Combatant = _combatants.get(user_id, null)
	
	match targeting:
		"SingleEnemy":
			if target_id != "":
				var t: Combatant = _combatants.get(target_id, null)
				if t and t.is_alive() and t.is_player != user.is_player:
					targets.append(t)
			else:
				# Auto-target first alive enemy
				for id in _combatants:
					var c: Combatant = _combatants[id]
					if c.is_alive() and c.is_player != user.is_player:
						targets.append(c)
						break
		"AllEnemies":
			for id in _combatants:
				var c: Combatant = _combatants[id]
				if c.is_alive() and c.is_player != user.is_player:
					targets.append(c)
		"Self":
			if user:
				targets.append(user)
		"Ally":
			# For PoC, just target self (no party members yet)
			if user:
				targets.append(user)
	
	return targets


## Defend action (placeholder - reduces damage next turn)
func defend(user_id: String) -> Dictionary:
	# For PoC, defend is a no-op that ends turn
	return {"success": true, "action": "defend"}


## End current turn and advance to next
func end_turn() -> void:
	var current_id := get_current_combatant_id()
	var current: Combatant = _combatants.get(current_id, null)
	
	if current:
		current.tick_cooldowns()
	
	# Check win/lose conditions
	_check_battle_end()
	
	if _is_battle_over:
		return
	
	# Advance turn
	_current_turn_index += 1
	
	# Recalculate turn order if we've gone through everyone
	if _current_turn_index >= _turn_order.size():
		_current_turn_index = 0
		_calculate_turn_order()
	
	# Skip dead combatants
	while not _turn_order.is_empty():
		var next_id := get_current_combatant_id()
		var next: Combatant = _combatants.get(next_id, null)
		if next and next.is_alive():
			break
		_current_turn_index += 1
		if _current_turn_index >= _turn_order.size():
			_current_turn_index = 0
			_calculate_turn_order()
	
	turn_started.emit(get_current_combatant_id())


## Check for battle end conditions
func _check_battle_end() -> void:
	var player: Combatant = get_player()
	if not player or not player.is_alive():
		_is_battle_over = true
		_player_won = false
		battle_lost.emit()
		return
	
	var any_enemy_alive := false
	for id in _combatants:
		var c: Combatant = _combatants[id]
		if not c.is_player and c.is_alive():
			any_enemy_alive = true
			break
	
	if not any_enemy_alive:
		_is_battle_over = true
		_player_won = true
		battle_won.emit()
