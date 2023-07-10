extends Control

func set_game_time(value): #sets the number for the timer label
	$time_label.text = "TIME: " + str(value) #grabs the text setting of the time label and changes it to say TIME: + a string version of value

func set_score_label(value): #sets the number for the score label
	$score_label.text = "SCORE: " + str(value)  #grabs the text setting of the score label 
