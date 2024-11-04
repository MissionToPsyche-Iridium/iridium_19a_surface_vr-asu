using System.Collections;
using System.Collections.Generic;
using TerrainEngine;
using UnityEngine;

public class TerrainMenuButton : MonoBehaviour
{
    private SceneMaterializer sceneMaterializer;
    private DataPackBehaviour scene;
    // Start is called before the first frame update
    void Start()
    {
        sceneMaterializer = GameObject.Find("Terrain").GetComponent<SceneMaterializer>();
        scene = GetComponent<DataPackBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void LoadScene()
    {
        sceneMaterializer.LoadTerrain(scene);
    }
}
