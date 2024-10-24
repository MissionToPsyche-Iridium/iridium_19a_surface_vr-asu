using System;
using System.Collections;
using System.Collections.Generic;
using TerrainEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class LoadingBar : Menu
    {
        public static ToggleDelegate OpenMenu { get; private set; }
        /// <summary>
        /// Loading Bar delegate handles all calls and updated to the loading bar menu
        /// </summary>
        public delegate void LoadingBarDelegate(float value = 0f, string text = "");
        public static LoadingBarDelegate Loading { get; private set; }
        public static LoadingBarDelegate DoneLoading { get; private set; }

        public static bool Abort;

        public Slider loadingBar;
        public Button exitButton;
        public TMP_Text loadingText;
        public TMP_Text loadingPercent;

        void Awake()
        {
            OpenMenu = ToggleMenu;
            loadingBar.value = 0f;
            Loading = UpdateValue;
            DoneLoading = CloseMenu;
            
            exitButton.onClick.AddListener(delegate
            {
                Abort = true;
                SceneDownloader.singleton.StopAllCoroutines();
                StartCoroutine(SceneDownloader.singleton.ChangeState(SceneDownloader.SceneSession.DONE));

                CloseMenu(0f, ""); //resets loading bar to zero
                PreviousMenu.ToggleMenu(true);
            });
        }

        public override void ToggleMenu(bool active)
        {
            Abort = false; //set abort to false everytime the menu opens
            parentObject.SetActive(active);
        }

        private void UpdateValue(float value, string text)
        {
            loadingBar.value += value;
            loadingPercent.text = Math.Round(loadingBar.value, 2) * 100 + "%";
            loadingText.text = text;
        }

        private void CloseMenu(float value, string text)
        {
            parentObject.SetActive(false);
            loadingBar.value = value;

            loadingPercent.text = value + "%";
            loadingText.text = text;
        }
    }

}