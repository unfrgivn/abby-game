# PoC regression checklist (macOS + iOS)

Use this checklist before merging major changes and before shipping a PoC build.

## 1) Boot + basic navigation
- [ ] Game launches to the expected start scene
- [ ] Player can move (WASD + joystick)
- [ ] Interact button works on touch and mouse/controller
- [ ] Scene transitions (House ↔ School ↔ Park ↔ Arcade) work without errors

## 2) Lantern + hidden content
- [ ] Lantern toggles/holds as designed
- [ ] Hidden notes reveal when scanned and stay discovered after leaving the scene
- [ ] Hidden doors reveal when scanned and become interactable
- [ ] Hidden doors remain unlocked after save/load

## 3) Journal
- [ ] Journal opens/closes reliably
- [ ] Newly discovered notes appear in the journal
- [ ] Opening journal during/after scene transitions does not break UI focus

## 4) Sticker Book
- [ ] Sticker Book opens/closes reliably
- [ ] New game grants starter stickers
- [ ] Player can equip exactly 4 stickers
- [ ] Equipped loadout persists after save/load

## 5) Battles
- [ ] Visible encounter reliably starts the correct battle
- [ ] Command menu: Stickers / Items / Defend / Run all respond
- [ ] Using a sticker applies the correct effect and starts cooldown
- [ ] Stickers on cooldown are disabled and communicate cooldown clearly
- [ ] Victory returns to overworld and grants rewards
- [ ] First win grants the reward sticker exactly once
- [ ] Loss triggers tired respawn rules and does not corrupt save
- [ ] Run returns to overworld near the encounter (no soft-lock)

## 6) Arcade claw machine
- [ ] Claw machine triggers the mini-game
- [ ] Rewards are granted and reflected in inventory/currency
- [ ] Multiple plays do not duplicate unique rewards unintentionally

## 7) Save/load integrity
- [ ] Save anywhere does not freeze or crash
- [ ] Reload restores current scene + player position (or safe fallback)
- [ ] Reload restores notes/doors discovery state
- [ ] Reload restores sticker inventory + loadout
- [ ] Reload restores currency

## 8) Mobile UX sanity
- [ ] Tap targets are not cramped; mis-taps are rare
- [ ] UI scales correctly on iPhone and iPad
- [ ] Battle UI remains readable at smallest supported size
