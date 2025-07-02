using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;

namespace LaTrobeVR
{
    public class VRBuildingList : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject listCanvas;
        public GameObject listItemPrefab;
        public Transform listContent;
        public ScrollRect scrollRect;
        
        [Header("Interaction")]
        public XRSimpleInteractable toggleButton;
        public GameObject toggleButtonVisual;
        
        [Header("Positioning")]
        public Transform followTarget;
        public Vector3 offset = new Vector3(0, 0, -1.5f);
        public float followSpeed = 5f;
        
        private bool isVisible = false;
        private List<VRBuildingListItem> listItems = new List<VRBuildingListItem>();
        private VRSceneManager sceneManager;
        
        void Start()
        {
            SetupUI();
            SetupInteraction();
            HideList();
        }
        
        void SetupUI()
        {
            if (listCanvas == null)
            {
                CreateListUI();
            }
        }
        
        void CreateListUI()
        {
            // Create canvas
            GameObject canvasObj = new GameObject("ListCanvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = Vector3.zero;
            
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
            
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            listCanvas = canvasObj;
            
            // Create main panel
            GameObject panelObj = new GameObject("ListPanel");
            panelObj.transform.SetParent(canvasObj.transform);
            panelObj.transform.localPosition = Vector3.zero;
            panelObj.transform.localScale = Vector3.one * 0.01f;
            
            Image panelImage = panelObj.AddComponent<Image>();
            panelImage.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
            
            RectTransform panelRect = panelObj.GetComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(600, 800);
            
            // Create title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(panelObj.transform);
            titleObj.transform.localPosition = new Vector3(0, 350, 0);
            
            TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
            titleText.text = "Select Building";
            titleText.fontSize = 36;
            titleText.color = Color.white;
            titleText.alignment = TextAlignmentOptions.Center;
            
            RectTransform titleRect = titleText.GetComponent<RectTransform>();
            titleRect.sizeDelta = new Vector2(500, 60);
            
            // Create scroll view
            CreateScrollView(panelObj);
            
            // Create toggle button
            CreateToggleButton(panelObj);
        }
        
        void CreateScrollView(GameObject parent)
        {
            // Create scroll view container
            GameObject scrollViewObj = new GameObject("ScrollView");
            scrollViewObj.transform.SetParent(parent.transform);
            scrollViewObj.transform.localPosition = new Vector3(0, 200, 0);
            
            RectTransform scrollViewRect = scrollViewObj.AddComponent<RectTransform>();
            scrollViewRect.sizeDelta = new Vector2(550, 500);
            
            // Add scroll rect
            scrollRect = scrollViewObj.AddComponent<ScrollRect>();
            
            // Create viewport
            GameObject viewportObj = new GameObject("Viewport");
            viewportObj.transform.SetParent(scrollViewObj.transform);
            viewportObj.transform.localPosition = Vector3.zero;
            
            RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
            viewportRect.sizeDelta = new Vector2(550, 500);
            
            Image viewportImage = viewportObj.AddComponent<Image>();
            viewportImage.color = new Color(0.15f, 0.15f, 0.15f, 0.8f);
            
            Mask viewportMask = viewportObj.AddComponent<Mask>();
            viewportMask.showMaskGraphic = false;
            
            // Create content
            GameObject contentObj = new GameObject("Content");
            contentObj.transform.SetParent(viewportObj.transform);
            contentObj.transform.localPosition = Vector3.zero;
            
            RectTransform contentRect = contentObj.AddComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(530, 500);
            
            VerticalLayoutGroup layoutGroup = contentObj.AddComponent<VerticalLayoutGroup>();
            layoutGroup.spacing = 10;
            layoutGroup.padding = new RectOffset(10, 10, 10, 10);
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = false;
            layoutGroup.childForceExpandHeight = false;
            
            ContentSizeFitter sizeFitter = contentObj.AddComponent<ContentSizeFitter>();
            sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            
            listContent = contentObj.transform;
            
            // Setup scroll rect
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.vertical = true;
            scrollRect.horizontal = false;
        }
        
        void CreateToggleButton(GameObject parent)
        {
            GameObject buttonObj = new GameObject("ToggleButton");
            buttonObj.transform.SetParent(parent.transform);
            buttonObj.transform.localPosition = new Vector3(250, 350, 0);
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            
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
            ToggleList();
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
        
        public void Initialize(VRSceneManager manager)
        {
            sceneManager = manager;
            PopulateList();
        }
        
        void PopulateList()
        {
            if (sceneManager == null) return;
            
            // Clear existing items
            foreach (var item in listItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            listItems.Clear();
            
            // Create new items
            VRSceneManager.BuildingData[] buildings = sceneManager.GetAllBuildings();
            foreach (var building in buildings)
            {
                CreateListItem(building);
            }
        }
        
        void CreateListItem(VRSceneManager.BuildingData building)
        {
            GameObject itemObj = new GameObject($"ListItem_{building.id}");
            itemObj.transform.SetParent(listContent);
            
            RectTransform itemRect = itemObj.AddComponent<RectTransform>();
            itemRect.sizeDelta = new Vector2(500, 80);
            
            Image itemImage = itemObj.AddComponent<Image>();
            itemImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            
            // Add interaction
            XRSimpleInteractable itemInteractable = itemObj.AddComponent<XRSimpleInteractable>();
            
            // Create text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(itemObj.transform);
            textObj.transform.localPosition = new Vector3(0, 0, 0);
            
            TextMeshProUGUI itemText = textObj.AddComponent<TextMeshProUGUI>();
            itemText.text = building.name;
            itemText.fontSize = 24;
            itemText.color = Color.white;
            itemText.alignment = TextAlignmentOptions.Left;
            
            RectTransform textRect = itemText.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(480, 60);
            
            // Create component
            VRBuildingListItem listItem = itemObj.AddComponent<VRBuildingListItem>();
            listItem.Initialize(building, sceneManager, itemInteractable);
            
            listItems.Add(listItem);
        }
        
        public void ShowList()
        {
            isVisible = true;
            if (listCanvas != null)
            {
                listCanvas.SetActive(true);
            }
        }
        
        public void HideList()
        {
            isVisible = false;
            if (listCanvas != null)
            {
                listCanvas.SetActive(false);
            }
        }
        
        public void ToggleList()
        {
            if (isVisible)
                HideList();
            else
                ShowList();
        }
        
        public bool IsVisible()
        {
            return isVisible;
        }
    }
    
    public class VRBuildingListItem : MonoBehaviour
    {
        private VRSceneManager.BuildingData buildingData;
        private VRSceneManager sceneManager;
        private XRSimpleInteractable interactable;
        private Image backgroundImage;
        
        public void Initialize(VRSceneManager.BuildingData data, VRSceneManager manager, XRSimpleInteractable itemInteractable)
        {
            buildingData = data;
            sceneManager = manager;
            interactable = itemInteractable;
            backgroundImage = GetComponent<Image>();
            
            interactable.selectEntered.AddListener(OnItemSelected);
            interactable.hoverEntered.AddListener(OnItemHovered);
            interactable.hoverExited.AddListener(OnItemUnhovered);
        }
        
        void OnItemSelected(SelectEnterEventArgs args)
        {
            if (sceneManager != null)
            {
                sceneManager.SelectBuilding(buildingData.id);
            }
        }
        
        void OnItemHovered(HoverEnterEventArgs args)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            }
        }
        
        void OnItemUnhovered(HoverExitEventArgs args)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            }
        }
    }
} 