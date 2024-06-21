extends Node

onready var player := {
	"1" : {
		viewport = $"HBoxContainer/ViewportContainer2/Viewport",
		camera = $"HBoxContainer/ViewportContainer2/Viewport/AIMaster/AIPlayer/Camera2D",
		player = $HBoxContainer/ViewportContainer2/Viewport/AIMaster/AIPlayer,

	},
	"2" : {
		viewport = $"HBoxContainer/ViewportContainer/Viewport",
		camera = $"HBoxContainer/ViewportContainer/Viewport/Master/Player/Camera2D",
		player = $HBoxContainer/ViewportContainer/Viewport/Master/Player,
	}
}


