extends Resource
class_name NoteDef
## Definition for a hidden note revealed by the Blacklight Lantern.

## Stable ID for persistence
@export var id: String = ""

## Title shown in the journal
@export var title: String = ""

## Full text content of the note
@export var body: String = ""

## Optional icon/doodle texture path
@export var icon_path: String = ""
