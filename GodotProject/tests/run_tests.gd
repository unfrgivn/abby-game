extends SceneTree
## Headless test runner for BattleState.
## Run with: godot --headless --script res://tests/run_tests.gd

var _tests_passed := 0
var _tests_failed := 0
var _current_test := ""


func _init() -> void:
	print("\n=== BattleState Tests ===\n")
	
	# Run all tests
	test_setup_creates_combatants()
	test_turn_order_by_speed()
	test_player_attack_damages_enemy()
	test_enemy_defeated_at_zero_hp()
	test_player_wins_when_all_enemies_dead()
	test_player_loses_when_dead()
	test_sticker_cooldown_prevents_reuse()
	test_cooldown_decrements_each_turn()
	test_healing_restores_hp()
	test_healing_capped_at_max_hp()
	
	# Print results
	print("\n=== Results ===")
	print("Passed: ", _tests_passed)
	print("Failed: ", _tests_failed)
	print("")
	
	# Exit with appropriate code
	if _tests_failed > 0:
		quit(1)
	else:
		quit(0)


func _start_test(name: String) -> void:
	_current_test = name
	print("  [TEST] ", name)


func _pass() -> void:
	_tests_passed += 1
	print("    ✓ PASS")


func _fail(reason: String) -> void:
	_tests_failed += 1
	print("    ✗ FAIL: ", reason)


func _assert_eq(actual: Variant, expected: Variant, msg: String = "") -> bool:
	if actual != expected:
		_fail("%s - Expected %s, got %s" % [msg, expected, actual])
		return false
	return true


func _assert_true(value: bool, msg: String = "") -> bool:
	if not value:
		_fail("%s - Expected true" % msg)
		return false
	return true


func _assert_false(value: bool, msg: String = "") -> bool:
	if value:
		_fail("%s - Expected false" % msg)
		return false
	return true


# ============ TESTS ============

func test_setup_creates_combatants() -> void:
	_start_test("setup creates combatants")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 20, "speed": 5}])
	
	var player := state.get_player()
	var enemies := state.get_enemies()
	
	if not _assert_true(player != null, "Player exists"): return
	if not _assert_eq(player.max_hp, 50, "Player HP"): return
	if not _assert_eq(enemies.size(), 1, "Enemy count"): return
	if not _assert_eq(enemies[0].name, "Raccoon", "Enemy name"): return
	
	_pass()


func test_turn_order_by_speed() -> void:
	_start_test("turn order sorted by speed (higher first)")
	
	var state := BattleState.new()
	# Player speed 5, enemy speed 10 - enemy should go first
	state.setup(50, 5, [{"id": "fast_enemy", "name": "Fast", "max_hp": 20, "speed": 10}])
	
	var first := state.get_current_combatant_id()
	if not _assert_eq(first, "fast_enemy", "Faster enemy goes first"): return
	
	# Now with player faster
	var state2 := BattleState.new()
	state2.setup(50, 15, [{"id": "slow_enemy", "name": "Slow", "max_hp": 20, "speed": 5}])
	
	var first2 := state2.get_current_combatant_id()
	if not _assert_eq(first2, "player", "Faster player goes first"): return
	
	_pass()


func test_player_attack_damages_enemy() -> void:
	_start_test("player attack damages enemy")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 20, "speed": 5}])
	
	var enemy := state.get_combatant("enemy1")
	var initial_hp := enemy.current_hp
	
	var result := state.use_sticker("player", "bonk", 15, "Attack", "SingleEnemy", 0, "enemy1")
	
	if not _assert_true(result["success"], "Attack succeeds"): return
	if not _assert_eq(enemy.current_hp, initial_hp - 15, "Enemy took 15 damage"): return
	
	_pass()


func test_enemy_defeated_at_zero_hp() -> void:
	_start_test("enemy defeated at zero HP")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 20, "speed": 5}])
	
	var enemy := state.get_combatant("enemy1")
	
	# Deal lethal damage
	state.use_sticker("player", "bonk", 25, "Attack", "SingleEnemy", 0, "enemy1")
	
	if not _assert_eq(enemy.current_hp, 0, "Enemy at 0 HP"): return
	if not _assert_false(enemy.is_alive(), "Enemy not alive"): return
	
	_pass()


