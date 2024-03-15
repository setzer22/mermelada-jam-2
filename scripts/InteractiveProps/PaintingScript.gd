extends Node

var messed_with = false;

func _on_Switchable_StateChanged(isOn, playerWasHolding):
	if not messed_with:
		get_parent().rotation_degrees = 25;
		messed_with = true;
