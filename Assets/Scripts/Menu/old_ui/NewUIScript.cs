using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Multiuser;
using TerrainEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XRControls;

public class NewUIScript : MonoBehaviour
{
    public static NewUIScript singleton;

    [SerializeField] private GameObject newUI;
    public Transform playerTransform;
    public Transform playerKeyboard;


    [HideInInspector] public JMARSScene.Metadata jmarsScene;

    [SerializeField] private GameObject layersParent;
    [SerializeField] private GameObject layerPrefab;
    [SerializeField] private GameObject exaggerationParent;
    [SerializeField] private GameObject loadingMenu;
    public Slider loadingBar;

    [SerializeField] private GameObject SceneParent;
    [SerializeField] private GameObject ScenePrefab;
    [SerializeField] private GameObject TerrainTools;
    private Texture2D _thumbnailImg;
    private string _viewSceneJson;

    [SerializeField] private GameObject customTerrainsMenu;
    [SerializeField] private GameObject sampleTerrainsMenu;

    public TMP_InputField username;
    public TMP_InputField password;


    private void Start()
    {
        singleton = this;
        newUI.SetActive(true);
        //playerTransform = NewMenuManager.singleton._localPlayer.transform;
    }
    
    private void Update()
    {
        if (playerTransform != null)
        {
            float offset = 1.5f;
            PlaceUIInFrontOfPlayer(playerTransform, newUI, offset);
            ShowUIForPlayer(playerTransform, newUI, offset);
        }
    }
    
    
    /// <summary>
    /// Populates terrain layers onto UI panel in-game
    /// </summary>
    /// <param name="scene"></param>
    /// 
    public void PlaceUIInFrontOfPlayer(Transform playerTransform, GameObject uiElement, float heightOffset)
    {
        if (playerTransform != null)
            //if (playerCamera != null)
        {
            Vector3 playerPosition = playerTransform.position;
            Vector3 playerForward = playerTransform.forward;
            //Vector3 cameraPosition = playerCamera.transform.position;
            //Vector3 cameraForward = playerCamera.transform.forward;
            float distanceInFront = 3f; // Adjust this distance as needed
            Vector3 uiPosition = playerPosition + playerForward * distanceInFront + Vector3.up * heightOffset;
            uiElement.transform.position = uiPosition;
            //Vector3 keyboardPosition = new Vector3(uiPosition.x , uiPosition.y - 1f, uiPosition.z - 0.25f);
            //playerKeyboard.position = keyboardPosition;
            //playerKeyboard.rotation = playerTransform.rotation;
        }
    }
    public void ShowUIForPlayer(Transform playerTransform, GameObject uiElement, float heightOffset)
    {
        //PlaceUIInFrontOfPlayer(playerCamera.transform, uiElement);
        //uiElement.transform.rotation = playerCamera.transform.rotation;
    PlaceUIInFrontOfPlayer(playerTransform, uiElement, heightOffset);
        uiElement.transform.rotation = playerTransform.rotation;
        uiElement.SetActive(true);
    }

