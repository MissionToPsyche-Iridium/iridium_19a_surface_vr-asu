using System.Collections;
using System.Collections.Generic;
using TerrainEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserInterface;

namespace UserInterface
{
    public class SampleTerrainsMenu : Menu
    {
        public static ToggleDelegate OpenMenu { get; private set; }
        public List<Button> terrainButtons;
        public Button exitToMenuButton;
        public Button exitToGameButton;
        public bool loadedSampleTerrain;

        void Awake()
        {
            OpenMenu = ToggleMenu;

            // instantiating button listeners
            foreach (Button button in terrainButtons)
            {
                button.onClick.AddListener(delegate { LoadTerrain(button.GetComponent<DataPackBehaviour>()); });
            }

            exitToMenuButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                MainMenu.OpenMenu(true);
            });
            
            exitToGameButton.onClick.AddListener(delegate
            {
                PreviousMenu = this;
                ToggleMenu(false);
                MainMenu.OpenPrimaryMenus(false);
                GameState.InTerrain = true;
            });
            
            //exitToGameButton.gameObject.SetActive(false); // button stays off until a terrain is chosen. disabled for now so user always has access to it
        }

        public override void ToggleMenu(bool active)
        {
            parentObject.SetActive(active);
        }

        private void LoadTerrain(DataPackBehaviour datapack)
        {
            PreviousMenu = this;
            datapack.LoadData();
            ToggleMenu(false);
            MainMenu.OpenPrimaryMenus(false);
            
            loadedSampleTerrain = true;
            //exitToGameButton.gameObject.SetActive(true);
        }
    }
}