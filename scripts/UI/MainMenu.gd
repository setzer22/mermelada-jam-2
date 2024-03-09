extends Node2D

## Make sure to put game scene on Menu Node!
export var mainGameScene : PackedScene

func _ready():
	$VBoxContainer/VBoxContainer/StartButton.grab_focus()

func _on_StartButton_pressed():
	get_tree().change_scene(mainGameScene.resource_path)

func _on_QuitButton_pressed():
	get_tree().quit()