    /// <summary>
    /// Popualtes custom user scenes onto a UI panel in-game
    /// </summary>
    /// <param name="userScenes"></param>
    public IEnumerator PopulateScenes(List<JMARSScene.Metadata> userScenes)
    {

        float loadingbarValue = 0f;
        loadingBar.value = loadingbarValue;

        //Debug.Log("populating scenes");
        if (SceneParent.transform.childCount != 0) //Checks if there are already scenes populated on the userScenes page
        {
            //Debug.Log("Destroying Old Singleplayer Scenes");
            foreach (Transform child in SceneParent.transform)
                Destroy(child.gameObject);
        }

       // print("Number of scenes = " + userScenes.Count);

        //loop through all scenes
        foreach (var scene in userScenes)
        {

            var prefab = gameObject;
            prefab = Instantiate(ScenePrefab, SceneParent.transform); //create new layer gameobject
            prefab.name = scene.scene_name; //name of gameobject in hierarchy
            prefab.GetComponentInChildren<TextMeshProUGUI>().text = scene.scene_name; //text of textmeshpro comp on layer prefab
            //Debug.Log(scene.scene_name);
            var button = prefab.GetComponent<Button>();

            //StartCoroutine(GetSceneData(scene));

            yield return StartCoroutine(GetSceneData(scene));
            scene.thumbnail = _thumbnailImg;
            prefab.GetComponentInChildren<RawImage>().texture = scene.thumbnail;

            loadingbarValue += (1f / userScenes.Count);
            loadingBar.value = loadingbarValue;
           // print("loading bar value = " + loadingbarValue);


            button.onClick.AddListener(delegate
            {
                //Get rid of the layers from the previous terrain
                DepopulateLayers();
                
                /*NewMenuManager.singleton.previouslyOpenedMenu = "CustomTerrains";
                SceneDownloader.singleton.terrainURL = "https://cm.mars.asu.edu/api/vr/viewScene.php?access_key=" + scene.access_key;
                jmarsScene = scene;
                NewMenuManager.singleton.exitButton.onClick.AddListener(NewMenuManager.singleton.CustomTerrainToMenu);
                customTerrainsMenu.SetActive(false);
                StartCoroutine(SceneDownloader.singleton.ChangeState(SceneDownloader.SceneSession.DOWNLOADING));
                //Debug.Log("jmarsscene access key = " + jmarsScene.access_key);

                NewMenuManager.singleton.isSampleTerrain = false;

                //If the button is pressed, destroy the children from the scenes menu
                //ScenesMenu.singleton.DestroySingleplayerCustomSceneList();

                loadingMenu.SetActive(true);*/

            });

        }
        loadingMenu.SetActive(false);
        //NewMenuManager.singleton.ClosePrimaryMenus();
        customTerrainsMenu.SetActive(true);
        yield return null;

    }


    public IEnumerator GetSceneData(JMARSScene.Metadata scene)
    {
        var url = "https://cm.mars.asu.edu/api/vr/viewScene.php?access_key=" + scene.access_key;
        using var webRequest = UnityWebRequest.Get(url);
        var downloadHandler = webRequest.downloadHandler;

        yield return webRequest.SendWebRequest();

        _viewSceneJson = downloadHandler.text;
        var currentScene = JsonConvert.DeserializeObject<JMARSScene>(_viewSceneJson);


        // download image
        string thumbnailUrl = currentScene.thumbnail_img;
        //print(thumbnailUrl);
        var thumbnailDownloadHandler = new DownloadHandlerTexture();
        var req = new UnityWebRequest(thumbnailUrl, "GET", thumbnailDownloadHandler, null);
        yield return req.SendWebRequest();

        _thumbnailImg = thumbnailDownloadHandler.texture;

        //Have to get reference to DownloadTexture() from SceneDownloader script.
        //This will let you take the link to the graphic image then it can be used to put on the image object.

        //prefab.AddComponent<Image>().sprite = currentScene.layers[0].graphic_img;
    }


    public void DepopulateLayers()
    {
        foreach (Transform VARIABLE in layersParent.transform)
        {
            Destroy(VARIABLE.gameObject);
        }
    }

    /*/// <summary>
    /// ClientRPC to update the layer transparency across a network
    /// </summary>
    /// <param name="sliderValue"></param>
    [ClientRpc]
    void LayerClientRpc(JMARSScene scene, string layerName, float sliderValue)
    {
        Debug.Log($"Tracking LayerClientRPC Values. Scene = {scene.name}. Layer = {layerName}. Slider Value = {sliderValue}");
        foreach (var layer in scene.layers)
        {
            if (layerName.Equals(layer.layer_name))
            {
                layer.transparency = sliderValue;
            }
        }
    }*/


