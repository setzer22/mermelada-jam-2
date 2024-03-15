extends Node

func _on_GUI_start_night():
	get_node("/root/GameManager").ReportNewDay()

func _on_GUI_start_day():
	get_node("/root/GameManager").ReportNewDay()
