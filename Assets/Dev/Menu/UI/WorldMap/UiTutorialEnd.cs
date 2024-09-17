using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTutorialEnd : MonoBehaviour
{

    public SceneLoader SceneLoader;
    void SetDirty(int notUsed)
    {
      
    }

    public void SetUpContent(int notUsed)
    {
       
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
 
    }

    public void OnClickLobby()
    {
        SceneLoader.LoadScene("Register");
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }
}
