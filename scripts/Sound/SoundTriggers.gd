extends Node

func _ready():
	get_node("/root/SoundManager").ChangeAmbientSound("Day")
	get_node("/root/GameManager").connect("LevelCompleted", self, "on_level_completed")
	get_node("/root/GameManager").connect("LevelFailed", self, "on_level_failed")
	get_node("/root/GameManager").connect("TenantDamaged", self, "_on_GameManager_TenantDamaged")

func _on_GUI_start_day():
	get_node("/root/SoundManager").ChangeAmbientSound("Day")

func _on_GUI_start_night():
	get_node("/root/SoundManager").ChangeAmbientSound("Night")

func on_level_completed():
	get_node("/root/SoundManager").ChangeAmbientSound("Office")

func on_level_failed():
	get_node("/root/SoundManager").ChangeAmbientSound("Office")

func _on_GameManager_TenantDamaged(amountPct):
	get_node("/root/SoundManager").PlayOneShot("Scream")
