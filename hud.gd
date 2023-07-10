extends Control

func set_game_time(value): #sets the number for the UI
	$time_label.text = "TIME: " + str(value) #grabs the text setting of the time label and changes it to say TIME: + a string version of value
