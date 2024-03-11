extends KinematicBody2D

var playerOverEnemy = false

func _ready():
	pass

func _on_Area2D_body_entered(body:Node):
	if body.get_name() == "Player":
		playerOverEnemy=true

func _on_Area2D_body_exited(body:Node):
	if body.get_name() == "Player":
		playerOverEnemy=false

