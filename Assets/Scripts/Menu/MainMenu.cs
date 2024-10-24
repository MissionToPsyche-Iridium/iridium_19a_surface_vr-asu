using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

namespace UserInterface
{
    public class MainMenu : Menu
    {
        public static ToggleDelegate OpenMenu { get; private set; }
        public static ToggleDelegate OpenPrimaryMenus { get; private set; }
        
        public GameObject PrimaryMenus;
        public Button sampleTerrainButton;
        public Button customTerrainButton;
        public Button multiuserButton;
        public Button settingButton;
        public Button exitToGameButton;

        void Awake()
        {
            //setting delegates
            OpenMenu = ToggleMenu;
            OpenPrimaryMenus = TogglePrimaryMenu;
            
            //close all menus, start on main menu
            CloseAllMenus();   
            ToggleMenu(true);
            PreviousMenu = this;
            
            //button listeners
            sampleTerrainButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                SampleTerrainsMenu.OpenMenu.Invoke(true);
            });
            
            customTerrainButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                PreviousMenu = this;
                Login.TryLogin.Invoke();
            });
            
            multiuserButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                MultiuserMenu.OpenMenu(true);
            });
            
            settingButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                SettingsController.OpenMenu(true);
            });
            
            exitToGameButton.onClick.AddListener(delegate
            {
                PreviousMenu = this;
                ToggleMenu(false);
                OpenPrimaryMenus(false);
                GameState.InTerrain = true;
            });

        }
        public override void ToggleMenu(bool active)
        {
            multiuserButton.gameObject.SetActive(!GameState.InMultiuser);
            parentObject.SetActive(active);
            GameState.InTerrain = false;
        }

        private void CloseAllMenus()
        {
            for (int i = 0; i < PrimaryMenus.transform.childCount; i++)
            {
                PrimaryMenus.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void TogglePrimaryMenu(bool active)
        {
            PrimaryMenus.SetActive(active);
            TerrainTools.OpenMenu.Invoke(!active);
            if(!GameState.IsVR) LockCursor.Invoke(!active);
        }
    }
}
