extends Control

@export var mainGamePackedScene = preload("res://Scenes/main_game.tscn")
@export var gameOverPackedScene = preload("res://Scenes/game_over.tscn")
@export var firstLevelName = "firstLevel"


func _ready():
	(get_node("%Background") as AnimatedSprite2D).play()
	if get_node_or_null("/root/MainGame") != null:
		#this is the end and its not loading right for some reason so BRUTE FORCE IT WEEEEEE
		get_tree().change_scene_to_packed(gameOverPackedScene)

func _on_game_start() -> void:
	get_tree().change_scene_to_packed(mainGamePackedScene)
	GameManager.InstanceButForGD.FirstLevelName = firstLevelName
	GameManager.InstanceButForGD.StartNewGame();
