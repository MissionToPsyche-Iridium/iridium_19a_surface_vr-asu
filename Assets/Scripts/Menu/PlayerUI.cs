using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace UserInterface
{

    /// <summary>
    /// This script controls the transform of the Canvas in world space to follow VR player around.
    /// Attached to XR Rig.
    /// </summary>
    public class PlayerUI : MonoBehaviour 
    {
        public Canvas canvas;
        public GameObject keyboard;
        private bool isCanvasOpen = true;
        private XRControls.XRController _xrController; // Reference to the XR controller that controls player movement

        void Start()
        {
            canvas.enabled = true;
            _xrController = gameObject.GetComponent<XRControls.XRController>();
        }

        void Update()
        {
            /*// Open canvas on VR controller button press (for example, Oculus Quest X button)
            if (GameState.InTerrain && _xrController.aActive) // == "canceled")
            {
                ToggleCanvas();
                _xrController.aActive = false;
            }*/

            if (gameObject != null)
            {
                float offset = 2f;
                var parent = canvas.transform.parent;
                PlaceUIInFrontOfPlayer(gameObject.transform, canvas.transform.parent.gameObject, offset);
                ShowUIForPlayer(gameObject.transform, canvas.transform.parent.gameObject, offset);
            }
        }

        void ToggleCanvas()
        {
            isCanvasOpen = !isCanvasOpen;
            canvas.enabled = isCanvasOpen;

            if (!isCanvasOpen)
            {
                keyboard.SetActive(false);
            }
        }

        private void PlaceUIInFrontOfPlayer(Transform playerTransform, GameObject uiElement, float heightOffset)
        {
            if (playerTransform != null)
            {
                Vector3 playerPosition = playerTransform.position;
                Vector3 playerForward = playerTransform.forward;
                float distanceInFront = 4f; // Adjust this distance as needed
                Vector3 uiPosition = playerPosition + playerForward * distanceInFront + Vector3.up * heightOffset;
                uiElement.transform.position = uiPosition;
            }
        }

        private void ShowUIForPlayer(Transform playerTransform, GameObject uiElement, float heightOffset)
        {
            PlaceUIInFrontOfPlayer(playerTransform, uiElement, heightOffset);
            uiElement.transform.rotation = playerTransform.rotation;
            uiElement.SetActive(true);
        }
    }
}