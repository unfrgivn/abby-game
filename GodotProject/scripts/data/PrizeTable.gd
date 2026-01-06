extends Resource
class_name PrizeTable
## Table of possible prizes with weighted random selection.

@export var entries: Array[Resource] = []  # Array of PrizeTableEntry

## Roll a random prize from the table
func roll_prize() -> Resource:
	if entries.is_empty():
		return null
	
	var total_weight: int = 0
	for entry in entries:
		var w: int = entry.get("weight") if entry.get("weight") else 1
		total_weight += w
	
	var roll: int = randi() % total_weight
	var cumulative: int = 0
	
	for entry in entries:
		var w: int = entry.get("weight") if entry.get("weight") else 1
		cumulative += w
		if roll < cumulative:
			return entry
	
	return entries[0]
