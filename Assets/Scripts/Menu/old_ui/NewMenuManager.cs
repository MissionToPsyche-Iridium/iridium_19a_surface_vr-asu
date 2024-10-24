using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TerrainEngine;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using UserInterface;
using XRControls;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Multiuser;
using System.ComponentModel.Design;
using Unity.Netcode;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Unity.XR.CoreUtils;


public class NewMenuManager : MonoBehaviour
{

    //public static NewMenuManager singleton;

    
    [SerializeField] public NewUIScript newUIScript;
    [SerializeField] private GameObject EventSystem;
    [SerializeField] private FirstPersonController _firstPersonController;
    public GameObject _localPlayer;

    [Header ("Menu Assets")]
    [SerializeField] private GameObject primaryMenus;
    // ... is the parent of ..
    [SerializeField] private MainMenuAssets _mainMenuAssets;
    
    //Public because we need to be able to remove button listeners only if a log-in is successful
    public InputFieldMenuAssets _inputFieldMenuAssets; 
    
    [SerializeField] private SampleTerrainAssets _sampleTerrainAssets;
    [SerializeField] private TwoButtonMenuAssets _twoButtonMenuAssets;
    [SerializeField] private PauseMenuAssets _pauseMenuAssets;
    public TerrainToolbar _TerrainToolbar;

    [Header ("Menu Panels")]
    [SerializeField] private GameObject layersMenu;
    [SerializeField] public GameObject loadingMenu;
    [SerializeField] private GameObject customTerrainsMenu;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private GameObject errorMenu;

    [Header ("Buttons")]
    [SerializeField] private Button refreshButton;
    [SerializeField] private KeyboardController _keyboardController;
    [SerializeField] private Button ejectDownloadButton;
    public Button exitButton;
    
    public GameObject clearAllPinsButton; //Only visible in multiplayer as a host
    public GameObject clearMyPinsButton;

    [Header ("Booleans")]
    public bool inTerrain = false;
    public bool inMultiplayer = false;
    public bool isSampleTerrain;
    public bool abort;
    public bool downloading = false;
    public string previouslyOpenedMenu;
    public Canvas newCanvas;
    public bool mouseLockedBeforePaused; //Mouse starts out as free in desktop

    public Button resetPosition; // use? when terrain moved, button appears. add onlistener

    //public GameObject jezero;

    #region MONO

