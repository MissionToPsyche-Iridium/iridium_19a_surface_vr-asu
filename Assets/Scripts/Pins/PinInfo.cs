using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UserInterface;

public class PinInfo : MonoBehaviour
{
    public void OpenVRKeyboard(GameObject inputField)
    {
        KeyboardController.Instance.OpenVRKeyboard(inputField.GetComponent<TMP_InputField>());
    }   
    
    public void CloseVRKeyboard(GameObject inputField)
    {
        KeyboardController.Instance.CloseVRKeyboard();
    }  
}
