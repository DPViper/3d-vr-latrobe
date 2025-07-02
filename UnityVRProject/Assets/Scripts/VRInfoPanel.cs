using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;

namespace LaTrobeVR
{
    public class VRInfoPanel : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject panelCanvas;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI codeText;
        public TextMeshProUGUI carParkText;
        public TextMeshProUGUI constructedText;
        public TextMeshProUGUI descriptionText;
        
        [Header("Interaction")]
        public XRSimpleInteractable toggleButton;
        public GameObject toggleButtonVisual;
        
        [Header("Positioning")]
        public Transform followTarget;
        public Vector3 offset = new Vector3(0, 0, -2f);
        public float followSpeed = 5f;
        
        private bool isVisible = false;
        private VRSceneManager.BuildingData currentBuilding;
        
        void Start()
        {
            SetupUI();
            SetupInteraction();
            HidePanel();
        }
        
        void SetupUI()
        {
            if (panelCanvas == null)
            {
                panelCanvas = transform.Find("Canvas")?.gameObject;
            }
            
            if (panelCanvas == null)
            {
                CreateUIPanel();
            }
        }
        
        void CreateUIPanel()
        {
            // Create canvas
            GameObject canvasObj = new GameObject("Canvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.zero;
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            panelCanvas = canvasObj;
            
            // Create panel background
            GameObject panelObj = new GameObject("Panel");
            panelObj.transform.SetParent(canvasObj.transform);
            panelObj.transform.localPosition = Vector3.zero;
            panelObj.transform.localScale = Vector3.one * 0.01f; // Scale down for VR
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.25f, 0.25f, 0.25f, 0.9f);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(800, 600);
            
            // Create title
            CreateTextElement("Title", panelObj, out titleText, new Vector2(0, 250), 48, TextAlignmentOptions.Center);
            
            // Create info table
            CreateTextElement("Code", panelObj, out codeText, new Vector2(-200, 150), 24, TextAlignmentOptions.Left);
            CreateTextElement("CarPark", panelObj, out carParkText, new Vector2(-200, 100), 24, TextAlignmentOptions.Left);
            CreateTextElement("Constructed", panelObj, out constructedText, new Vector2(-200, 50), 24, TextAlignmentOptions.Left);
            
            // Create description
            CreateTextElement("Description", panelObj, out descriptionText, new Vector2(0, -50), 20, TextAlignmentOptions.Left);
            
            // Create toggle button
            CreateToggleButton(panelObj);
        }
        
        void CreateTextElement(string name, GameObject parent, out TextMeshProUGUI textComponent, Vector2 position, float fontSize, TextAlignmentOptions alignment)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent.transform);
            textObj.transform.localPosition = new Vector3(position.x, position.y, 0);
            
            textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = fontSize;
            textComponent.color = Color.white;
            textComponent.alignment = alignment;
            textComponent.text = "";
            
            RectTransform textRect = textComponent.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(400, 50);
        }
        
        void CreateToggleButton(GameObject parent)
        {
            GameObject buttonObj = new GameObject("ToggleButton");
            buttonObj.transform.SetParent(parent.transform);
            buttonObj.transform.localPosition = new Vector3(350, 250, 0);
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);
            
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(50, 50);
            
            toggleButton = buttonObj.AddComponent<XRSimpleInteractable>();
            toggleButtonVisual = buttonObj;
        }
        
        void SetupInteraction()
        {
            if (toggleButton != null)
            {
                toggleButton.selectEntered.AddListener(OnToggleButtonPressed);
            }
        }
        
        void OnToggleButtonPressed(SelectEnterEventArgs args)
        {
            TogglePanel();
        }
        
        void Update()
        {
            // Follow camera or target
            if (followTarget != null)
            {
                Vector3 targetPosition = followTarget.position + followTarget.forward * offset.z + Vector3.up * offset.y;
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
                transform.LookAt(followTarget);
            }
            else if (Camera.main != null)
            {
                Vector3 targetPosition = Camera.main.transform.position + Camera.main.transform.forward * offset.z + Vector3.up * offset.y;
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
                transform.LookAt(Camera.main.transform);
            }
        }
        
        public void ShowBuildingInfo(VRSceneManager.BuildingData building)
        {
            currentBuilding = building;
            
            if (titleText != null)
                titleText.text = building.name;
            
            if (codeText != null)
                codeText.text = $"Building Code: {building.code}";
            
            if (carParkText != null)
                carParkText.text = $"Closest Car Park: {building.closestCarPark}";
            
            if (constructedText != null)
                constructedText.text = $"Constructed: {building.constructed}";
            
            if (descriptionText != null)
                descriptionText.text = building.description;
            
            ShowPanel();
        }
        
        public void ShowPanel()
        {
            isVisible = true;
            if (panelCanvas != null)
            {
                panelCanvas.SetActive(true);
            }
        }
        
        public void HidePanel()
        {
            isVisible = false;
            if (panelCanvas != null)
            {
                panelCanvas.SetActive(false);
            }
        }
        
        public void TogglePanel()
        {
            if (isVisible)
                HidePanel();
            else
                ShowPanel();
        }
        
        public bool IsVisible()
        {
            return isVisible;
        }
    }
} 