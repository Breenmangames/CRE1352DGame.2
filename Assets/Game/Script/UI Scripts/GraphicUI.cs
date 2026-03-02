using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using System;

public class GraphicUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    GraphicUI uiDocument;
    Button _cancel;
    Button _apply;

     void OnEnable()
    {
        //var root = GetComponent<GraphicUI>().rootVisualElement;
       // var startButton = root.Q<Button>("apply");

        _cancel.clicked += onCancel;
        _apply.clicked += onApply;
    }

    private void OnDisable()
    {
        _cancel.clicked -= onCancel;
        _apply.clicked -= onApply;
    }

    void onCancel()
    {
        gameObject.SetActive(false);
    }
    void onApply()
    {
        // Apply changes to the graphics settings here
        gameObject.SetActive(false);
    }
}



