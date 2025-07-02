using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

namespace LaTrobeVR
{
    public class VRControllerInput : MonoBehaviour
    {
        [Header("Controller References")]
        public ActionBasedController leftController;
        public ActionBasedController rightController;
        
        [Header("Input Actions")]
        public InputActionReference leftThumbstick;
        public InputActionReference rightThumbstick;
        public InputActionReference leftTrigger;
        public InputActionReference rightTrigger;
        public InputActionReference leftGrip;
        public InputActionReference rightGrip;
        public InputActionReference leftPrimaryButton;
        public InputActionReference rightPrimaryButton;
        public InputActionReference leftSecondaryButton;
        public InputActionReference rightSecondaryButton;
        
        [Header("VR Scene Manager")]
        public VRSceneManager sceneManager;
        
        [Header("UI Controls")]
        public VRInfoPanel infoPanel;
        public VRBuildingList buildingList;
        
        private bool infoPanelVisible = false;
        private bool buildingListVisible = false;
        
        void Start()
        {
            SetupInputActions();
        }
        
        void SetupInputActions()
        {
            // Left controller - movement and UI
            if (leftThumbstick != null)
            {
                leftThumbstick.action.performed += OnLeftThumbstickMoved;
                leftThumbstick.action.canceled += OnLeftThumbstickReleased;
            }
            
            if (leftPrimaryButton != null)
            {
                leftPrimaryButton.action.performed += OnLeftPrimaryButtonPressed;
            }
            
            if (leftSecondaryButton != null)
            {
                leftSecondaryButton.action.performed += OnLeftSecondaryButtonPressed;
            }
            
            // Right controller - interaction and UI
            if (rightThumbstick != null)
            {
                rightThumbstick.action.performed += OnRightThumbstickMoved;
                rightThumbstick.action.canceled += OnRightThumbstickReleased;
            }
            
            if (rightPrimaryButton != null)
            {
                rightPrimaryButton.action.performed += OnRightPrimaryButtonPressed;
            }
            
            if (rightSecondaryButton != null)
            {
                rightSecondaryButton.action.performed += OnRightSecondaryButtonPressed;
            }
            
            // Grips for additional controls
            if (leftGrip != null)
            {
                leftGrip.action.performed += OnLeftGripPressed;
            }
            
            if (rightGrip != null)
            {
                rightGrip.action.performed += OnRightGripPressed;
            }
        }
        
        void OnDestroy()
        {
            // Clean up input actions
            if (leftThumbstick != null)
                leftThumbstick.action.performed -= OnLeftThumbstickMoved;
            if (rightThumbstick != null)
                rightThumbstick.action.performed -= OnRightThumbstickMoved;
            if (leftPrimaryButton != null)
                leftPrimaryButton.action.performed -= OnLeftPrimaryButtonPressed;
            if (rightPrimaryButton != null)
                rightPrimaryButton.action.performed -= OnRightPrimaryButtonPressed;
        }
        
        void OnLeftThumbstickMoved(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            
            // Handle movement (this is typically handled by XR Interaction Toolkit)
            // But we can add custom movement logic here if needed
            
            // Use thumbstick for UI navigation
            if (buildingListVisible && Mathf.Abs(input.y) > 0.5f)
            {
                // Scroll building list
                ScrollBuildingList(input.y);
            }
        }
        
        void OnLeftThumbstickReleased(InputAction.CallbackContext context)
        {
            // Stop any continuous actions
        }
        
        void OnRightThumbstickMoved(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            
            // Handle camera rotation or other right controller actions
            if (Mathf.Abs(input.x) > 0.5f)
            {
                // Rotate camera or handle horizontal input
            }
        }
        
        void OnRightThumbstickReleased(InputAction.CallbackContext context)
        {
            // Stop any continuous actions
        }
        
        void OnLeftPrimaryButtonPressed(InputAction.CallbackContext context)
        {
            // Toggle info panel
            ToggleInfoPanel();
        }
        
