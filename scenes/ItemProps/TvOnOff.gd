extends Node

func _on_Switchable_StateChanged(isOn, playerWasHolding):
	if playerWasHolding == "blanket":
		$"../BlanketSprite".visible = true
	else:
		$"../OffSprite".visible = !isOn
