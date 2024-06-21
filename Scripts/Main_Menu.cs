using Godot;
using System;


public class Main_Menu : Control {

	private AudioStream StartGame;  // Audio stream for starting the game.
	private AudioStream StartTrain;  // Audio stream for starting the training scene.
	private AudioStream Quit;  // Audio stream for quitting the game.
	private AudioStreamPlayer audioStreamPlayer;  // Node responsible for playing audio streams.
	private VideoPlayer _videoPlayer;  // Node responsible for playing videos.

	public override void _Ready() {
		// Load audio streams
		StartGame = GD.Load<AudioStream>("res://Assets/SoundFX/LevelStart.wav");
		Quit = GD.Load<AudioStream>("res://Assets/SoundFX/Quit.wav");
		StartTrain = GD.Load<AudioStream>("res://Assets/SoundFX/LevelStart.wav");

		// Get reference to AudioStreamPlayer node
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

		// Get reference to VideoPlayer node and connect finished signal
		_videoPlayer = GetNode<VideoPlayer>("VideoPlayer");
		_videoPlayer.Connect("finished", this, nameof(OnVideoFinished));
	}

	// Method triggered when the "GameButton" is pressed
	private async void _on_GameButton_pressed()
	{
		/*
		if (StartGame != null) {
			audioStreamPlayer.Stream = StartGame;
			audioStreamPlayer.Play();
			while (audioStreamPlayer.Playing)
			{
				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
			}
		}
		*/

		GetTree().ChangeScene("res://Scenes/Jeu.tscn");  // Change scene to the game scene
	}

	// Method triggered when the "TrainButton" is pressed
	private async void _on_TrainButton_pressed()
	{
		/*
		if (StartTrain != null) {
			audioStreamPlayer.Stream = StartTrain;
			audioStreamPlayer.Play();
			while (audioStreamPlayer.Playing)
			{
				await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
			}
		}
		*/

		GetTree().ChangeScene("res://Scenes/TrainingScene.tscn");  // Change scene to the training scene
	}

	// Method triggered when the "QuitButton" is pressed
	private async void _on_QuitButton_pressed() {
		/*
		audioStreamPlayer.Stream = Quit;
		audioStreamPlayer.Play();

		while (audioStreamPlayer.Playing)
			await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
		*/

		GetTree().Quit();  // Quit the game
	}

	// Method called when the Main_Menu node is resized
	private void _on_Main_Menu_resized()
	{
		if (_videoPlayer != null)
		{
			// Resize the VideoPlayer to match the size of the viewport
			_videoPlayer.RectMinSize = GetViewport().Size;
			_videoPlayer.RectSize = GetViewport().Size;
		}
	}
	
	// Method called when the VideoPlayer finishes playing the video
	private void OnVideoFinished()
	{
		if (_videoPlayer != null)
			_videoPlayer.Play();  // Restart playing the video
	}
	
}