    public void Start()
    {
        ClosePrimaryMenus();
        StartFromMainMenu();

        //The user is either in VR or utilizing VRDebug Mode
        if (GameState.IsVR || GameState.vrDebug)
        {
            //print("VR User.");
            //ActivateVRKeyboards();
            ToggleCursorMode(true);
        }
        else //The user is in desktop mode
        {
            //print("Desktop User. Locking canvas to screen");
            LockCanvasToScreen();
            ToggleCursorMode(false);
        }

        //jezero.GetComponent<DataPackBehaviour>().LoadData(); // load data
        //print("jezero loaded");

        //_TerrainToolbar.terrainTools.SetActive(true);

        //ButtonListeners();
        _inputFieldMenuAssets.Start();
        _twoButtonMenuAssets.Start();

        //menu changes for whenever there is a scene change
        SceneDownloader.singleton.stateChanged.AddListener(delegate
        {
            //downloading a terrain
            if (SceneDownloader.currentState == SceneDownloader.SceneSession.DOWNLOADING)
            {
                primaryMenus.SetActive(true);
                loadingMenu.SetActive(true);
            }
            if (SceneDownloader.currentState == SceneDownloader.SceneSession.READY)
            {
                EnterTerrain();
            }
        });

        resetPosition.onClick.AddListener(delegate
        {
            _TerrainToolbar.ResetTerrainPosition();
        });

        if(GameState.IsVR)
        {
            _localPlayer = GameObject.Find("XRRig");
        }
        else
        {
            _localPlayer = GameObject.Find("DesktopRig");
        }

        
    }
    
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        /*if (singleton != null && singleton != this)
        {
            Destroy(this);
        }
        else
        {
            singleton = this;
        }*/
        
       
    }

    private void Update()
    {
        /*            if (_TerrainToolbar.PerPixelPanel.activeSelf)
                    {
                        //_TerrainToolbar.PerPixelFloat.text = (PerPixelDataReader.singleton.data);
                    }*/

        // if terrain is moved, make reset position button appear for user to click

        //print("MULTIUSER: " +GameState.singleton.inMultiuser);

        if(GameState.InMultiuser)
        {
            //_mainMenuAssets.multiuserButton.gameObject.SetActive(false);
            //print("IN MULTIPLAYER");

            if(NetworkManager.Singleton.IsClient)
            {
                // turn off resetPosition button
                resetPosition.gameObject.SetActive(false);
            }
            else
            {
                // is host
                if (SceneMaterializer.singleton.terrain.transform.position != SceneMaterializer.singleton.terrainStartingPosition)
                {
                    resetPosition.gameObject.SetActive(true);
                }
                else
                {
                    resetPosition.gameObject.SetActive(false);

                }
            }
        }
        else
        {
            //_mainMenuAssets.multiuserButton.gameObject.SetActive(true);
            //print("NOT IN MULTIPLAYER");
        }

        

        //Pause(); //pause menu code 
        // disabled Pause menu for user studies

        //Toggle Terrain Tools
        if (Input.GetKeyDown(KeyCode.Tab) && (PauseMenu.activeSelf == false) && (primaryMenus.activeSelf == false))
        {
            //If the user is already in a menu, unlock the cursor
            if (!_firstPersonController.cursorLocked)
            {
                _TerrainToolbar.tabTip.SetActive(false); //Once user presses tab for the first time, remove tip
                //print("Mouse Lock Mode Off");
                ToggleCursorMode(false);
                mouseLockedBeforePaused = !mouseLockedBeforePaused;
            }
            else // If the user is trying to open a menu, lock the cursor for them.
            {
                //print("Mouse Lock Mode On");
                ToggleCursorMode(true);
                mouseLockedBeforePaused = !mouseLockedBeforePaused;
            }
        }

        // turn off per pixel feature if Per Pixel Panel is turned off
        if (!_TerrainToolbar.PerPixelPanel.activeInHierarchy)
        {
            PerPixelDataReader.singleton.readingData = false;
        }

        // turn off reset position button 
    }

    #endregion

    public void Pause()
    {
        //if you press escape and the pause menu isn't open, and the user is in the terrain
        if (Input.GetKeyDown(KeyCode.Escape) && !PauseMenu.activeSelf && inTerrain)
        {
            // Pause Menu 
            //print("Pause");
            
            //Open up primary menus and allow for mouse movement
            primaryMenus.SetActive(true);
            ToggleCursorMode(false); //Mouse unlocks when entering pause menu

            _pauseMenuAssets.pauseMenu.SetActive(true);
            _pauseMenuAssets.header.text = "Pause Menu";
            _pauseMenuAssets.topButton.GetComponentInChildren<TMP_Text>().text = "Leave Multiuser";
            _pauseMenuAssets.bottomButton.GetComponentInChildren<TMP_Text>().text = "Exit Game";
            _pauseMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back to Game";

            if (GameState.InMultiuser)
            {
                // Shows both buttons for multiuser
                _pauseMenuAssets.topButton.gameObject.SetActive(true);
            }
            else
            {
                // Shows only 1 button in single-player
                _pauseMenuAssets.topButton.gameObject.SetActive(false);
            }

            // ADD FUNCTIONALITY TO BUTTONS
            _pauseMenuAssets.topButton.onClick.AddListener(delegate {
                if (NetworkManager.Singleton.IsClient)
                {
                    ResetUI();
                }
                
                NetworkManager.Singleton.Shutdown();
                StartFromMainMenu();
                GameState.InMultiuser = false;
            });
            //back to game
            _pauseMenuAssets.backButton.onClick.AddListener(delegate
            {
                EnterTerrain();
            });
            // Quit Application
            _pauseMenuAssets.bottomButton.onClick.AddListener(delegate
            {
                // Quit in Unity Editor
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
                // Quit in Builds
                Application.Quit();
            });
        }
        
        //If the pause menu is ALREADY opened, close the menu and bring the player back to the game=
        else if (Input.GetKeyDown(KeyCode.Escape) && PauseMenu.activeSelf && inTerrain)
        {
            //print("Unpause");
            primaryMenus.SetActive(false);
            //Turn off mouse visibility and lock the cursor.
            ToggleCursorMode(mouseLockedBeforePaused);
            _pauseMenuAssets.pauseMenu.SetActive(false);


        }
    }
    
    //Controls the terrain button on the terrain toolbar (it looks like a mountain)
    public void TerrainButtonListener()
    {
        //if the menu is already openned, just close the terrains menu. The user wants to go back to the terrain
        if (inTerrain && primaryMenus.activeSelf)
        {
            ClosePrimaryMenus();
            //print("Closing primary menus and returning to terrain");
            primaryMenus.SetActive(false);
            ToggleCursorMode(mouseLockedBeforePaused);

            //Re-activate buttons
            _TerrainToolbar.multiplayerButton.interactable = true;
            //_TerrainToolbar.activatedLayersButton.interactable = true;
            _TerrainToolbar.layersButton.interactable = true;
            _TerrainToolbar.perPixelButton.interactable = true;
            return;
        }
        
        
        
        //Deactivate buttons in terrain toolbar while user is navigating menus
        //Only the terrin button is active so that the user can go back to their terrain
        _TerrainToolbar.PerPixelPanel.SetActive(false);

        _TerrainToolbar.multiplayerButton.interactable = false;
        //_TerrainToolbar.activatedLayersButton.interactable = false;
        _TerrainToolbar.layersButton.interactable = false;
        _TerrainToolbar.perPixelButton.interactable = false;

        if (isSampleTerrain)
        {
            SampleTerrainToMenu();
        }
        else
        {
            CustomTerrainToMenu();
        }
    }
    
    
    public void AbortDownload()
    {
        abort = true;
        NewUIScript.singleton.loadingBar.value = 0f;
        //print("Aborting download");
        StopAllCoroutines();
        SceneDownloader.singleton.StopAllCoroutines();
        NewUIScript.singleton.StopAllCoroutines();
        ClosePrimaryMenus();
        
        //note: can simplify this by always setting the previous menu -- can be useful for other flows
        switch (previouslyOpenedMenu)
        {
            case "SinglePlayerCustomTerrainLogIn":
                SceneDownloader.singleton.StopAllCoroutines();
                SceneDownloader.singleton.userScenes.Clear();
                SinglePlayerCustomTerrainLogIn();
                break;
            case "MultiPlayerCustomTerrainLogIn":
                SceneDownloader.singleton.StopAllCoroutines();
                SceneDownloader.singleton.userScenes.Clear();
                MultiPlayerCustomTerrainLogIn();
                break;
            case "CustomTerrains":
                customTerrainsMenu.SetActive(true);
                SceneDownloader.singleton.StopAllCoroutines();
                StartCoroutine(SceneDownloader.singleton.ChangeState(SceneDownloader.SceneSession.DONE));
                break;
            case "CreateRoom":
                CreateRoom();
                break;
            case "JoinRoom":
                JoinRoom();
                break;
            default:
                StartFromMainMenu();
                print("Unaccounted for download eject!!");
                break;
        }
    }
    
    
    public void TogglePerPixelData()
    {
        layersMenu.SetActive(false);
        //print("Toggling per pixel");
        if (_TerrainToolbar.PerPixelPanel.activeSelf == true) //Turn off per pixel data reading
        {
            PerPixelDataReader.singleton.readingData = false;
            _TerrainToolbar.PerPixelPanel.SetActive(false);
        }
        else 
        {
            PerPixelDataReader.singleton.readingData = true;
            _TerrainToolbar.PerPixelPanel.SetActive(true);
        }

    }

    public void TurnOffPerPixelData()
    {
        PerPixelDataReader.singleton.readingData = false;
        _TerrainToolbar.PerPixelPanel.SetActive(false);

    }

    /// <summary>
    /// Toggles Cursor mode depending on if the menu is open or not
    /// </summary>
    public void ToggleCursorMode(bool locked)
    {
        //If menu is open, the cursor is LOCKED, the user is in-game
        //and able to freely move the player.
        if (locked)
        {
            //print("Locking Mouse + Making Invisible.");
            // Lock mouse so user can explore scene / terrain
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            //_firstPersonController.cursorLocked = false;

        }
        //If menu is open, the cursor is UNLOCKED.
        //If the cursor is unlocked, the user should be able to see the cursor
        //and be able to utilize menus freely.
        else
        {
            // Unlock mouse so user can interact with menu
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            //_firstPersonController.cursorLocked = true;
        }
    }
    
    public void LockCanvasToScreen()
    {
        if (!GameState.vrDebug)
        {
            //print("Locking canvas to screen");
            newCanvas.renderMode = RenderMode.ScreenSpaceOverlay; // changes canvas to screen space 
            
        }
    }

    private void ActivateVRKeyboards()
    {
        
        _inputFieldMenuAssets.upperField.onSelect.AddListener(delegate
        {
            _keyboardController.OpenVRKeyboard(_inputFieldMenuAssets.upperField);
            //Debug.Log("active listener in newmenumanager");
        });


        _inputFieldMenuAssets.lowerField.onSelect.AddListener(delegate
        {
            _keyboardController.OpenVRKeyboard(_inputFieldMenuAssets.lowerField);

        });

    }

    public void StartFromMainMenu()
    {
        //print("Starting from main menu");
        ClosePrimaryMenus();
        _mainMenuAssets.mainMenu.SetActive(true);
        layersMenu.SetActive(false);
        _inputFieldMenuAssets.inputFieldMenu.SetActive(false);
        _sampleTerrainAssets.sampleTerrainsMenu.SetActive(false);

        //Button Fuctions
        _mainMenuAssets.sampleTerrainsButton.onClick.AddListener(SinglePlayerSampleTerrain);
        _mainMenuAssets.customTerrainsButton.onClick.AddListener(SinglePlayerCustomTerrainLogIn);
        //_mainMenuAssets.multiuserButton.onClick.AddListener(ToMultiplayerMenu);


    }

    public void RemoveListener(Button button)
    {
        button.onClick.RemoveAllListeners();
    }


    #region MULTIUSER

    private void returnToGameFromMultiplayerMenu()
    {
        ToggleCursorMode(false);
        inTerrain = true;
        _TerrainToolbar.terrainTools.SetActive(true);
        ClosePrimaryMenus();
        primaryMenus.SetActive(false);
    }


    /// <summary>
    /// When in a terrain
    /// </summary>
    public void ToMultiplayerMenu()
    {
        if (!GameState.vrDebug && !GameState.IsVR)
        {
            ToggleCursorMode(false);

        }
        if (GameState.InMultiuser) //button is clicked when in a multiuser room
        {
            _TerrainToolbar.terrainTools.SetActive(false);
            primaryMenus.SetActive(true);

            _twoButtonMenuAssets.twoButtonMenu.SetActive(true);
            _twoButtonMenuAssets.header.text = "Room Code: " + MultiplayerManager.Instance.joinCode;
            _twoButtonMenuAssets.topButton.GetComponentInChildren<TMP_Text>().text = "Leave Multiuser";
            _twoButtonMenuAssets.bottomButton.gameObject.SetActive(false);
            _twoButtonMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back to Game";
            _twoButtonMenuAssets.backButton.onClick.AddListener(delegate 
            {
                returnToGameFromMultiplayerMenu();
                _twoButtonMenuAssets.buttonClicked.Invoke();
            });

            //Leave Multiplayer Button
            _twoButtonMenuAssets.topButton.onClick.AddListener(delegate
            {
                NetworkManager.Singleton.Shutdown();
                clearAllPinsButton.SetActive(false);
                StartFromMainMenu();
                GameState.InMultiuser = false;

                ResetUI();
                _twoButtonMenuAssets.buttonClicked.Invoke();
            });
        }
        else //in singleplayer room
        {
            _TerrainToolbar.terrainTools.SetActive(false);
            _mainMenuAssets.mainMenu.SetActive(false);
            primaryMenus.SetActive(true);

            _twoButtonMenuAssets.twoButtonMenu.SetActive(true);
            _twoButtonMenuAssets.header.text = "Multiuser";
            _twoButtonMenuAssets.topButton.GetComponentInChildren<TMP_Text>().text = "Create Room";
            _twoButtonMenuAssets.bottomButton.gameObject.SetActive(true);
            _twoButtonMenuAssets.bottomButton.GetComponentInChildren<TMP_Text>().text = "Join Room";
            _twoButtonMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back to Game";

            //back button
            _twoButtonMenuAssets.backButton.onClick.AddListener(delegate
            {
                if (inTerrain)
                {
                    returnToGameFromMultiplayerMenu();
                    _twoButtonMenuAssets.buttonClicked.Invoke();
                }
                else
                {
                    StartFromMainMenu();
                    _twoButtonMenuAssets.buttonClicked.Invoke();
                }

            });

            //Create Room button
            _twoButtonMenuAssets.topButton.onClick.AddListener(delegate 
            {
                CreateRoom();
                //MultiPlayerCustomTerrainLogIn();
                _twoButtonMenuAssets.buttonClicked.Invoke();
            });

            //Join Room button
            _twoButtonMenuAssets.bottomButton.onClick.AddListener(delegate 
            { 
                JoinRoom();
                _twoButtonMenuAssets.buttonClicked.Invoke();
            });
        }

    }

    

    private void CreateRoom()
    {
        _twoButtonMenuAssets.twoButtonMenu.SetActive(false);
        _inputFieldMenuAssets.inputFieldMenu.SetActive(true);


        _inputFieldMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back";
        _inputFieldMenuAssets.header.text = "Create Room";
        _inputFieldMenuAssets.instructions.text = "";
        _inputFieldMenuAssets.upperFieldDescriptor.SetText("Username:");
        _inputFieldMenuAssets.upperField.text = "";
        _inputFieldMenuAssets.centerButton.GetComponentInChildren<TMP_Text>().text = "Create Room";
        _inputFieldMenuAssets.lowerField.contentType = TMP_InputField.ContentType.Standard;

        _inputFieldMenuAssets.lowerFieldObject.SetActive(false);

        //CREATE ROOM BUTTON
        _inputFieldMenuAssets.centerButton.onClick.AddListener(delegate
        {
            previouslyOpenedMenu = "CreateRoom";
            //_inputFieldMenuAssets.inputFieldMenu.SetActive(false);

            //MultiPlayerCustomTerrainLogIn();
            SinglePlayerCustomTerrainLogIn();

            _localPlayer.GetComponentInChildren<TMP_Text>().text = _inputFieldMenuAssets.upperFieldObject.GetComponentInChildren<TMP_InputField>().text;
            MultiplayerManager.Instance.CreateRelay();


            // disable per pixel if active in singleplayer
            TurnOffPerPixelData();
            _keyboardController.CloseVRKeyboard();


            _inputFieldMenuAssets.buttonClicked.Invoke();
        });

        //BACK BUTTON
        _inputFieldMenuAssets.backButton.onClick.AddListener(delegate
        {
            _inputFieldMenuAssets.inputFieldMenu.SetActive(false);
            ToMultiplayerMenu();
            _keyboardController.CloseVRKeyboard();

            _inputFieldMenuAssets.buttonClicked.Invoke();
        });
    }

    private void JoinRoom()
    {
        _twoButtonMenuAssets.twoButtonMenu.SetActive(false);

        _inputFieldMenuAssets.inputFieldMenu.SetActive(true);
        _inputFieldMenuAssets.lowerFieldObject.SetActive(true);

        _inputFieldMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back";
        _inputFieldMenuAssets.header.text = "Join Room";
        _inputFieldMenuAssets.instructions.text = "";
        _inputFieldMenuAssets.upperFieldDescriptor.SetText("Username:");
        _inputFieldMenuAssets.upperField.text = "";
        _inputFieldMenuAssets.lowerFieldDescriptor.SetText("Room Code:");
        _inputFieldMenuAssets.lowerField.text = "";
        _inputFieldMenuAssets.centerButton.GetComponentInChildren<TMP_Text>().text = "Join Room";
        _inputFieldMenuAssets.lowerField.contentType = TMP_InputField.ContentType.Standard;


        //JOIN ROOM BUTTON
        _inputFieldMenuAssets.centerButton.onClick.AddListener(delegate
        {
            previouslyOpenedMenu = "JoinRoom";
            _inputFieldMenuAssets.inputFieldMenu.SetActive(false);

            _localPlayer.GetComponentInChildren<TMP_Text>().text = _inputFieldMenuAssets.upperFieldObject.GetComponentInChildren<TMP_InputField>().text; //sets username
            MultiplayerManager.Instance.JoinRelay(_inputFieldMenuAssets.lowerFieldObject.GetComponentInChildren<TMP_InputField>().text);
            _keyboardController.CloseVRKeyboard();

            // disable per pixel if active in singleplayer
            TurnOffPerPixelData();

            
            _inputFieldMenuAssets.buttonClicked.Invoke();
        });

        //BACK BUTTON
        _inputFieldMenuAssets.backButton.onClick.AddListener(delegate
        {
            _inputFieldMenuAssets.inputFieldMenu.SetActive(false);
            ToMultiplayerMenu();
            _keyboardController.CloseVRKeyboard();


            _inputFieldMenuAssets.buttonClicked.Invoke();
        });
    }

    public void SetClientUI()
    {
        _TerrainToolbar.terrainMenuButton.gameObject.SetActive(false);
        //.activatedLayersButton.gameObject.SetActive(false);
        _TerrainToolbar.layersButton.gameObject.SetActive(true);
        _TerrainToolbar.perPixelButton.gameObject.SetActive(true);
        resetPosition.gameObject.SetActive(false); 
        _TerrainToolbar.roomCodeText.text = "Room Code: " + MultiplayerManager.Instance.joinCode;
    }

    public void ResetUI()
    {
        _TerrainToolbar.terrainMenuButton.gameObject.SetActive(true);
        _TerrainToolbar.layersButton.gameObject.SetActive(true);
        _TerrainToolbar.perPixelButton.gameObject.SetActive(true);
        _TerrainToolbar.roomCodeText.text = string.Empty;
        resetPosition.gameObject.SetActive(true);
    }
    
    public void DespawnPlayer(string errorType, string errorMessage)
    {
        NetworkManager.Singleton.Shutdown();
        
        ClosePrimaryMenus();
        primaryMenus.SetActive(true);
        errorMenu.SetActive(true);
        ToggleCursorMode(false); //cursor is visible, menu locked to screen
        
        errorMenu.transform.GetChild(0).GetComponent<TMP_Text>().text = errorType; //header
        errorMenu.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = errorMessage; //body
        errorMenu.GetComponentInChildren<Button>().onClick.AddListener(delegate
        {
            errorMenu.SetActive(false);
            ResetUI();
            returnToGameFromMultiplayerMenu();
        });
    }

    #endregion

    private void SinglePlayerSampleTerrain()
    {
        //print("Sample Terrains Menu");

        _sampleTerrainAssets.sampleTerrainsMenu.SetActive(true);
        _mainMenuAssets.mainMenu.SetActive(false);
        _sampleTerrainAssets.backButton.onClick.AddListener(StartFromMainMenu);
        isSampleTerrain = true;

        foreach (var button in _sampleTerrainAssets.sampleTerrainButtons)
            button.onClick.AddListener(
                delegate
                {
                    newUIScript.DepopulateLayers();
                    LoadSampleTerrain(button.GetComponent<DataPackBehaviour>());
                }
            );
    }

    public void LoadSampleTerrain(DataPackBehaviour dataPack)
    {
        //print("Loading sample terrain ");
        ToggleCursorMode(false); //Toggle controls on
        dataPack.LoadData();

        ClosePrimaryMenus();
        primaryMenus.SetActive(false);
        
        //Turn off all terrain tools
        //_TerrainToolbar.terrainLayers.gameObject.SetActive(false);
        _TerrainToolbar.PerPixelPanel.gameObject.SetActive(false);

        PerPixelDataReader.singleton.RemoveOldPins();
        
        inTerrain = true;
        exitButton.onClick.AddListener(delegate
        {
            SampleTerrainToMenu();
            exitButton.onClick.RemoveAllListeners();
        });
        
        EnterTerrain();

    }
    
    private void SampleTerrainToMenu()
    {
        //print("Sample Terrain To Menu");
        ToggleCursorMode(false);
        //_TerrainToolbar.terrainTools.SetActive(false);
        _TerrainToolbar.layersButton.gameObject.SetActive(true);
        //_TerrainToolbar.activatedLayersButton.gameObject.SetActive(false);

        //inTerrain = false;
        PerPixelDataReader.singleton.readingData = false;
        //newUIScript.DepopulateLayers();
        PauseMenu.SetActive(false);
        layersMenu.SetActive(false);
        primaryMenus.SetActive(true);
        _sampleTerrainAssets.sampleTerrainsMenu.SetActive(true);
    }

    public void CustomTerrainToMenu()
    {
        ToggleCursorMode(false);
        //_TerrainToolbar.terrainTools.SetActive(false);
        _TerrainToolbar.layersButton.gameObject.SetActive(true);
        //_TerrainToolbar.activatedLayersButton.gameObject.SetActive(false);
        //inTerrain = false;
        //newUIScript.DepopulateLayers();
        PauseMenu.SetActive(false);
        layersMenu.SetActive(false);
        primaryMenus.SetActive(true);
        customTerrainsMenu.SetActive(true);
    }
    public void SinglePlayerCustomTerrainLogIn()
    {
        //print("Custom Terrain Login");
        _mainMenuAssets.mainMenu.SetActive(false);
        _inputFieldMenuAssets.inputFieldMenu.SetActive(true);


        //Alter assets for login
        _inputFieldMenuAssets.header.text = "Sign in to JMARS";
        _inputFieldMenuAssets.upperFieldDescriptor.text = "Username:";
       // _inputFieldMenuAssets.upperField.text = "vr-demo"; //FOR DEMOS
        _inputFieldMenuAssets.upperField.text = "";
        _inputFieldMenuAssets.upperField.Select();
        
        _inputFieldMenuAssets.lowerFieldObject.SetActive(true);
        _inputFieldMenuAssets.lowerField.contentType = TMP_InputField.ContentType.Password;
        //_inputFieldMenuAssets.lowerField.text = "omed"; //FOR DEMOS
        _inputFieldMenuAssets.lowerField.text = "";
        


        _inputFieldMenuAssets.lowerFieldDescriptor.text = "Password:";
        _inputFieldMenuAssets.instructions.text = "To view custom terrains,\nplease log into your JMARS account.";
        _inputFieldMenuAssets.upperField.placeholder.GetComponent<TMP_Text>().text = "Enter username...";
        _inputFieldMenuAssets.lowerField.placeholder.GetComponent<TMP_Text>().text = "Enter password...";
        _inputFieldMenuAssets.lowerField.contentType = TMP_InputField.ContentType.Password;
        _inputFieldMenuAssets.centerButton.GetComponentInChildren<TMP_Text>().text = "Log In";
        _inputFieldMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back to Main Menu";

        //Disabled while still in development
        //_twoInputFieldMenuAssets.rememberMeToggle.SetActive(true);

        //Button Functions
        _inputFieldMenuAssets.backButton.onClick.AddListener(
            delegate
            {
                _inputFieldMenuAssets.backButton.onClick.RemoveAllListeners();
                StartFromMainMenu();
                _inputFieldMenuAssets.buttonClicked.Invoke();
                _keyboardController.CloseVRKeyboard();

            });
        
        //The listener is called twice for some reason unless the listeners are removed. I don't know where the listener is being added
        _inputFieldMenuAssets.centerButton.onClick.RemoveAllListeners();
        _inputFieldMenuAssets.centerButton.onClick.Invoke();
        
        _inputFieldMenuAssets.centerButton.onClick.AddListener(delegate
        {
            //print("Logging user in");
           // Download(StartCoroutine(GetUserTerrains(_inputFieldMenuAssets.upperField.text, _inputFieldMenuAssets.lowerField.text)), _inputFieldMenuAssets.inputFieldMenu);
           previouslyOpenedMenu = "SinglePlayerCustomTerrainLogIn";
           _keyboardController.CloseVRKeyboard();
           _inputFieldMenuAssets.notificationText.text = "";
           StartCoroutine(GetUserTerrains(_inputFieldMenuAssets.upperField.text, _inputFieldMenuAssets.lowerField.text));
        });

    }

    public void MultiPlayerCustomTerrainLogIn()
    {
        print("Custom Terrain Login");
        _mainMenuAssets.mainMenu.SetActive(false);
        _inputFieldMenuAssets.inputFieldMenu.SetActive(true);



        //Alter assets for login
        _inputFieldMenuAssets.header.text = "Sign in to JMARS";
        _inputFieldMenuAssets.upperFieldDescriptor.text = "Username:";
        _inputFieldMenuAssets.upperField.text = ""; //""vr-demo";


        _inputFieldMenuAssets.lowerFieldObject.SetActive(true);
        _inputFieldMenuAssets.lowerField.text = ""; //""omed";
        _inputFieldMenuAssets.lowerField.Select();


        _inputFieldMenuAssets.lowerFieldDescriptor.text = "Password:";
        _inputFieldMenuAssets.instructions.text = "To view custom terrains,\nplease log into your JMARS account.";
        _inputFieldMenuAssets.upperField.placeholder.GetComponent<TMP_Text>().text = "Enter username...";
        _inputFieldMenuAssets.lowerField.placeholder.GetComponent<TMP_Text>().text = "Enter password...";
        _inputFieldMenuAssets.lowerField.contentType = TMP_InputField.ContentType.Password;
        _inputFieldMenuAssets.centerButton.GetComponentInChildren<TMP_Text>().text = "Log In";
        _inputFieldMenuAssets.backButton.GetComponentInChildren<TMP_Text>().text = "Back to Main Menu";

        //Disabled while still in development
        //_twoInputFieldMenuAssets.rememberMeToggle.SetActive(true);

        //Button Functions
        _inputFieldMenuAssets.backButton.onClick.AddListener(
            delegate
            {
                _inputFieldMenuAssets.backButton.onClick.RemoveAllListeners();
                StartFromMainMenu();
                _inputFieldMenuAssets.buttonClicked.Invoke();
                _keyboardController.CloseVRKeyboard();

            });

        //The listener is called twice for some reason unless the listeners are removed. I don't know where the listener is being added
        _inputFieldMenuAssets.centerButton.onClick.RemoveAllListeners();
        _inputFieldMenuAssets.centerButton.onClick.Invoke();

        _inputFieldMenuAssets.centerButton.onClick.AddListener(delegate
        {
            print("Logging user in");
            // Download(StartCoroutine(GetUserTerrains(_inputFieldMenuAssets.upperField.text, _inputFieldMenuAssets.lowerField.text)), _inputFieldMenuAssets.inputFieldMenu);
            previouslyOpenedMenu = "MultiPlayerCustomTerrainLogIn";
            _keyboardController.CloseVRKeyboard();
            StartCoroutine(GetUserTerrains(_inputFieldMenuAssets.upperField.text, _inputFieldMenuAssets.lowerField.text));
        });


    }

    IEnumerator GetUserTerrains(string username, string password)
    {
        //print("Getting user terrains");

        if (username == "" || password == "")
        {
            _inputFieldMenuAssets.notificationText.text = "Incorrect username or password.";
            
        }
        else
        {
            _inputFieldMenuAssets.inputFieldMenu.SetActive(false);
            customTerrainsMenu.SetActive(false); //used if user refreshes terrains
            loadingMenu.SetActive(true);
            //yield return SceneDownloader.singleton.ChangeState(SceneDownloader.SceneSession.LISTSCENES);

            yield return StartCoroutine(SceneDownloader.singleton.DownloadViewList(username, password));
            exitButton.onClick.AddListener(delegate
            {
                CustomTerrainToMenu();
                exitButton.onClick.RemoveAllListeners();
            });
            SetRefreshTerrainsButton();
        }


    }

    private void SetRefreshTerrainsButton()
    {
        //If singleplayer
        refreshButton.onClick.AddListener(delegate
        {
            //print("Refreshing singleplayer terrains");
            
            StartCoroutine(GetUserTerrains(_inputFieldMenuAssets.upperField.text,
                _inputFieldMenuAssets.lowerField.text));
        });

        //else if multiplayer...
    }

    public void UserLoginFailed()
    {
        loadingMenu.SetActive(false);
        _inputFieldMenuAssets.inputFieldMenu.SetActive(true);
        _inputFieldMenuAssets.notificationText.text = "Incorrect username or password.";
    }

    //Also utilized in GameManager when user selects custom terrain
    public void ClosePrimaryMenus()
    {
        for (int i = 0; i < primaryMenus.transform.childCount; i++)
        {
            primaryMenus.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ToggleLayersPanel()
    {
        layersMenu.SetActive(!layersMenu.activeSelf);
    }

    /// <summary>
    /// When a user enters a terrain, they should have their cursor locked so that
    /// they are able to move the player and have all menus closed
    /// </summary>
    public void EnterTerrain()
    {
        //Re-activate buttons
        if(GameState.IsVR || GameState.vrDebug)
            _TerrainToolbar.tabTip.SetActive(false); //No need to show this in VR
        
        _TerrainToolbar.multiplayerButton.interactable = true;
        //_TerrainToolbar.activatedLayersButton.interactable = true;
        _TerrainToolbar.layersButton.interactable = true;
        _TerrainToolbar.perPixelButton.interactable = true;
        
        mouseLockedBeforePaused = true; // user starts out with mouse locked;
        ToggleCursorMode(true);
        ClosePrimaryMenus();
        primaryMenus.SetActive(false);
        _TerrainToolbar.terrainTools.SetActive(true);
        
        //set pause menu to false
        _pauseMenuAssets.pauseMenu.SetActive(false);
    }

    #region CLASSES

    [Serializable]
    protected class MainMenuAssets
    {
        public GameObject mainMenu;
        public Button customTerrainsButton;
        public Button sampleTerrainsButton;
        public Button multiuserButton;
    }

    [Serializable]
    public class InputFieldMenuAssets
    {
        [Header ("GameObjects")]
        public GameObject inputFieldMenu;
        public GameObject upperFieldObject;
        public GameObject lowerFieldObject;
        public GameObject rememberMeToggle;

        [Header ("Input Fields")]
        public TMP_InputField upperField;
        public TMP_InputField lowerField;

        [Header ("Text Assets")]
        public TMP_Text upperFieldDescriptor;
        public TMP_Text lowerFieldDescriptor;
        public TMP_Text instructions;
        public TMP_Text header;
        public TMP_Text notificationText;

        [Header ("Buttons")]
        public Button backButton;
        public Button centerButton;


        [HideInInspector] public UnityEvent buttonClicked;

        public void Start()
        {
            buttonClicked.AddListener(delegate
            {
                backButton.onClick.RemoveAllListeners();
                centerButton.onClick.RemoveAllListeners();
            });
        }

    }

    [Serializable]
    protected class SampleTerrainAssets
    {
        public GameObject sampleTerrainsMenu;
        public Button backButton;
        [NonReorderable] public List<Button> sampleTerrainButtons; //Also contain datapacks
    }

    [Serializable]
    protected class TwoButtonMenuAssets
    {
        public GameObject twoButtonMenu;
        public TMP_Text header;
        public Button topButton;
        public Button bottomButton;
        public Button backButton;

        [HideInInspector] public UnityEvent buttonClicked;

        public void Start()
        {
            buttonClicked.AddListener(delegate
            {
                topButton.onClick.RemoveAllListeners();
                bottomButton.onClick.RemoveAllListeners();
                backButton.onClick.RemoveAllListeners();
            });
        }
    }

    [Serializable]
    protected class PauseMenuAssets
    {
        public GameObject pauseMenu;
        public TMP_Text header;
        public Button topButton;
        public Button bottomButton;
        public Button backButton;
    }

    [Serializable]
    public class TerrainToolbar
    {
        [Header("Panels")]
        public GameObject terrainTools;
        public GameObject terrainLayers;
        public GameObject PerPixelPanel;
        public GameObject leftSideTools;
        public HorizontalLayoutGroup horizontalLayoutGroup;
        
        [Header ("Text Assets")]
        public TMP_Text PerPixelFloat;
        public TMP_Text PerPixelShort;
        public TMP_Text roomCodeText;
        public TMP_Text generalTip;
        public GameObject tabTip;

        [Header("Buttons")]
        public Button perPixelButton;
        public Button multiplayerButton;
        [FormerlySerializedAs("deactivatedLayersButton")] public Button layersButton;
        public Button terrainMenuButton;

        /// <summary>
        /// Reset terrain position back to -1f,-5f,0f
        /// </summary>
        public void ResetTerrainPosition()
        {
            SceneMaterializer.singleton.gameObject.transform.position = new Vector3(-1f, -5f, 0f);
        }

    }

    #endregion
}

