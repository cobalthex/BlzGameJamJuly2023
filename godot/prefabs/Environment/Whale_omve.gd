extends Node3D

func _ready() -> void:
	pass
		

func _physics_process(delta: float) -> void:
	const move_speed := 4.0
	%Path3D/PathFollow3D.progress += move_speed * delta