func test_player_wins_when_all_enemies_dead() -> void:
	_start_test("player wins when all enemies dead")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 20, "speed": 5}])
	
	# Kill the enemy
	state.use_sticker("player", "bonk", 25, "Attack", "SingleEnemy", 0, "enemy1")
	state.end_turn()
	
	if not _assert_true(state.is_battle_over(), "Battle is over"): return
	if not _assert_true(state.did_player_win(), "Player won"): return
	
	_pass()


func test_player_loses_when_dead() -> void:
	_start_test("player loses when dead")
	
	var state := BattleState.new()
	state.setup(20, 5, [{"id": "enemy1", "name": "Raccoon", "max_hp": 50, "speed": 10}])
	
	# Enemy attacks player (simulated)
	state.use_sticker("enemy1", "scratch", 25, "Attack", "SingleEnemy", 0, "player")
	state.end_turn()
	
	if not _assert_true(state.is_battle_over(), "Battle is over"): return
	if not _assert_false(state.did_player_win(), "Player lost"): return
	
	_pass()


func test_sticker_cooldown_prevents_reuse() -> void:
	_start_test("sticker cooldown prevents reuse")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 50, "speed": 5}])
	
	# Use sticker with 2-turn cooldown
	var result1 := state.use_sticker("player", "power_move", 20, "Attack", "SingleEnemy", 2, "enemy1")
	if not _assert_true(result1["success"], "First use succeeds"): return
	
	# Try to use again immediately
	var result2 := state.use_sticker("player", "power_move", 20, "Attack", "SingleEnemy", 2, "enemy1")
	if not _assert_false(result2["success"], "Second use blocked"): return
	if not _assert_eq(result2["error"], "Sticker on cooldown", "Correct error"): return
	
	_pass()


func test_cooldown_decrements_each_turn() -> void:
	_start_test("cooldown decrements each turn")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 100, "speed": 5}])
	
	var player := state.get_player()
	
	# Use sticker with 2-turn cooldown
	state.use_sticker("player", "power_move", 10, "Attack", "SingleEnemy", 2, "enemy1")
	if not _assert_eq(player.get_cooldown("power_move"), 2, "Initial cooldown is 2"): return
	
	# End turn (player's turn ends, cooldown ticks)
	state.end_turn()
	if not _assert_eq(player.get_cooldown("power_move"), 1, "Cooldown is 1 after one turn"): return
	
	# Simulate enemy turn
	state.end_turn()
	if not _assert_eq(player.get_cooldown("power_move"), 0, "Cooldown is 0 after player's next turn"): return
	
	# Should be usable again
	var result := state.use_sticker("player", "power_move", 10, "Attack", "SingleEnemy", 2, "enemy1")
	if not _assert_true(result["success"], "Can use after cooldown expires"): return
	
	_pass()


func test_healing_restores_hp() -> void:
	_start_test("healing restores HP")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 20, "speed": 5}])
	
	var player := state.get_player()
	
	# Take damage first
	player.take_damage(20)
	if not _assert_eq(player.current_hp, 30, "Player at 30 HP after damage"): return
	
	# Heal
	state.use_sticker("player", "heal", 10, "Support", "Self", 0)
	if not _assert_eq(player.current_hp, 40, "Player at 40 HP after heal"): return
	
	_pass()


func test_healing_capped_at_max_hp() -> void:
	_start_test("healing capped at max HP")
	
	var state := BattleState.new()
	state.setup(50, 10, [{"id": "enemy1", "name": "Raccoon", "max_hp": 20, "speed": 5}])
	
	var player := state.get_player()
	
	# Take small damage
	player.take_damage(5)
	if not _assert_eq(player.current_hp, 45, "Player at 45 HP"): return
	
	# Try to heal more than missing
	state.use_sticker("player", "big_heal", 20, "Support", "Self", 0)
	if not _assert_eq(player.current_hp, 50, "Player capped at max 50 HP"): return
	
	_pass()
