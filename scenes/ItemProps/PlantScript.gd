extends Node

func _on_Switchable_StateChanged(isOn, playerWasHolding):
	if playerWasHolding == "beer":
		$"../BeerPlant".visible = true;
	else:
		$"../AnimationPlayer".play("MessWith")
