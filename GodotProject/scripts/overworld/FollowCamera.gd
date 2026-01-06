extends Camera3D
## Follow camera with fixed 60° tilt for top-down perspective.
## Smoothly follows the target with configurable lag.

@export var target_path: NodePath
@export var follow_speed: float = 5.0
@export var offset: Vector3 = Vector3(0, 12, 10)

var _target: Node3D

func _ready() -> void:
	if target_path:
		_target = get_node_or_null(target_path)
	
	if not _target:
		push_warning("[FollowCamera] No target assigned.")

func _physics_process(delta: float) -> void:
	if not _target:
		return
	
	var target_pos := _target.global_position + offset
	global_position = global_position.lerp(target_pos, follow_speed * delta)
	
	# Always look at target (with slight offset to keep player centered)
	look_at(_target.global_position, Vector3.UP)
