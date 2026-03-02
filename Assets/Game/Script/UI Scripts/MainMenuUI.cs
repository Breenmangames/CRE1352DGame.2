using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using System;

public class MainMenuUI : MonoBehaviour
{
    private UIDocument _document;
    [SerializeField] private string _sceneToLoad;
    private Button _Button;

    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();
       

        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(onAllButtonsClick);
        }
    }

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
       var root = GetComponent<UIDocument>().rootVisualElement;
       var startButton = root.Q<Button>("PlayButton");


        startButton.clicked += () =>
         {
             SceneManager.LoadScene(_sceneToLoad);
         };



    }

    private void OnDisable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var startButton = root.Q<Button>("PlayButton");
        startButton.clicked -= () =>
        {
            SceneManager.LoadScene(_sceneToLoad);
        };

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(onAllButtonsClick);
        }
    }

    private void onAllButtonsClick(ClickEvent evt)
    {
        _audioSource.Play();
    }
   
}
