using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using TerrainEngine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UserInterface
{
    public class Login : Menu
    {
        public delegate void LoginDelegate(UnityWebRequest.Result result);
        public static LoginDelegate FailedLogin { get; private set; }
        public static MenuDelegate TryLogin { get; private set; }
        
        public TMP_InputField usernameField;
        public TMP_InputField passwordField;
        public Button loginButton;
        public Button signInBackButton;
        public TMP_Text notificationText;

        [HideInInspector] public static string username, password;
        private bool canTab = false, canEnter = false;

        void Awake()
        {
            TryLogin = LogIn;
            FailedLogin = UserLoginFailed;

            loginButton.onClick.AddListener(GetLoginInfo);
        }

        new void Start()
        {
            base.Start();
            username = ""; // doesn't save any data after closing app
            
            signInBackButton.onClick.AddListener(delegate
            {
                ToggleMenu(false);
                MainMenu.OpenMenu.Invoke(true);
                if(GameState.IsVR) vrKeyboard.gameObject.SetActive(false);
            });

            usernameField.onSelect.AddListener(delegate(string arg0)
            {
                canTab = true;
                
                if (GameState.IsVR)
                {
                    vrKeyboard.gameObject.SetActive(true);
                    vrKeyboard.inputField = usernameField;
                }
            });
            
            usernameField.onDeselect.AddListener(delegate(string arg0)
            {
                canTab = false;
            });
            
            passwordField.onSelect.AddListener(delegate(string arg0)
            {
                canEnter = true;
                
                if (GameState.IsVR)
                {
                    vrKeyboard.gameObject.SetActive(true);
                    vrKeyboard.inputField = passwordField;
                }
            });

            passwordField.onDeselect.AddListener(delegate(string arg0)
            {
                canEnter = false;
            });
        }

        private void Update()
        {
            if (PreviousMenu.GetType() == typeof(MainMenu))
            {
                if (canTab && Input.GetKeyDown(KeyCode.Tab))
                {
                    passwordField.Select();
                }

                if (canEnter && Input.GetKeyDown(KeyCode.Return))
                {
                    GetLoginInfo();
                }
            }
        }

        public override void ToggleMenu(bool active)
        {
            notificationText.text = "";
            if (username != "") usernameField.text = username;
            parentObject.SetActive(active);
        }

        private void LogIn()
        {
            ToggleMenu(true);
            TerrainMenu.OpenMenu.Invoke(false);
        }

        private void GetLoginInfo()
        {
            if (usernameField.text == "" || passwordField.text == "")
            {
                notificationText.text = "Incorrect username or password.";
            }
            else
            {
                ToggleMenu(false);
                LoadingBar.OpenMenu(true);
                PreviousMenu = this;
                StartCoroutine(SceneDownloader.singleton.DownloadViewList(usernameField.text, passwordField.text));
                username = usernameField.text;
                password = passwordField.text;
                usernameField.text = "";
                passwordField.text = "";

                if (GameState.IsVR) vrKeyboard.gameObject.SetActive(false);
            }
        }

        private void UserLoginFailed(UnityWebRequest.Result result)
        {
            LoadingBar.DoneLoading();
            ToggleMenu(true);
            if (result == UnityWebRequest.Result.ProtocolError)
            {
                notificationText.text = "Incorrect username or password.";
            }
            else if (result == UnityWebRequest.Result.ConnectionError)
            {
                notificationText.text = "Connection error. Check internet connection before trying again";
            }
            
        }

    }
}