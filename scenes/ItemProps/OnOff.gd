extends Node

onready var shade = get_parent().get_node("Shade");

# Changes the sprite of the lamp to toggle on / off
func _on_Switchable_StateChanged(isOn, _a):
	if isOn:
		shade.texture = preload("res://assets/lamp_light.png")
	else:
		shade.texture = preload("res://assets/lamp_dark.png")
