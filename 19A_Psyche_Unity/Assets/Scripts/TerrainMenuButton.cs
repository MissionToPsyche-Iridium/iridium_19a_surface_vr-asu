using System;
using System.Collections;
using System.Collections.Generic;
using TerrainEngine;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainMenuButton : MonoBehaviour
{
    public enum buttonType {PARFAIT, TERRAIN, SCENE};

    [SerializeField] private buttonType type;
    [SerializeField] private GameObject terrain;
    [SerializeField] private String unityScene;
    private SceneMaterializer sceneMaterializer;
    private DataPackBehaviour scene;
    
    
    // Start is called before the first frame update
    void Start()
    {
        switch (type)
        {
            case buttonType.PARFAIT:
                sceneMaterializer = GameObject.Find("Terrain").GetComponent<SceneMaterializer>();
                scene = GetComponent<DataPackBehaviour>();
                break;

            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void LoadScene()
    {
        switch(type)
        {
            case buttonType.PARFAIT:
                sceneMaterializer.LoadTerrain(scene);
                break;

            case buttonType.SCENE:
                SceneManager.LoadScene(unityScene);
                break;

            case buttonType.TERRAIN:
                GameObject[] terrains = GameObject.FindGameObjectsWithTag("Terrain");
                foreach (GameObject t in terrains)
                {
                    t.SetActive(false);
                }
                terrain.SetActive(true);
                break;
        }
        
    }
}
