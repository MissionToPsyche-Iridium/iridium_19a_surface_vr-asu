using System;
using System.Collections;
using System.Collections.Generic;
using Multiuser;
using TerrainEngine;
using TMPro;
using Unity.Netcode;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using XRControls;
using Button = UnityEngine.UI.Button;

namespace UserInterface
{
    public class TerrainTools : Menu
    {
        public static ToggleDelegate OpenMenu { get; private set; }
        public static ToggleDelegate SetClientUI { get; private set; }
        public static MenuDelegate SetRoomCode { get; private set; }
        public static MenuDelegate TabTip { get; private set; }
        public static TextDelegate DynamicReadout { get; private set; }

        [Header("3D Object")]
        public GameObject terrainTools; // refers to the GameObject that is attached to the podium
        
        [Header("Panels")] //public GameObject terrainToolbar;
        public GameObject terrainLayers;
        public GameObject perPixelPanel;
        public GameObject leftSideTools;
        
        [Header("Text Assets")]
        public TMP_Text PerPixelHeader;
        public TMP_Text PerPixelData;
        public TMP_Text roomCodeText;
        public TMP_Text generalTip;
        public GameObject tabTip;
        
        [Header("Buttons")]
        public Button perPixelButton;
        public Button multiplayerButton;
        public Button scaleBarButton;
        public Button layersButton;
        public Button terrainMenuButton;
        public Button resetPosition;
        public Button clearAllPins;
        public Button clearMyPins;
        
        [Header("Podium Buttons")] // buttons added for podium UI
        public Button perPixelButton2;
        public Button multiplayerButton2;
        public Button scalebarButton2;
        public Button layersButton2;
        public Button terrainMenuButton2;

        [SerializeField] private XRController _xrController; // Reference to the XR controller that controls player movement

        void Awake()
        {
            OpenMenu = ToggleMenu;
            SetClientUI = SetClientToolbar;
            SetRoomCode = RoomCodeUI;
            DynamicReadout = ReadData;
            TabTip = ToggleTabTip;

            // instantiating button listeners
            terrainMenuButton.onClick.AddListener(delegate
            {
                ToggleTerrainsPanel(!PreviousMenu.parentObject.activeSelf);
            });
            perPixelButton.onClick.AddListener(delegate
            {
                TogglePerPixelData(!perPixelPanel.activeSelf);
                ToggleScaleBar(false);
            });
            layersButton.onClick.AddListener(delegate
            {
                ToggleLayersPanel(!terrainLayers.activeSelf);
            });
            multiplayerButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                MainMenu.OpenPrimaryMenus(true);
                MultiuserMenu.OpenMenu.Invoke(true);
                generalTip.text = "";
            });
            scaleBarButton.onClick.AddListener(delegate
            {
                ScaleBarPrefabs.singleton.ToggleScalebarMode();
                TogglePerPixelData(false);
            });
            resetPosition.onClick.AddListener(delegate
            {
                SceneMaterializer.singleton.terrain.transform.position = SceneMaterializer.singleton.terrainStartingPosition;
            });
            
            
            // new button listeners
            terrainMenuButton2.onClick.AddListener(delegate
            {
                ToggleTerrainsPanel(!PreviousMenu.parentObject.activeSelf);
            });
            perPixelButton2.onClick.AddListener(delegate
            {
                TogglePerPixelData(!perPixelPanel.activeSelf);
                ToggleScaleBar(false);
            });
            layersButton2.onClick.AddListener(delegate
            {
                ToggleLayersPanel(!terrainLayers.activeSelf);
            });
            multiplayerButton2.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                MainMenu.OpenPrimaryMenus(true);
                MultiuserMenu.OpenMenu.Invoke(true);
            });
            scalebarButton2.onClick.AddListener(delegate
            {
                ScaleBarPrefabs.singleton.ToggleScalebarMode();
                TogglePerPixelData(false);
            });
        }

        new void Start()
        {
            base.Start();
            tabTip.SetActive(!GameState.IsVR);
        }

        void Update()
        {
            resetPosition.gameObject.SetActive(SceneMaterializer.singleton.terrain.transform.position !=
                                               SceneMaterializer.singleton.terrainStartingPosition);
            
            /*
            // VR controller buttons
            if (GameState.InTerrain && _xrController.aActive && PreviousMenu) // toggling terrains menu
            {
                ToggleTerrainsPanel(!PreviousMenu.parentObject.activeSelf);
                _xrController.aActive = false;
            }

            if (GameState.InTerrain && _xrController.bActive && PreviousMenu) // toggling terrains menu
            {
                ToggleLayersPanel(!terrainLayers.activeSelf);
                _xrController.bActive = false;
            }
            */
            
        }

        public void ShowTerrainTools()
        {
            terrainTools.SetActive(true);
            PlayEnterAnimation();
        }

        public void HideTerrainTools()
        {
            terrainTools.SetActive(false);
        }
        
        private void PlayEnterAnimation()
        {
            // do later
        }

        public override void ToggleMenu(bool active)
        {
            parentObject.SetActive(active);
        }

        private void TogglePerPixelData(bool active)
        {
            clearAllPins.gameObject.SetActive(GameState.InMultiuser && NetworkManager.Singleton.IsHost);
            PerPixelDataReader.singleton.readingData = active;
            perPixelPanel.SetActive(active);
        }

        private void ToggleLayersPanel(bool active)
        {
            terrainLayers.SetActive(active);
        }

        private void ToggleTerrainsPanel(bool active) // !!! need to work out flow of toggling terrains panel on/off
        {
            MainMenu.OpenPrimaryMenus(active);
            //close other panels if terrains menu is open
            TogglePerPixelData(false);
            ToggleLayersPanel(false);
            ToggleScaleBar(false);
            generalTip.text = "";
            
            if (PreviousMenu.GetType() != typeof(MultiuserMenu))
            {
                PreviousMenu.ToggleMenu(active);
            }
            else
            {
                MainMenu.OpenMenu(true);
            }
        }

        private void ToggleScaleBar(bool active)
        {
            ScaleBarPrefabs.singleton.scalebarMode = active;
        }

        private void ToggleTabTip()
        {
            tabTip.SetActive(false);
        }

        private void ReadData(string header, string data)
        {
            PerPixelHeader.text = header;
            PerPixelData.text = data;
        }

        private void SetClientToolbar(bool active)
        {
            clearAllPins.gameObject.SetActive(!active);
            terrainMenuButton.gameObject.SetActive(!active);
            resetPosition.gameObject.SetActive(!active);
        }

        private void RoomCodeUI()
        {
            if (GameState.InMultiuser)
            {
                roomCodeText.text = "Room Code: " + MultiplayerManager.Instance.joinCode + " ";
            }
            else
            {
                roomCodeText.text = string.Empty;
            }
        }
    }

}