        void OnRightPrimaryButtonPressed(InputAction.CallbackContext context)
        {
            // Toggle building list
            ToggleBuildingList();
        }
        
        void OnLeftSecondaryButtonPressed(InputAction.CallbackContext context)
        {
            // Previous building
            NavigateToPreviousBuilding();
        }
        
        void OnRightSecondaryButtonPressed(InputAction.CallbackContext context)
        {
            // Next building
            NavigateToNextBuilding();
        }
        
        void OnLeftGripPressed(InputAction.CallbackContext context)
        {
            // Teleport to current building
            TeleportToCurrentBuilding();
        }
        
        void OnRightGripPressed(InputAction.CallbackContext context)
        {
            // Reset camera position
            ResetCameraPosition();
        }
        
        void ToggleInfoPanel()
        {
            if (infoPanel != null)
            {
                infoPanelVisible = !infoPanelVisible;
                if (infoPanelVisible)
                {
                    infoPanel.ShowPanel();
                    if (buildingListVisible)
                    {
                        buildingList.HideList();
                        buildingListVisible = false;
                    }
                }
                else
                {
                    infoPanel.HidePanel();
                }
            }
        }
        
        void ToggleBuildingList()
        {
            if (buildingList != null)
            {
                buildingListVisible = !buildingListVisible;
                if (buildingListVisible)
                {
                    buildingList.ShowList();
                    if (infoPanelVisible)
                    {
                        infoPanel.HidePanel();
                        infoPanelVisible = false;
                    }
                }
                else
                {
                    buildingList.HideList();
                }
            }
        }
        
        void NavigateToPreviousBuilding()
        {
            if (sceneManager != null)
            {
                int currentID = sceneManager.GetCurrentBuildingID();
                VRSceneManager.BuildingData[] buildings = sceneManager.GetAllBuildings();
                
                int previousIndex = (currentID - 2 + buildings.Length) % buildings.Length;
                sceneManager.SelectBuilding(buildings[previousIndex].id);
            }
        }
        
        void NavigateToNextBuilding()
        {
            if (sceneManager != null)
            {
                int currentID = sceneManager.GetCurrentBuildingID();
                VRSceneManager.BuildingData[] buildings = sceneManager.GetAllBuildings();
                
                int nextIndex = currentID % buildings.Length;
                sceneManager.SelectBuilding(buildings[nextIndex].id);
            }
        }
        
        void TeleportToCurrentBuilding()
        {
            if (sceneManager != null)
            {
                int currentID = sceneManager.GetCurrentBuildingID();
                sceneManager.TeleportToBuilding(currentID);
            }
        }
        
        void ResetCameraPosition()
        {
            if (Camera.main != null)
            {
                Camera.main.transform.position = new Vector3(0, 1.6f, 5f);
                Camera.main.transform.rotation = Quaternion.identity;
            }
        }
        
        void ScrollBuildingList(float direction)
        {
            if (buildingList != null && buildingList.scrollRect != null)
            {
                float currentValue = buildingList.scrollRect.verticalNormalizedPosition;
                float newValue = Mathf.Clamp01(currentValue + direction * 0.1f);
                buildingList.scrollRect.verticalNormalizedPosition = newValue;
            }
        }
        
        // Public methods for external access
        public void ShowInfoPanel()
        {
            if (infoPanel != null)
            {
                infoPanel.ShowPanel();
                infoPanelVisible = true;
            }
        }
        
        public void HideInfoPanel()
        {
            if (infoPanel != null)
            {
                infoPanel.HidePanel();
                infoPanelVisible = false;
            }
        }
        
        public void ShowBuildingList()
        {
            if (buildingList != null)
            {
                buildingList.ShowList();
                buildingListVisible = true;
            }
        }
        
        public void HideBuildingList()
        {
            if (buildingList != null)
            {
                buildingList.HideList();
                buildingListVisible = false;
            }
        }
    }
} 