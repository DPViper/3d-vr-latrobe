using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Unity.XR.CoreUtils;

namespace LaTrobeVR
{
    public class VRSceneManager : MonoBehaviour
    {
        [Header("VR Setup")]
        public XROrigin xrOrigin;
        public ActionBasedContinuousMoveProvider moveProvider;
        public ActionBasedSnapTurnProvider turnProvider;
        
        [Header("Building Data")]
        public BuildingData[] buildings;
        public GameObject buildingPrefab;
        public Transform buildingsParent;
        
        [Header("UI")]
        public VRInfoPanel infoPanel;
        public VRBuildingList buildingList;
        
        [Header("Navigation")]
        public float teleportDistance = 10f;
        public LayerMask teleportLayerMask = -1;
        
        private int currentBuildingID = 1;
        private Dictionary<int, GameObject> buildingInstances = new Dictionary<int, GameObject>();
        private Camera vrCamera;
        
        [System.Serializable]
        public class BuildingData
        {
            public int id;
            public string name;
            public string code;
            public string closestCarPark;
            public string filepath;
            public Vector3 position;
            public Vector3 rotation;
            public int constructed;
            public string description;
        }
        
        void Start()
        {
            InitializeVR();
            LoadBuildingData();
            SetupVRControls();
            LoadBuildings();
        }
        
        void InitializeVR()
        {
            vrCamera = Camera.main;
            if (vrCamera == null)
            {
                Debug.LogError("No main camera found!");
                return;
            }
            
            // Set initial VR camera position
            vrCamera.transform.position = new Vector3(0, 1.6f, 5f);
        }
        
        void LoadBuildingData()
        {
            // Load building data from JSON or ScriptableObject
            // For now, we'll use the data from your original modelData.json
            buildings = new BuildingData[]
            {
                new BuildingData
                {
                    id = 1,
                    name = "Graduate House - East",
                    code = "NR7",
                    closestCarPark = "CP8",
                    filepath = "NR7.splat",
                    position = new Vector3(50, 0, 0),
                    rotation = Vector3.zero,
                    constructed = 1970,
                    description = "An old accommodation, when it was still in use, the rent wasn't this high if you ask."
                },
                new BuildingData
                {
                    id = 2,
                    name = "Jenny Graves",
                    code = "JG",
                    closestCarPark = "VCP",
                    filepath = "JG.splat",
                    position = new Vector3(0, -0.5f, 3),
                    rotation = Vector3.zero,
                    constructed = 2024,
                    description = "Formerly Thomas Cherry, the upgrade of the building in 2024 involved adaptive reuse of the existing 1960 structure with new facade glazing and fabric, roof, lift shaft and stair well that transformed it into into a new innovation precinct."
                },
                new BuildingData
                {
                    id = 3,
                    name = "Dante's Divine Comedy",
                    code = "JG-BST",
                    closestCarPark = "VCP",
                    filepath = "JG-BST.splat",
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    constructed = 1983,
                    description = "Sculpted by Bart Sanciolo across 1980 to 1983. The bronze depicts scenes from Dante's work. It was installed at the University in 1987, and is adjacent to the Thomas Cherry Building, and proximate to the Physical Sciences Building. The sculpture was provided by the Italian community, and draws on the most famous text from Italy's most celebrated author.\n A weird Pyramid, definitely has nothing to do with mummies..."
                },
                new BuildingData
                {
                    id = 4,
                    name = "Bus Stops",
                    code = "BST",
                    closestCarPark = "VCP",
                    filepath = "BST.splat",
                    position = new Vector3(-1.85f, 0, -7.5f),
                    rotation = Vector3.zero,
                    constructed = 1970,
                    description = "And always remember to touch on your myki at the start of your journey."
                },
                new BuildingData
                {
                    id = 5,
                    name = "John Scott Meeting House",
                    code = "JSM",
                    closestCarPark = "CP7A",
                    filepath = "JSM.splat",
                    position = new Vector3(0, 0, 50),
                    rotation = Vector3.zero,
                    constructed = 1993,
                    description = "Named after a Cambridge University alumni John Fraser Scott, who was the second vice-chancellor of the university during its difficult period after years of expansion as there were no funds, no growth and static enrolments.\n However, under his leadership, La Trobe was still able to move forward with higher degree student numbers doubled, research increased, teaching improved and courses were diversified. Two new schools were established, new departments were established, and new fields undertaken such as Italian and computer science."
                }
            };
        }
        
        void SetupVRControls()
        {
            // Configure VR movement and turning
            if (moveProvider != null)
            {
                moveProvider.moveSpeed = 2f;
                moveProvider.enableStrafe = true;
            }
            
            if (turnProvider != null)
            {
                turnProvider.turnAmount = 45f;
            }
        }
        
        void LoadBuildings()
        {
            foreach (var building in buildings)
            {
                CreateBuildingInstance(building);
            }
        }
        
        void CreateBuildingInstance(BuildingData building)
        {
            // Create building instance
            GameObject buildingInstance = Instantiate(buildingPrefab, buildingsParent);
            buildingInstance.transform.position = building.position;
            buildingInstance.transform.rotation = Quaternion.Euler(building.rotation);
            
            // Add building component
            VRBuilding buildingComponent = buildingInstance.GetComponent<VRBuilding>();
            if (buildingComponent == null)
            {
                buildingComponent = buildingInstance.AddComponent<VRBuilding>();
            }
            
            buildingComponent.Initialize(building, this);
            buildingInstances[building.id] = buildingInstance;
        }
        
        public void SelectBuilding(int buildingID)
        {
            currentBuildingID = buildingID;
            
            // Update UI
            if (infoPanel != null)
            {
                infoPanel.ShowBuildingInfo(GetBuildingData(buildingID));
            }
            
            // Teleport to building
            TeleportToBuilding(buildingID);
        }
        
        public void TeleportToBuilding(int buildingID)
        {
            if (buildingInstances.ContainsKey(buildingID))
            {
                Vector3 buildingPosition = buildingInstances[buildingID].transform.position;
                Vector3 teleportPosition = buildingPosition + Vector3.up * 1.6f + Vector3.back * 5f;
                
                // Use XR Origin for teleportation
                if (xrOrigin != null)
                {
                    xrOrigin.MoveCameraToWorldLocation(teleportPosition);
                }
            }
        }
        
        public BuildingData GetBuildingData(int buildingID)
        {
            foreach (var building in buildings)
            {
                if (building.id == buildingID)
                    return building;
            }
            return null;
        }
        
        public BuildingData[] GetAllBuildings()
        {
            return buildings;
        }
        
        public int GetCurrentBuildingID()
        {
            return currentBuildingID;
        }
    }
} 