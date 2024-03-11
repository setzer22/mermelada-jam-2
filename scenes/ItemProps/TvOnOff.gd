extends Node

func _on_Switchable_StateChanged(isOn):
	$"../OffSprite".visible = !isOn
