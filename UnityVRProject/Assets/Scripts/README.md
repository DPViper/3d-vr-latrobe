# Scripts Setup Guide

## Step-by-Step Instructions

### 1. Create Folders in Unity
In your Unity project, create these folders:
```
Assets/
├── Scripts/
├── Scenes/
├── Models/
│   ├── Splats/
│   └── Converted/
├── Materials/
├── Prefabs/
└── UI/
```

### 2. Copy Scripts
Copy each script file to the correct location:

#### Core Scripts (Assets/Scripts/)
- `VRSceneManager.cs` - Main scene controller
- `VRBuilding.cs` - Individual building component
- `VRInfoPanel.cs` - VR information panel
- `VRBuildingList.cs` - Building list UI
- `VRControllerInput.cs` - Controller input handling
- `SplatModelConverter.cs` - Model conversion utility
- `BuildScript.cs` - Build and deployment system

### 3. Verify Scripts
After copying, Unity should automatically compile the scripts. Check the Console for any compilation errors.

### 4. Next Steps
Once scripts are copied and compiled successfully, proceed to Scene Configuration. 