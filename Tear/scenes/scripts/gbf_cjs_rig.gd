@tool
extends Node2D
## Root of a scene built by GbfCjsBuilder from a baked GBF animation.
##
## The AnimationPlayer child holds one animation per GBF motion (wait, attack, mortal_A...).
## Frame scripts of the original animations are replayed by a method track calling
## _gbf_event(): sounds are played from sound_dir, everything else is forwarded
## through the gbf_event signal (mDamage, mAnimShake, mChangeMotion...).

## Emitted for every frame script call, e.g. ("lib/raid/extension", "mDamage", [3]).
signal gbf_event(module: String, call: String, args: Array)

## Folder holding the "se/..." and "voice/..." files played by the animations.
@export_dir var sound_dir: String = ""
## Play sounds when the animation asks for it.
@export var play_sounds: bool = true

@onready var animation_player: AnimationPlayer = $AnimationPlayer


## Play a motion by its GBF name.
func play(motion: StringName) -> void:
	animation_player.play(motion)


func _gbf_event(module: String, call: String, args: Array) -> void:
	if module == "lib/sound" and call == "play" and args.size() > 0:
		_play_sound(str(args[0]))
	gbf_event.emit(module, call, args)


func _play_sound(file: String) -> void:
	if not play_sounds or sound_dir.is_empty() or Engine.is_editor_hint():
		return
	var stream_path := sound_dir.path_join(file)
	if not ResourceLoader.exists(stream_path):
		return
	var player := AudioStreamPlayer.new()
	player.stream = load(stream_path)
	player.finished.connect(player.queue_free)
	add_child(player)
	player.play()
