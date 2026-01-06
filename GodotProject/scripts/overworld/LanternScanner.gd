extends Node3D
## LanternScanner - Attached to Player, detects hidden notes/doors when lantern is active.
## Uses an Area3D to detect HiddenNote and HiddenDoor nodes in range.

signal lantern_toggled(is_active: bool)
signal note_revealed(note_id: String)
signal door_revealed(door_id: String)

@export var scan_radius: float = 3.0

var is_active: bool = false
var _area: Area3D
var _collision_shape: CollisionShape3D


func _ready() -> void:
	_setup_detection_area()


func _setup_detection_area() -> void:
	_area = Area3D.new()
	_area.name = "LanternArea"
	_area.collision_layer = 0
	_area.collision_mask = 16  # Layer 5 for hidden objects
	add_child(_area)
	
	_collision_shape = CollisionShape3D.new()
	var sphere := SphereShape3D.new()
	sphere.radius = scan_radius
	_collision_shape.shape = sphere
	_area.add_child(_collision_shape)
	
	_area.body_entered.connect(_on_body_entered)


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("lantern"):
		toggle_lantern()


func toggle_lantern() -> void:
	is_active = not is_active
	lantern_toggled.emit(is_active)
	print("[LanternScanner] Lantern ", "ON" if is_active else "OFF")
	
	if is_active:
		_scan_for_hidden_objects()


func _scan_for_hidden_objects() -> void:
	var bodies := _area.get_overlapping_bodies()
	for body in bodies:
		_try_reveal(body)
	
	var areas := _area.get_overlapping_areas()
	for area in areas:
		if area.get_parent():
			_try_reveal(area.get_parent())


func _on_body_entered(body: Node3D) -> void:
	if is_active:
		_try_reveal(body)


func _try_reveal(node: Node) -> void:
	if node.has_method("reveal"):
		node.call("reveal")
