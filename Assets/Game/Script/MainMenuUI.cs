using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private string _sceneToLoad;
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

    // Update is called once per frame
   
}
