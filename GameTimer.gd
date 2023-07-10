extends Control

@export var starting_time = 99 #export is used like a global var maybe?
var timer_node = null
var time_left

@onready var timer_hud = $UI/HUD # allows access to the HUD scene from the Timer_UI layer
# Called when the node enters the scene tree for the first time.
func _ready():
	timer_node = Timer.new() #creates an object from the Timer class
	timer_node.name = "Timer" # sets the name of the Timer node
	timer_node.wait_time = 1 # amount of seconds it waits per timeout
	timer_node.timeout.connect(level_timeout) # runs the function level timeout every second it times out
	
	add_child(timer_node) #creating a child node showing up under remote tab

	time_left = starting_time
	timer_hud.set_game_time(time_left)
	
	timer_node.start() # timer begins here

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func level_timeout(): #happens every timeout
	time_left -= 1 #subtracts 1 from time_left
	timer_hud.set_game_time(time_left) #
	#print(time_left)
	#if time_left < 0 can reset the player
