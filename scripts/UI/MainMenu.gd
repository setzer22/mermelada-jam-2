extends Node2D

## Make sure to put game scene on Menu Node!
export var mainGameScene : PackedScene

func _ready():
	$VBoxContainer/VBoxContainer/StartButton.grab_focus()
	get_node("%StartButton").connect("onClick", self, "_on_StartButton_pressed")
	get_node("%OptionsButton").connect("onClick",self, "_on_OptionsButton_pressed")
	get_node("%QuitButton").connect("onClick", self, "_on_QuitButton_pressed")

func _on_StartButton_pressed():
	GameManager.GoToNextLevel()

func _on_QuitButton_pressed():
	get_tree().quit()

func _on_OptionsButton_pressed():
	$OptionsMenu.visible = !$OptionsMenu.visible

func _input(event):
	if event is InputEventKey and event.scancode == KEY_ESCAPE and event.pressed:
		if $OptionsMenu.visible:
			$OptionsMenu.visible = false
