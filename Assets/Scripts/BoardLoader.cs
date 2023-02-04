using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardLoader : MonoBehaviour
{
    private static BoardLoader s_instance;

    public static int BoardID
    {
        get => s_instance._loadedBoardID;
        set => s_instance._loadedBoardID = value;
    }
    public static Board LoadedBoard
    {
        get => s_instance._loadedBoard;
        set => s_instance._loadedBoard = value;
    }

    [SerializeField] private Board _loadedBoard;
    [SerializeField] private int _loadedBoardID;


    private void Awake ()
    {
        if (s_instance)
        {
            Destroy(gameObject);
            return;
        }

        s_instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
