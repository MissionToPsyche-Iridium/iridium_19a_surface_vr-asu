using System;
using System.Collections;
using System.Collections.Generic;
using TerrainEngine;
using TMPro;
using UnityEngine;


namespace UserInterface
{
    public class InfoPanel : MonoBehaviour
    {
        public static InfoPanel Panel;
        public TMP_Text terrainNameText; // terrain name
        public TMP_Text bodyText; // planet
        public TMP_Text dimText; // terrain dimensions
        public TMP_Text exagText; // exaggeration
        
        private void Start()
        {
            Panel = this;
        }

        public void ChangeName(JMARSScene scene)
        {
            terrainNameText.text = scene.name;
            bodyText.text = "Body: " + scene.body;
            
            var xdim = scene.dimension.Split("x")[0];
            var ydim = scene.dimension.Split("x")[1];
            var zdim = scene.dimension.Split("x")[2];

            float xxdim = Convert.ToSingle(xdim);
            float yydim = Convert.ToSingle(ydim);
            float zzdim = Convert.ToSingle(zdim); //height

            string dimStr = Math.Round(xxdim, 0) + scene.units + " X " + Math.Round(yydim, 0) + scene.units; // + " X " + Math.Round(zzdim, 0) + scene.units;
            dimText.text = "Dimensions: " + dimStr;

            exagText.text = "Exaggeration: " + Math.Round(SceneMaterializer.singleton.exaggerationSlider.value, 2) + "x";
        }

        /// <summary>
        /// Called on the exaggeration Slider's OnValueChanged() Event
        /// </summary>
        public void ChangeExaggeration()
        {
            exagText.text = "Exaggeration: " + Math.Round(SceneMaterializer.singleton.exaggerationSlider.value, 2) + "x";
        }
    }

}