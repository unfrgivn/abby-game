extends Node
## BattleDirector - Placeholder battle scene controller.
## Will be expanded in Issue 5 to wire BattleState and BattleHUD.

var _encounter_id: String = ""


func _ready() -> void:
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		_encounter_id = scene_router.get_current_encounter_id()
	
	print("[BattleDirector] Battle started. Encounter: ", _encounter_id)
	
	# Update debug label
	var label := get_node_or_null("BattleHUD/Panel/DebugLabel")
	if label:
		label.text = "BATTLE SCENE\nEncounter: %s\n\n(Press Escape to exit)" % [_encounter_id if _encounter_id else "debug"]


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("ui_cancel"):
		_exit_battle(true)


func _exit_battle(victory: bool) -> void:
	print("[BattleDirector] Exiting battle. Victory: ", victory)
	var scene_router: Node = get_node_or_null("/root/SceneRouter")
	if scene_router:
		scene_router.end_battle(victory)
	else:
		push_error("[BattleDirector] SceneRouter not found!")
