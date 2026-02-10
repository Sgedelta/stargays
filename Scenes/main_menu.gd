extends Control

@export var firstLevelPath = "res://Scenes/Levels/TestLevel.tscn"

func _on_button_pressed() -> void:
	get_tree().change_scene_to_file(firstLevelPath)