    /// <summary>
    /// Populates terrain layers onto UI panel in-game
    /// </summary>
    /// <param name="scene"></param>
    public void PopulateLayers(JMARSScene scene)
    {
        //NewMenuManager.singleton.inTerrain = true;
        //Debug.Log("populating layers");
        //print("Number of layers = " + scene.layers.Capacity);
        //destroys old layers
        if (layersParent.transform.childCount != 0)
        {
           // Debug.Log("Destroying Old Singleplayer Layer Fields");
            foreach (Transform child in layersParent.transform)
            {
                Destroy(child.gameObject);
            }


        }

        //8C5F40

        //loop through all layers
        foreach (var layer in scene.layers)
        {
            //create new layer gameobject
            var prefab = gameObject;
            prefab = Instantiate(layerPrefab, layersParent.transform);
            prefab.name = layer.layer_name; //name of gameobject in hierarchy
            prefab.GetComponentInChildren<TextMeshProUGUI>().text = layer.layer_name; //text of textmeshpro comp on layer prefab
            
            var slider = prefab.GetComponentInChildren<Slider>(); //slider component in child object
            //Get Layer toggle from children
            //It starts off as ON.
            var toggle = prefab.GetComponentInChildren<Toggle>();

            slider.onValueChanged.AddListener(t =>
            {
                if (t == 0)
                {
                    toggle.isOn = false;

                }
                else
                {
                    toggle.isOn = true;
                }
                layer.transparency = t;
            });

            slider.onValueChanged.AddListener(t =>
            {
                layer.transparency = (float)(slider.value);
                
                if (GameState.InMultiuser)
                {
                    LayerSync.singleton.SendUnnamedMessage(layer.layer_name);
                    LayerSync.singleton.SendUnnamedMessage((layer.transparency).ToString());
                }
            });

            layer.slider = slider;
            
            toggle.onValueChanged.AddListener(t =>
            {

                if (toggle.isOn)
                {
                    //layerBackground.color = new Color32(198, 117, 63, 80);

                    layer.transparency = slider.value;
                }
                else
                {
                    //layerBackground.color = new Color32(142, 82, 41, 80);
                    layer.transparency = 0;
                }
                
                if (GameState.InMultiuser)
                {
                    LayerSync.singleton.SendUnnamedMessage(layer.layer_name);
                    LayerSync.singleton.SendUnnamedMessage((layer.transparency).ToString());
                }

            });

            layer.toggle = toggle;

            //loop through all layers
        }
        
        if(GameState.generateDataImages){
            //print("adding data layers as images in terrain");
           
            for (int i = 0; i < SceneDownloader.singleton.datalayertextures.Count; i++)
            {
                Texture2D flippedDataTexture = new Texture2D(SceneDownloader.singleton.datalayertextures[i].width,
                    SceneDownloader.singleton.datalayertextures[i].height);
                
                for (int x = 0; x < SceneDownloader.singleton.datalayertextures[i].width; x++)
                {
                    for (int y = 0; y < SceneDownloader.singleton.datalayertextures[i].height; y++)
                    {
                        flippedDataTexture.SetPixel(x, SceneDownloader.singleton.datalayertextures[i].height - y,
                            SceneDownloader.singleton.datalayertextures[i].GetPixel(x, y));
                    }
                }
                flippedDataTexture.Apply();
                JMARSScene.Layer layer = new JMARSScene.Layer();
                layer.graphicTexture = flippedDataTexture;
                layer.layer_name = "Data Layer " + i;
                layer.transparency = 1; // Turn the layer on by default
                layer.toggle_state = "true";

                //create new layer gameobject
                var prefab = gameObject;
                prefab = Instantiate(layerPrefab, layersParent.transform);
                prefab.name = layer.layer_name; //name of gameobject in hierarchy
                prefab.GetComponentInChildren<TextMeshProUGUI>().text =
                    layer.layer_name; //text of textmeshpro comp on layer prefab
                
                var slider = prefab.GetComponentInChildren<Slider>(); //slider component in child object
                //Get Layer toggle from children
                //It starts off as ON.
                var toggle = prefab.GetComponentInChildren<Toggle>();
                toggle.isOn = true; // Turn the layer on by default
                slider.onValueChanged.AddListener(t =>
                {
                    if (t == 0)
                    {
                        toggle.isOn = false;

                    }
                    else
                    {
                        toggle.isOn = true;
                    }

                    layer.transparency = t;
                });

                slider.onValueChanged.AddListener(t =>
                {
                    layer.transparency = (float)(slider.value);

                    if (GameState.InMultiuser)
                    {
                        LayerSync.singleton.SendUnnamedMessage(layer.layer_name);
                        LayerSync.singleton.SendUnnamedMessage((layer.transparency).ToString());
                    }
                });

                toggle.onValueChanged.AddListener(t =>
                {

                    if (toggle.isOn)
                    {
                        //layerBackground.color = new Color32(198, 117, 63, 80);
                        layer.transparency = slider.value;
                    }
                    else
                    {
                        //layerBackground.color = new Color32(142, 82, 41, 80);
                        layer.transparency = 0;
                    }

                    if (GameState.InMultiuser)
                    {
                        LayerSync.singleton.SendUnnamedMessage(layer.layer_name);
                        LayerSync.singleton.SendUnnamedMessage((layer.transparency).ToString());
                    }
                });


                   // print("Adding data layer " + i);
                    SceneDownloader.singleton.scene.layers.Add(layer);
            }

        }

        //creates another "layer" for dymanic exaggeration
        DynamicExaggeration(scene);

    }

