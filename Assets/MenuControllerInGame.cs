using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControllerInGame : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    
    public void GoToMenu() 
    {
        SceneManager.LoadScene(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void Close() 
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GoToMenu();
    }

    
}
