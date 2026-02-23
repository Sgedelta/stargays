extends CanvasLayer


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	if GameManager.InstanceButForGD.TriggerWarningSeen:
		queue_free()
	else:
		var fadeIn = get_tree().create_tween()
		fadeIn.tween_property(get_node("QuestionLabel"), "modulate", Color(1, 1, 1, 1), GameManager.InstanceButForGD.FadeTime).from(Color(1, 1, 1, 0))
		fadeIn.tween_property(get_node("OptionsHFlow"), "modulate", Color(1, 1, 1, 1), GameManager.InstanceButForGD.FadeTime).from(Color(1, 1, 1, 0)).set_delay(GameManager.InstanceButForGD.FadeTime * .5)
	pass # Replace with function body.


func _on_proceed_pressed() -> void:
	GameManager.InstanceButForGD.TriggerWarningSeen = true
	var fadeOut = get_tree().create_tween()
	fadeOut.tween_property(get_node("CanvasModulate"), "color", Color(1, 1, 1, 0), GameManager.InstanceButForGD.FadeTime)
	fadeOut.tween_callback(queue_free)
	pass # Replace with function body.
