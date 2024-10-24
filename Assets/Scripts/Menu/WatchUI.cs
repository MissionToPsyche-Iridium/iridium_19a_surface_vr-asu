using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserInterface;

public class WatchUI : MonoBehaviour
{
    public Button scalebarButton;
    public Button terrainButton;
    public Button layersButton;
    // Start is called before the first frame update
    void Awake()
    {
        // instantiating button listeners
        scalebarButton.onClick.AddListener(delegate
        {
            //refactor for menu system
            //ScaleBarPrefabs.singleton.ToggleScalebarMode(); // toggles on/off scalebar feature
        });
        terrainButton.onClick.AddListener(delegate
        {
            // implement later
        });
        layersButton.onClick.AddListener(delegate
        {
            // implement later
        });
    }
}
