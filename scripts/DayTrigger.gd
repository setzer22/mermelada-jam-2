extends Node

func _on_GUI_start_night():
	get_node("/root/GameManager").ReportNewDay()
