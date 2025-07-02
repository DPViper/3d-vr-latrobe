using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

namespace LaTrobeVR
{
    public class VRBuilding : MonoBehaviour
    {
        [Header("Building Info")]
        public VRSceneManager.BuildingData buildingData;
        public VRSceneManager sceneManager;
        
        [Header("Visual Elements")]
        public GameObject waypointMarker;
        public TextMeshPro waypointText;
        public GameObject buildingModel;
        
        [Header("Interaction")]
        public XRSimpleInteractable interactable;
        public Material selectedMaterial;
        public Material unselectedMaterial;
        
        private Renderer buildingRenderer;
        private bool isSelected = false;
        
        public void Initialize(VRSceneManager.BuildingData data, VRSceneManager manager)
        {
            buildingData = data;
            sceneManager = manager;
            
            // Setup building model
            if (buildingModel == null)
            {
                buildingModel = transform.Find("BuildingModel")?.gameObject;
            }
            
            if (buildingModel != null)
            {
                buildingRenderer = buildingModel.GetComponent<Renderer>();
                LoadBuildingModel();
            }
            
            // Setup waypoint marker
            SetupWaypointMarker();
            
            // Setup interaction
            SetupInteraction();
            
            // Set initial position
            transform.position = buildingData.position;
            transform.rotation = Quaternion.Euler(buildingData.rotation);
        }
        
        void LoadBuildingModel()
        {
            // Load the 3D model (splat file or converted model)
            // For now, we'll create a placeholder cube
            if (buildingModel == null)
            {
                buildingModel = GameObject.CreatePrimitive(PrimitiveType.Cube);
                buildingModel.name = "BuildingModel";
                buildingModel.transform.SetParent(transform);
                buildingModel.transform.localPosition = Vector3.zero;
                
                buildingRenderer = buildingModel.GetComponent<Renderer>();
            }
            
            // Set material based on selection state
            UpdateMaterial();
        }
        
        void SetupWaypointMarker()
        {
            if (waypointMarker == null)
            {
                // Create waypoint marker
                waypointMarker = new GameObject("WaypointMarker");
                waypointMarker.transform.SetParent(transform);
                
                // Add visual elements
                GameObject markerVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                markerVisual.transform.SetParent(waypointMarker.transform);
                markerVisual.transform.localScale = Vector3.one * 0.5f;
                
                // Add text
                GameObject textObj = new GameObject("WaypointText");
                textObj.transform.SetParent(waypointMarker.transform);
                textObj.transform.localPosition = Vector3.up * 1.5f;
                
                waypointText = textObj.AddComponent<TextMeshPro>();
                waypointText.text = buildingData.name;
                waypointText.fontSize = 0.5f;
                waypointText.color = Color.white;
                waypointText.alignment = TextAlignmentOptions.Center;
                
                // Position marker above building
                waypointMarker.transform.localPosition = Vector3.up * 2f;
            }
            
            // Update text
            if (waypointText != null)
            {
                waypointText.text = buildingData.name;
            }
        }
        
        void SetupInteraction()
        {
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRSimpleInteractable>();
            }
            
            interactable.selectEntered.AddListener(OnBuildingSelected);
            interactable.hoverEntered.AddListener(OnBuildingHovered);
            interactable.hoverExited.AddListener(OnBuildingUnhovered);
        }
        
        void OnBuildingSelected(SelectEnterEventArgs args)
        {
            if (sceneManager != null)
            {
                sceneManager.SelectBuilding(buildingData.id);
            }
        }
        
        void OnBuildingHovered(HoverEnterEventArgs args)
        {
            if (waypointText != null)
            {
                waypointText.color = new Color(0.95f, 0.57f, 0.6f); // Light red
            }
        }
        
        void OnBuildingUnhovered(HoverExitEventArgs args)
        {
            UpdateWaypointColor();
        }
        
        public void SetSelected(bool selected)
        {
            isSelected = selected;
            UpdateMaterial();
            UpdateWaypointColor();
        }
        
        void UpdateMaterial()
        {
            if (buildingRenderer != null)
            {
                buildingRenderer.material = isSelected ? selectedMaterial : unselectedMaterial;
            }
        }
        
        void UpdateWaypointColor()
        {
            if (waypointText != null)
            {
                waypointText.color = isSelected ? Color.red : Color.white;
            }
        }
        
        void Update()
        {
            // Make waypoint marker always face camera
            if (waypointMarker != null && Camera.main != null)
            {
                waypointMarker.transform.LookAt(Camera.main.transform);
                waypointMarker.transform.Rotate(0, 180, 0);
            }
        }
        
        // IXRInteractable implementation
        public bool isHovered { get; private set; }
        
        public void OnHoverEntered(HoverEnterEventArgs args)
        {
            isHovered = true;
            OnBuildingHovered(args);
        }
        
        public void OnHoverExited(HoverExitEventArgs args)
        {
            isHovered = false;
            OnBuildingUnhovered(args);
        }
        
        public void OnSelectEntered(SelectEnterEventArgs args)
        {
            isSelected = true;
            OnBuildingSelected(args);
        }
        
        public void OnSelectExited(SelectExitEventArgs args)
        {
            isSelected = false;
        }
    }
} 