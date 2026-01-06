extends Resource
class_name PrizeTableEntry
## A single entry in a prize table with weight for random selection.

@export var prize_type: String = "gems"  # "gems", "candy", "sticker"
@export var prize_id: String = ""  # For stickers, the sticker ID
@export var amount: int = 1  # For gems/candy, how many
@export var weight: int = 10  # Higher = more common