    /// <summary>
    /// Creates a layer onto each scene that allows the user to change the exaggeration of the selected terrain.
    /// </summary>
    /// <param name="scene"></param>
    private void DynamicExaggeration(JMARSScene scene)
    {
        //creates exaggeration slider on layers menu after loading all layers
        var exagObj = gameObject;
        exagObj = exaggerationParent;
        //RESET EXAGGERATION BUTTON
        var exagReset = exagObj.GetComponentInChildren<Button>();

        //calculates scaledheight value in Unity "units" -- from SceneMaterializer 
        var exagSlider = exagObj.GetComponentInChildren<Slider>();

        var exag = scene.exaggeration.Split(", ")[0];
        var exaggeration = Convert.ToSingle(exag);

        SceneMaterializer.singleton.exaggerationSlider = exagSlider;

        //EXAGGERATION SLIDER VALUES -- based on scale values in inspector
        exagSlider.minValue = 1;
        exagSlider.maxValue = 5;
        exagSlider.value = 1;
        //exagSlider.wholeNumbers = true;
        //SceneMaterializer.singleton.heightMaterial.SetFloat("_scaleFactor", -(float)exaggeration * 0.001f * exagSlider.value); 
        //SceneMaterializer.singleton.terrain.transform.localScale = new Vector3(SceneMaterializer.singleton.terrain.transform.localScale.x, exagSlider.value, SceneMaterializer.singleton.terrain.transform.localScale.z);

        //EXAGGERATION VALUE TEXT FIELD
        var exagValue = exagObj.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        exagValue.text = "Exaggeration: " + (exagSlider.value);

        exagSlider.onValueChanged.AddListener(t =>
        {
            if (GameState.InMultiuser)
            {
                LayerSync.singleton.SendUnnamedMessage("Exaggeration");
                LayerSync.singleton.SendUnnamedMessage(exagSlider.value.ToString());
            }
            
            //print("Exaggeration Slider set to " + exagSlider.value);
            
            //SceneMaterializer.singleton.exaggerationSlider.value = exagSlider.value;
            //SceneMaterializer.singleton.heightMaterial.SetFloat("_scaleFactor", -(float)exaggeration * 0.001f * t); 
            //float heightValue = scene.depthTexture.GetPixel(scene.depthTexture.width/2, scene.depthTexture.height/2).r * SceneMaterializer.singleton.heightMaterial.GetFloat("_scaleFactor");
            //SceneMaterializer.singleton.tiles.transform.localPosition = new Vector3(0, -heightValue, 0);
            
            //original scaling is (200, 200, 200) - multiply current scale value by 200 to get approporiate values
            SceneMaterializer.singleton.terrain.transform.localScale = new Vector3(SceneMaterializer.singleton.terrain.transform.localScale.x, t * 200, SceneMaterializer.singleton.terrain.transform.localScale.z);

            //converts unity scaledheight back into height value to get accurate height in meters
            exagValue.text = "Exaggeration: " + (exagSlider.value);
        });

        exagReset.onClick.AddListener(delegate
        {
            exagSlider.value = 1;
            SceneMaterializer.singleton.terrain.transform.localScale = new Vector3(SceneMaterializer.singleton.terrain.transform.localScale.x, 200, SceneMaterializer.singleton.terrain.transform.localScale.z);
            exagValue.text = "Exaggeration: " + (exagSlider.value);
        });
        

    }
}
