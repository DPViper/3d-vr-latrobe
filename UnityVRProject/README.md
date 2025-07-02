# La Trobe 3D VR - Unity Native App

This is the Unity conversion of the La Trobe 3D Campus Viewer for Meta Quest 3.

## Project Structure

```
Assets/
├── Scripts/           # C# scripts
├── Scenes/           # Unity scenes
├── Models/           # 3D models and splat files
├── Materials/        # Materials and shaders
├── Prefabs/          # Reusable game objects
├── UI/               # VR UI elements
└── Settings/         # Project settings
```

## Requirements

- Unity 2022.3 LTS or newer
- Meta XR SDK (Oculus Integration)
- Android Build Support
- Universal Render Pipeline (URP)

## Setup Instructions

1. Create a new Unity 3D project
2. Install Meta XR SDK from Package Manager
3. Configure project for Android/Quest
4. Import the provided scripts and assets
5. Set up the main scene with VR camera rig

## Building for Quest 3

1. Set platform to Android
2. Configure Player Settings for Quest
3. Build APK and install via Meta Quest Developer Hub

## Features

- Native VR performance
- Controller-based navigation
- Interactive building selection
- Information panels in VR space
- Optimized for Quest 3 hardware 