using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class UIController : NetworkBehaviour
{
    public GameObject goToGameButton;
    public string gameScene;
    public GameObject mainCamera;
    void Start()
    {
        
    }
    public void GoToGameButton()
    {
        SceneManager.LoadScene(3, LoadSceneMode.Additive);
        mainCamera.SetActive(false);
        goToGameButton.SetActive(false);
        Debug.Log("Button has been down");
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
