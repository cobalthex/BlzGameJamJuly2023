@tool
extends Node

@export var new_Global_Seed: int

func set_global_seed(new_value):
	new_Global_Seed = new_value
	update_children_seed()
	
func update_children_seed():
	var children = get_children()
	for child in children:
			var child_script = child.get_script()
			if child_script and child_script.has_method('set_global_seed'):
				child.set_global_seed(new_Global_Seed)

