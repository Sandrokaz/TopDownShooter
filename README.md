# 2D Multiplayer Top-Down Shooter (University Project)

This repository contains the Unity project for a 2D Multiplayer Top-Down Shooter game. The game features real-time multiplayer gameplay enabled by Photon Unity Networking (PUN) and offers a dynamic combat experience in a top-down perspective.

## Project Overview

The game incorporates several systems to manage gameplay, networking, audio, and user interface, ensuring a comprehensive and interactive player experience.

### Core Gameplay

- **Bullet**: Handles bullet dynamics including instantiation, movement, and collision detection.
- **EntityBase**: Serves as the base class for all game entities, providing foundational attributes and functions.
- **NPC**: Manages behaviors for non-player characters, including AI and interaction mechanics.
- **PlayerController**: Processes player input for movement and actions such as shooting.
- **ViewCone**: Determines and visualizes the field of view for both NPCs and players.

### User Interface

- **HealthBar**: Displays health status for characters.
- **GameUIManager**: Controls all main UI elements in the game, including overlays, menus, and game-related notifications.

### Game Management

- **GameManager**: Central manager for game state control, including start, pause, and end game conditions.
- **GameSceneManager**: Manages transitions and settings between different scenes.
- **NetworkSceneManager**: Coordinates scene management across networked players to ensure synchronization.
- **NetworkUIManager**: Manages network-specific UI elements, such as multiplayer lobbies and network messages.

### Audio

- **GameAudioManager**: Manages and plays all game sounds, ensuring audio cues match game events.

### Networking

- **CustomRoom**: Custom script for handling Photon's room settings, facilitating the creation and management of multiplayer rooms.

## Scripts Overview

The scripts are organized into modules based on their functionality, simplifying navigation and maintenance. Each module, whether handling gameplay mechanics, UI, or networking, is designed to function independently yet integrate seamlessly for a cohesive game experience.



## Contributing

Contributions are welcome. If you'd like to improve features or suggest changes:
1. Fork this repository.
2. Create a new branch for your updates.
3. Commit your changes and push to your branch.
4. Submit a pull request with a clear description of your modifications.

## License

This project is licensed under the MIT License. See the LICENSE file in the repository for more details.
