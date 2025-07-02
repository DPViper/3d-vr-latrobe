using UnityEngine;
using Unity.XR.CoreUtils;

namespace LaTrobeVR
{
    public class ScriptTest : MonoBehaviour
    {
        [Header("Test Configuration")]
        public bool runTestsOnStart = true;
        
        void Start()
        {
            if (runTestsOnStart)
            {
                RunTests();
            }
        }
        
        [ContextMenu("Run Script Tests")]
        public void RunTests()
        {
            Debug.Log("=== La Trobe VR Script Tests ===");
            
            // Test 1: Check if all required components can be created
            TestComponentCreation();
            
            // Test 2: Check if namespaces are correct
            TestNamespaces();
            
            // Test 3: Check if required packages are available
            TestRequiredPackages();
            
            Debug.Log("=== Script Tests Complete ===");
        }
        
        void TestComponentCreation()
        {
            Debug.Log("Testing component creation...");
            
            try
            {
                // Test VRSceneManager
                GameObject testObj1 = new GameObject("Test_VRSceneManager");
                VRSceneManager sceneManager = testObj1.AddComponent<VRSceneManager>();
                Debug.Log("✓ VRSceneManager component created successfully");
                DestroyImmediate(testObj1);
                
                // Test VRBuilding
                GameObject testObj2 = new GameObject("Test_VRBuilding");
                VRBuilding building = testObj2.AddComponent<VRBuilding>();
                Debug.Log("✓ VRBuilding component created successfully");
                DestroyImmediate(testObj2);
                
                // Test VRInfoPanel
                GameObject testObj3 = new GameObject("Test_VRInfoPanel");
                VRInfoPanel infoPanel = testObj3.AddComponent<VRInfoPanel>();
                Debug.Log("✓ VRInfoPanel component created successfully");
                DestroyImmediate(testObj3);
                
                // Test VRBuildingList
                GameObject testObj4 = new GameObject("Test_VRBuildingList");
                VRBuildingList buildingList = testObj4.AddComponent<VRBuildingList>();
                Debug.Log("✓ VRBuildingList component created successfully");
                DestroyImmediate(testObj4);
                
                // Test VRControllerInput
                GameObject testObj5 = new GameObject("Test_VRControllerInput");
                VRControllerInput controllerInput = testObj5.AddComponent<VRControllerInput>();
                Debug.Log("✓ VRControllerInput component created successfully");
                DestroyImmediate(testObj5);
                
                // Test SplatModelConverter
                GameObject testObj6 = new GameObject("Test_SplatModelConverter");
                SplatModelConverter converter = testObj6.AddComponent<SplatModelConverter>();
                Debug.Log("✓ SplatModelConverter component created successfully");
                DestroyImmediate(testObj6);
                
                // Test BuildScript
                GameObject testObj7 = new GameObject("Test_BuildScript");
                BuildScript buildScript = testObj7.AddComponent<BuildScript>();
                Debug.Log("✓ BuildScript component created successfully");
                DestroyImmediate(testObj7);
                
            }
            catch (System.Exception e)
            {
                Debug.LogError($"✗ Component creation test failed: {e.Message}");
            }
        }
        
        void TestNamespaces()
        {
            Debug.Log("Testing namespaces...");
            
            // Check if all required types are available
            System.Type[] requiredTypes = {
                typeof(VRSceneManager),
                typeof(VRBuilding),
                typeof(VRInfoPanel),
                typeof(VRBuildingList),
                typeof(VRControllerInput),
                typeof(SplatModelConverter),
                typeof(BuildScript)
            };
            
            foreach (System.Type type in requiredTypes)
            {
                if (type != null)
                {
                    Debug.Log($"✓ {type.Name} namespace verified");
                }
                else
                {
                    Debug.LogError($"✗ {type.Name} namespace not found");
                }
            }
        }
        
        void TestRequiredPackages()
        {
            Debug.Log("Testing required packages...");
            
            // Check for XR Interaction Toolkit
            if (System.Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRInteractionManager") != null)
            {
                Debug.Log("✓ XR Interaction Toolkit available");
            }
            else
            {
                Debug.LogWarning("⚠ XR Interaction Toolkit not found - install via Package Manager");
            }
            
            // Check for Input System
            if (System.Type.GetType("UnityEngine.InputSystem.InputAction") != null)
            {
                Debug.Log("✓ Input System available");
            }
            else
            {
                Debug.LogWarning("⚠ Input System not found - install via Package Manager");
            }
            
            // Check for TextMeshPro
            if (System.Type.GetType("TMPro.TextMeshPro") != null)
            {
                Debug.Log("✓ TextMeshPro available");
            }
            else
            {
                Debug.LogWarning("⚠ TextMeshPro not found - install via Package Manager");
            }
        }
        
        [ContextMenu("Create Test Scene Setup")]
        public void CreateTestSceneSetup()
        {
            Debug.Log("Creating test scene setup...");
            
            // Create XR Origin
            GameObject xrOrigin = new GameObject("XR Origin");
            xrOrigin.AddComponent<Unity.XR.CoreUtils.XROrigin>();
            
            // Create Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOrigin.transform);
            
            // Create Main Camera
            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraOffset.transform);
            mainCamera.transform.localPosition = new Vector3(0, 1.6f, 0);
            mainCamera.AddComponent<Camera>();
            mainCamera.tag = "MainCamera";
            
            // Create VR Scene Manager
            GameObject sceneManager = new GameObject("VR Scene Manager");
            sceneManager.AddComponent<VRSceneManager>();
            
            // Create VR Info Panel
            GameObject infoPanel = new GameObject("VR Info Panel");
            infoPanel.AddComponent<VRInfoPanel>();
            
            // Create VR Building List
            GameObject buildingList = new GameObject("VR Building List");
            buildingList.AddComponent<VRBuildingList>();
            
            // Create VR Controller Input
            GameObject controllerInput = new GameObject("VR Controller Input");
            controllerInput.AddComponent<VRControllerInput>();
            
            Debug.Log("✓ Test scene setup created successfully");
            Debug.Log("You can now configure the components in the Inspector");
        }
    }
}