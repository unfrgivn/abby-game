extends CharacterBody3D
## Player controller for overworld movement.
## Simple top-down movement with WASD controls.

@export var move_speed: float = 5.0

func _physics_process(delta: float) -> void:
	var input_dir := Vector2.ZERO
	
	if Input.is_action_pressed("move_up"):
		input_dir.y -= 1
	if Input.is_action_pressed("move_down"):
		input_dir.y += 1
	if Input.is_action_pressed("move_left"):
		input_dir.x -= 1
	if Input.is_action_pressed("move_right"):
		input_dir.x += 1
	
	input_dir = input_dir.normalized()
	
	# Convert 2D input to 3D movement (top-down: X stays X, Y becomes Z)
	var direction := Vector3(input_dir.x, 0, input_dir.y)
	
	if direction.length() > 0:
		velocity.x = direction.x * move_speed
		velocity.z = direction.z * move_speed
	else:
		velocity.x = move_toward(velocity.x, 0, move_speed)
		velocity.z = move_toward(velocity.z, 0, move_speed)
	
	move_and_slide()
