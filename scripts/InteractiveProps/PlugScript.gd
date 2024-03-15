extends Node

func _on_Switchable_StateChanged(isOn, playerWasHolding):
	if playerWasHolding == "fork":
		$"../ForkInPlug".visible = true;
	else:
		$"../AnimationPlayer".play("MessWith")

