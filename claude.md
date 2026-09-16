# AI Pirate World — Claude Code Instructions

## Project

Name:
AI Pirate World: The Lost Treasure

Engine:
Unity 6.3 LTS

Language:
C#

IDE:
VS Code

Version Control:
Git / GitHub

AI Coding Assistant:
Claude Code

---

## Project Architecture

The project is a modular 3D treasure-hunting game.

Major systems:

1. Game Systems & Gameplay
2. AI Content Generation
3. World & Environment
4. IoT & CNS Security

---

## Person 1 — Gameplay Ownership

Person 1 owns:

- GameManager
- GameState
- GameProgress
- Input abstraction
- ShipController
- Interaction system
- Clue system
- Puzzle system
- Objective system
- Treasure system
- Dynamic event system

---

## Integration Rules

Gameplay systems must remain independent from:

- AI providers
- MQTT
- ESP32 hardware
- CNS authentication
- specific environment GameObjects

Use interfaces and data contracts for communication
between systems.

Use stable IDs for:

- islands
- locations
- clues
- puzzles
- objectives
- treasures
- events

Avoid hard-coded scene object names.

---

## Coding Rules

- Use C#
- Use namespaces
- Prefer modular architecture
- Prefer data-driven systems
- Use ScriptableObjects where appropriate
- Avoid unnecessary singletons
- Avoid hard-coded gameplay content
- Do not modify unrelated systems
- Do not rewrite existing systems without permission
- Keep public APIs stable
- Add comments for non-obvious logic
- Use Unity-compatible APIs

---

## Team Compatibility

Person 2 must be able to provide AI-generated content
without modifying gameplay core systems.

Person 3 must be able to create islands and locations
without modifying gameplay core systems.

Person 4 must be able to provide ESP32 input through
an input interface without modifying ShipController.

CNS authentication and authorization must remain
separate from gameplay logic.

---

## Git Rules

Never directly commit to main.

Use feature branches.

Make small commits.

Use descriptive commit messages.

Before modifying files:
inspect the existing implementation.

Do not assume files exist.

Do not delete existing systems unless explicitly instructed.