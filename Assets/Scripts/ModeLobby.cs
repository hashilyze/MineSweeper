using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeLobby : MonoBehaviour
{
    [SerializeField] private GameObject _uiTitle;
    [SerializeField] private GameObject _uiLobby;
    [SerializeField] private Board[] _boardTemplates;


    public void OpenTitle ()
    {
        _uiTitle.SetActive(true);
        _uiLobby.SetActive(false);
    }
    public void OpenLobby ()
    {
        _uiLobby.SetActive(true);
        _uiTitle.SetActive(false);
    }
    public void ExitTheGame ()
    {
        Application.Quit();
    }
    public void LoadLevel (int loadID)
    {
        SceneManager.LoadScene("Scene_Level");
        BoardLoader.LoadedBoard = _boardTemplates[loadID];
        BoardLoader.BoardID = loadID;
    }

    private void Awake ()
    {
        OpenTitle();
    }
}
