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


func _input(event):
	if playerOverEnemy && event is InputEventKey and event.scancode == KEY_ENTER and event.pressed:
		if get_node("FearBox").visible:
			get_node("FearBox").visible = false
			print("Lo oculto")
		else:
			get_node("FearBox").visible = true
			print("Lo pongo")
