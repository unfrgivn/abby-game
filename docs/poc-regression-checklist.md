# PoC Manual Regression Checklist

Run this checklist before shipping any PoC milestone.

## Pre-requisites

- [ ] Unity project opens without errors
- [ ] All scenes in build settings: Bootstrap, Cloverhollow, SchoolInterior, ArcadeInterior
- [ ] Bootstrap scene set as index 0

## Core Loop

### Spawn and Movement
- [ ] Game starts at home bed anchor in Cloverhollow
- [ ] Player can move in all directions smoothly
- [ ] Camera follows player with appropriate offset
- [ ] Player stops cleanly when input released

### Interaction System
- [ ] Interaction prompt appears when near interactables
- [ ] Prompt disappears when walking away
- [ ] Correct interactable selected when multiple nearby
- [ ] Interact button triggers the interaction

### Blacklight Lantern
- [ ] Lantern toggle on/off works
- [ ] Lantern visual feedback visible when active
- [ ] Hidden notes reveal after scanning
- [ ] Note popup displays title and body text
- [ ] Notes added to journal after discovery
- [ ] Hidden doors reveal after scanning
- [ ] Revealed doors become interactable
- [ ] Revealed state persists after save/load

### Journal
- [ ] Journal panel opens and closes
- [ ] Discovered notes listed in journal
- [ ] New notes highlighted until viewed
- [ ] Note details viewable from list

### Energy and Candy
- [ ] Energy bar displays current/max
- [ ] Taking damage reduces energy
- [ ] Candy bar count displays correctly
- [ ] Consuming candy restores energy
- [ ] Cannot consume candy when at full energy
- [ ] Cannot consume candy when count is zero

### Combat
- [ ] Light attack triggers attack animation
- [ ] Attack combo chains (2-3 hits)
- [ ] Dodge roll moves player
- [ ] Dodge has invulnerability frames
- [ ] Dodge has cooldown
- [ ] Player hurt state triggers on damage

### Chaos Raccoon
- [ ] Raccoon spawns from encounter trigger
- [ ] Raccoon telegraphs before swiping
- [ ] Raccoon swipe deals damage
- [ ] Raccoon dash-past behavior works
- [ ] Raccoon takes damage from player attacks
- [ ] Raccoon defeat triggers appropriate feedback

### Maddie Companion
- [ ] Maddie follows player smoothly
- [ ] Maddie does not block player movement
- [ ] Maddie assists in combat on cooldown
- [ ] Maddie assist deals damage to enemies

### Tired and Respawn
- [ ] Energy reaching zero triggers tired state
- [ ] Tired outdoors respawns at home bed
- [ ] Tired in school respawns at school entrance
- [ ] Tired in arcade respawns at arcade entrance
- [ ] Energy restored to 50% on respawn

### Arcade Claw Machine
- [ ] Claw machine interactable in arcade
- [ ] Minigame UI opens on interact
- [ ] Marker oscillates left-right
- [ ] Drop button stops marker
- [ ] Prize awarded based on accuracy
- [ ] Gems and candy bars added to inventory
- [ ] Minigame can be replayed

### Save System
- [ ] Save completes without errors
- [ ] Save file created in correct location
- [ ] Load restores player position
- [ ] Load restores player rotation/facing
- [ ] Load restores inventory (gems, candy)
- [ ] Load restores energy state
- [ ] Load restores discovered notes
- [ ] Load restores revealed doors
- [ ] Load restores story flags
- [ ] Corrupted save falls back to home bed safely

### Scene Transitions
- [ ] Entering school loads SchoolInterior
- [ ] Exiting school returns to Cloverhollow
- [ ] Entering arcade loads ArcadeInterior
- [ ] Exiting arcade returns to Cloverhollow
- [ ] Fade transition visible during loads
- [ ] Player spawns at correct anchor after transition

## Content Verification

### Notes (check all 8)
- [ ] Note 1: "Glow Time" near home
- [ ] Note 2: "Follow Me" on road to school
- [ ] Note 3: "Not All Posters" in school hallway
- [ ] Note 4: "Star Corner" near art room
- [ ] Note 5: "Snack Stash" in hidden room
- [ ] Note 6: "Trash Bandit" in park
- [ ] Note 7: "Paw Gate" near hedge
- [ ] Note 8: "Prize Machine" in arcade

### Hidden Doors (check both)
- [ ] School "Star Closet" reveals and opens
- [ ] Park "Paw Hedge Gate" reveals and opens

### Anchors
- [ ] Home Bed anchor exists and works
- [ ] School Entrance anchor exists and works
- [ ] Arcade Entrance anchor exists and works

## Platform Checks

### Desktop
- [ ] Keyboard input works (WASD, E, Space, etc.)
- [ ] Mouse can click UI buttons

### Touch (if testing on device)
- [ ] Virtual joystick moves player
- [ ] Touch buttons respond correctly
- [ ] UI panels respond to touch

## Debug Overlay (dev builds only)
- [ ] Toggle overlay works (F1 or on-screen button)
- [ ] Save button saves game
- [ ] Load button loads game
- [ ] Teleport Home works
- [ ] Grant Candy adds candy
- [ ] Grant Gems adds gems
- [ ] Toggle Lantern unlocks/locks lantern
- [ ] Spawn Raccoon creates raccoon near player

## Performance (target: iPad)
- [ ] Stable 30+ FPS during exploration
- [ ] No frame drops during combat
- [ ] No frame drops during scene transitions
- [ ] No visible GC spikes in profiler
