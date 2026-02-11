extends Control

@export var mainGamePath = "res://Scenes/main_game.tscn"
@export var firstLevelName = "firstLevel"


func _ready():
	(get_node("%Background") as AnimatedSprite2D).play()

func _on_button_pressed() -> void:
	get_tree().change_scene_to_file(mainGamePath)
