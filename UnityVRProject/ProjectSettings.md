# Unity VR Project Settings for Meta Quest 3

## Required Unity Version
- Unity 2022.3 LTS or newer
- Android Build Support module

## Package Manager Dependencies

### Core XR Packages
```
com.unity.xr.interaction.toolkit
com.unity.xr.legacyinputhelpers
com.unity.inputsystem
```

### Meta XR Integration
```
com.meta.xr.sdk.core
com.meta.xr.sdk.oculus
```

### UI and Rendering
```
com.unity.textmeshpro
com.unity.render-pipelines.universal
```

## Project Settings Configuration

### Player Settings (Android)
```
Company Name: La Trobe University
Product Name: La Trobe 3D VR Campus
Package Name: com.latrobe.campusvr
Version: 1.0.0

Target Architectures: ARM64
Scripting Backend: IL2CPP
API Compatibility Level: .NET Standard 2.1
```

### XR Plug-in Management
```
Enable XR Plug-in Management: Yes
Oculus: Enabled
```

### Quality Settings
```
Anti Aliasing: 4x Multi Sampling
Texture Quality: Full Res
Anisotropic Textures: Per Texture
Soft Particles: Enabled
```

### Universal Render Pipeline Settings
```
Main Light: Directional Light
Shadow Resolution: High
Shadow Distance: 50
```

## Scene Setup Instructions

### 1. Create Main Scene
- Create new scene: "LaTrobeVRScene"
- Set as default scene in Build Settings

### 2. Setup XR Origin
```
GameObject > XR > XR Origin (VR)
- Add XR Origin component
- Add Camera Offset child
- Add Main Camera as child of Camera Offset
```

### 3. Setup Controllers
```
GameObject > XR > XR Origin (VR)
- Add LeftHand Controller
- Add RightHand Controller
- Configure ActionBasedController components
```

### 4. Setup Interaction Manager
```
GameObject > XR > XR Interaction Manager
- Add XR Interaction Manager component
```

### 5. Setup Input Actions
```
Create Input Action Asset: "VRControls"
- Left Thumbstick (2D Vector)
- Right Thumbstick (2D Vector)
- Left Trigger (1D Axis)
- Right Trigger (1D Axis)
- Left Grip (1D Axis)
- Right Grip (1D Axis)
- Left Primary Button (Button)
- Right Primary Button (Button)
- Left Secondary Button (Button)
- Right Secondary Button (Button)
```

### 6. Setup Movement
```
Add to XR Origin:
- ActionBasedContinuousMoveProvider
- ActionBasedSnapTurnProvider
- Configure input actions
```

## Build Settings

### Android Settings
```
Target Device: Quest 3
Graphics APIs: OpenGLES3
Scripting Define Symbols: OCULUS
```

### Development Build
```
Development Build: Enabled
Script Debugging: Enabled
Deep Profiling Support: Enabled
```

## Performance Optimization

### Quest 3 Specific
```
Target Frame Rate: 90 FPS
Dynamic Resolution: Enabled
Foveated Rendering: Enabled
```

### Graphics Settings
```
Texture Compression: ASTC
Mesh Compression: Medium
Audio Compression: Vorbis
```

## Testing Setup

### Meta Quest Developer Hub
```
Install Meta Quest Developer Hub
Enable Developer Mode on Quest 3
Connect via USB or Wi-Fi
```

### Build and Deploy
```
Build APK
Install via Meta Quest Developer Hub
Test in VR environment
```

## Troubleshooting

### Common Issues
1. **Controllers not detected**: Check XR Interaction Manager setup
2. **Performance issues**: Enable foveated rendering and dynamic resolution
3. **Build errors**: Ensure all XR packages are installed
4. **UI not visible**: Check canvas render mode and camera setup

### Debug Tools
```
Enable XR Debugger in Window > Analysis > XR Debugger
Use Frame Debugger for performance analysis
Enable XR Interaction Debugger for interaction issues
``` 