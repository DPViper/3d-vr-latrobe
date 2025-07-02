# La Trobe 3D VR - Installation and Setup Guide

## Prerequisites

### Required Software
1. **Unity Hub** - Download from [unity.com](https://unity.com/download)
2. **Unity 2022.3 LTS** - Install via Unity Hub
3. **Android Studio** - Download from [developer.android.com](https://developer.android.com/studio)
4. **Meta Quest Developer Hub** - Download from [developer.oculus.com](https://developer.oculus.com/downloads/)

### Hardware Requirements
- **Meta Quest 3** (or Quest 2/Pro for testing)
- **USB-C cable** for development
- **PC with minimum specs**:
  - Windows 10/11
  - 16GB RAM
  - GTX 1060 or better
  - 50GB free space

## Step 1: Unity Setup

### 1.1 Create New Unity Project
```
1. Open Unity Hub
2. Click "New Project"
3. Select "3D (URP)" template
4. Name: "LaTrobe3DVR"
5. Location: Choose your preferred directory
6. Click "Create project"
```

### 1.2 Install Required Packages
Open **Window > Package Manager** and install:

#### Core XR Packages
```
- XR Interaction Toolkit
- XR Legacy Input Helpers
- Input System
```

#### Meta XR Integration
```
- Oculus XR Plugin
- Oculus Integration (from Asset Store)
```

#### UI and Rendering
```
- TextMeshPro
- Universal Render Pipeline
```

### 1.3 Configure Project Settings

#### Player Settings
```
1. Edit > Project Settings > Player
2. Android tab:
   - Company Name: La Trobe University
   - Product Name: La Trobe 3D VR Campus
   - Package Name: com.latrobe.campusvr
   - Target Architectures: ARM64
   - Scripting Backend: IL2CPP
   - API Compatibility: .NET Standard 2.1
```

#### XR Plug-in Management
```
1. Edit > Project Settings > XR Plug-in Management
2. Check "Initialize XR on Startup"
3. Android tab: Enable "Oculus"
```

#### Quality Settings
```
1. Edit > Project Settings > Quality
2. Set Android quality level to "High"
3. Enable "Anti Aliasing: 4x Multi Sampling"
```

## Step 2: Import Project Files

### 2.1 Copy Scripts
```
1. Create folder: Assets/Scripts/
2. Copy all .cs files from UnityVRProject/Assets/Scripts/ to Assets/Scripts/
3. Ensure namespace "LaTrobeVR" is preserved
```

### 2.2 Import 3D Models
```
1. Copy your .splat files to Assets/Models/Splats/
2. Use SplatModelConverter to convert to Unity format
3. Converted models will be saved to Assets/Models/Converted/
```

### 2.3 Setup Scene
```
1. Create new scene: Assets/Scenes/LaTrobeVRScene.unity
2. Save scene and add to Build Settings
```

## Step 3: Scene Configuration

### 3.1 Setup XR Origin
```
1. GameObject > XR > XR Origin (VR)
2. Add Camera Offset as child
3. Add Main Camera as child of Camera Offset
4. Position camera at (0, 1.6, 0) for eye height
```

### 3.2 Add Controllers
```
1. Right-click XR Origin > XR > LeftHand Controller
2. Right-click XR Origin > XR > RightHand Controller
3. Configure ActionBasedController components
```

### 3.3 Setup Interaction Manager
```
1. GameObject > XR > XR Interaction Manager
2. Add to scene root
```

### 3.4 Add VR Components
```
1. Add VRSceneManager to scene
2. Add VRInfoPanel to scene
3. Add VRBuildingList to scene
4. Add VRControllerInput to scene
5. Configure all references in Inspector
```

### 3.5 Setup Lighting
```
1. Add Directional Light
2. Position at (10, 10, 5)
3. Set intensity to 1.0
4. Enable shadows
```

## Step 4: Input System Setup

### 4.1 Create Input Actions
```
1. Right-click Project > Create > Input Actions
2. Name: "VRControls"
3. Add actions for:
   - Left/Right Thumbstick (2D Vector)
   - Left/Right Trigger (1D Axis)
   - Left/Right Grip (1D Axis)
   - Left/Right Primary Button (Button)
   - Left/Right Secondary Button (Button)
```

### 4.2 Configure Action Maps
```
1. Create "VR" action map
2. Bind actions to Oculus Touch controllers
3. Set up proper input references in VRControllerInput
```

## Step 5: Quest 3 Setup

### 5.1 Enable Developer Mode
```
1. Install Meta Quest app on phone
2. Connect Quest 3 to phone
3. Go to Settings > System > Developer Options
4. Enable "Developer Mode"
5. Restart Quest 3
```

### 5.2 Install Meta Quest Developer Hub
```
1. Download from developer.oculus.com
2. Install on PC
3. Connect Quest 3 via USB
4. Enable USB debugging when prompted
```

### 5.3 Test Connection
```
1. Open Meta Quest Developer Hub
2. Check device connection
3. Verify ADB connection
4. Test file transfer
```

## Step 6: Build and Deploy

### 6.1 Configure Build Settings
```
1. File > Build Settings
2. Platform: Android
3. Add LaTrobeVRScene to Scenes in Build
4. Click "Switch Platform"
```

### 6.2 Build APK
```
1. Use BuildScript component in scene
2. Right-click > Build for Quest 3
3. Or use menu: La Trobe VR > Build for Quest 3
4. Wait for build to complete
```

### 6.3 Deploy to Quest 3
```
1. Connect Quest 3 via USB
2. Enable USB debugging
3. Use BuildScript to deploy automatically
4. Or manually install APK via Meta Quest Developer Hub
```

## Step 7: Testing and Debugging

### 7.1 In-Editor Testing
```
1. Press Play in Unity
2. Use XR Device Simulator for testing
3. Test UI interactions
4. Verify building selection
```

### 7.2 Quest 3 Testing
```
1. Install app on Quest 3
2. Launch from app library
3. Test VR interactions
4. Verify performance (90 FPS target)
```

### 7.3 Debug Tools
```
1. Enable XR Debugger: Window > Analysis > XR Debugger
2. Use Frame Debugger for performance
3. Enable XR Interaction Debugger
4. Check console for errors
```

## Troubleshooting

### Common Issues

#### Build Errors
```
- Ensure all XR packages are installed
- Check Android SDK installation
- Verify JDK installation
- Check Unity version compatibility
```

#### Performance Issues
```
- Enable foveated rendering
- Reduce splat point count
- Use instanced rendering
- Optimize textures and materials
```

#### Controller Issues
```
- Check XR Interaction Manager setup
- Verify input action bindings
- Test controller tracking
- Check Quest 3 firmware
```

#### UI Not Visible
```
- Check canvas render mode
- Verify camera setup
- Test UI positioning
- Check layer settings
```

### Performance Optimization

#### Quest 3 Specific
```
- Target 90 FPS
- Use dynamic resolution
- Enable foveated rendering
- Optimize draw calls
- Use LOD for distant objects
```

#### Graphics Settings
```
- Use ASTC texture compression
- Enable mesh compression
- Use Vorbis audio compression
- Limit shadow distance
- Optimize lighting
```

## Next Steps

### Advanced Features
1. **Multiplayer Support** - Add Photon or Mirror networking
2. **Voice Commands** - Integrate speech recognition
3. **Hand Tracking** - Add hand gesture controls
4. **Spatial Audio** - Implement 3D audio
5. **Analytics** - Add usage tracking

### Distribution
1. **App Lab** - Submit to Meta App Lab
2. **Enterprise** - Deploy via MDM
3. **Custom Builds** - Create branded versions
4. **Updates** - Implement OTA updates

## Support

For technical support:
- Check Unity documentation
- Visit Meta Developer forums
- Review XR Interaction Toolkit docs
- Contact La Trobe IT support

## License

This project is licensed under the same terms as the original web application. 