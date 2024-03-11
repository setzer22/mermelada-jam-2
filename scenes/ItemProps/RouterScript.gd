extends Node

func _on_Switchable_StateChanged(isOn):
	var off_light = $"../OffLight";
	off_light.visible = !isOn;
