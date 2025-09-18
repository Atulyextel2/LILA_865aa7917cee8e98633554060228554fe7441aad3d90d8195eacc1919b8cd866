# LILA_865aa7917cee8e98633554060228554fe7441aad3d90d8195eacc1919b8cd866
Unity Data Optimisation &amp; Weapon System Demo  A Unity demo project built to practice data optimisation for multiplayer position sync (Task 1) and a class-only weapon system with HUD integration (Task 2). The project applies OOP principles, SOLID design, and design patterns to keep code modular, testable, and extensible.
🔹 Task 1: Position Sync & Data Optimisation
	•	Absolute + Delta Compression
	•	First snapshot = ABS (32-bit ints, supports any start position).
	•	Subsequent updates = DELTA (16-bit ints, just 9 bytes).
	•	Automatic ABS fallback if deltas overflow → prevents drift/clamping.
	•	Snapshot Interpolator (time-buffered)
	•	Remote movement is smoothed by sampling now − delay.
	•	Seeding on first ABS → no visible lag at start.
	•	Unity Integration
	•	Pure C# domain (quantiser, codec, interpolator).
	•	Thin Unity behaviours for sending/receiving + HUD binding.
	•	Simple WASD local movement → compressed → loopback → remote replay.
	•	HUD Debugging
	•	On-screen stats for sent bytes & received positions.
	•	Console logs distinguish ABS vs DELTA.

⸻

🔹 Task 2: Weapon System & HUD
	•	Domain-only weapon system (no MonoBehaviours in core logic):
	•	WeaponBase, Rifle, Pistol, PlayerCombat, WeaponLoadout.
	•	Handles ammo, fire rate, reload times, weapon switching.
	•	Event-driven architecture
	•	WeaponBus events for fire/reload.
	•	Easy to hook up to any UI or gameplay system.
	•	HUD Presenter (MVP pattern)
	•	WeaponHudPresenter builds a HUD view-model from domain state.
	•	UnityWeaponHudView renders it into uGUI Texts.
	•	Shows:
	•	Current active weapon.
	•	Current ammo (mag).
	•	Total ammo (mag + reserve).
	•	Which slot is equipped (P1, P2, S).
	•	Clean separation
	•	Core logic is pure C#, independent of Unity.
	•	Thin MonoBehaviours only for demo scene hosting.

⸻

🛠️ Tech Stack
	•	Unity 2022.3 LTS
	•	C# (OOP, SOLID, events, dependency inversion)
	•	Custom bit-packed networking
	•	Unity UI (uGUI) for HUDs

⸻

🚀 How to Run
	1.	Clone the repo.
	2.	Open in Unity 2022.3 LTS.
	3.	Open Task_1 and Task_2 scenes.

Controls

Task 1 (Networking demo)
	•	WASD → move LocalPlayer capsule.
	•	RemotePlayer capsule follows using compressed network snapshots.
	•	HUD shows Sent/Received data; Console logs show ABS/DELTA packets.

Task 2 (Weapons demo)
	•	F → fire active weapon.
	•	R → reload.
	•	1 / 2 / 3 → switch between P1 / P2 / S slots.
	•	HUD updates active weapon, current & total ammo.
