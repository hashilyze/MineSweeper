using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ModeLevel : MonoBehaviour
{
    private enum EState { Pause, Clear, Over }

    [Header("Play")]
    [SerializeField] private GameObject _uiHud;
    [SerializeField] private MSCore _core;
    [SerializeField] private MSRenderer _renderer;
    [SerializeField] private Timer _timer;
    [SerializeField] private TMPro.TextMeshProUGUI _mineBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _timeBoard;
    private EState _state;

    [Header("Menu")]
    [SerializeField] private GameObject _uiMenu;
    [SerializeField] private TMPro.TextMeshProUGUI _currentTimeBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _bestTimeBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _leftTileBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _totalTileBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _leftMineBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _totalMineBoard;
    [SerializeField] private TMPro.TextMeshProUGUI _messageBoard;
    [SerializeField] private Button _resumeButton;


    public void OpenMenu ()
    {
        _uiMenu.SetActive(true);
        _timer.StopTimer();

        _currentTimeBoard.text = _timer.TimeToString();
        _bestTimeBoard.text = Timer.TimeToString(PlayerPrefs.GetFloat(BoardLoader.BoardID.ToString()));
       
        _leftTileBoard.text = (_core.Board.Size.x * _core.Board.Size.y - _core.Board.BlockCount - _core.Board.OpenCount).ToString();
        _totalTileBoard.text = (_core.Board.Size.x * _core.Board.Size.y - _core.Board.BlockCount).ToString();

        _leftMineBoard.text = (_core.Board.MineCount - _core.Board.FlagCount).ToString();
        _totalMineBoard.text = _core.Board.MineCount.ToString();

        switch (_state)
        {
        case EState.Pause:
            _messageBoard.text = "Pause";
            _resumeButton.interactable = true;
            break;
        case EState.Clear:
            _messageBoard.text = "Clear";
            _resumeButton.interactable = false;
            break;
        case EState.Over:
            _messageBoard.text = "Over";
            _resumeButton.interactable = false;
            break;
        }
    }
    public void CloseMenu ()
    {
        _uiMenu.SetActive(false);
        _timer.StartTimer();
    }

    public void ExitLevel ()
    {
        SceneManager.LoadScene("Scene_Lobby");
    }

    public void Restart ()
    {
        _core.ResetBoard();
        _renderer.ResetBoard();
        _renderer.DrawBackgroundTiles();

        _core.GenerateMines();
        _renderer.DrawCloseTiles();

        _timer.ResetTimer();
        //_timer.StartTimer();

        _timeBoard.text = _timer.TimeToString();
        _mineBoard.text = _core.Board.MineCount.ToString();
        _state = EState.Pause;
    }

    private void UpdateOpenTileRender (int x, int y)
    {
        _renderer.DrawOpen(x, y);
        _core.Board.ExcuteToAdjacent(x, y, (x, y) => _renderer.DrawOpen(x, y));
    }

    private void UpdateLeftMines (int x, int y)
    {
        _mineBoard.text = (_core.Board.MineCount - _core.Board.FlagCount).ToString();
    }

    private void GameClearEvent ()
    {
        _state = EState.Clear;
        float prevBestTime = PlayerPrefs.GetFloat(BoardLoader.BoardID.ToString());
        if (prevBestTime <= 0.0f || _timer.Current < prevBestTime)
        {
            PlayerPrefs.SetFloat(BoardLoader.BoardID.ToString(), _timer.Current);
        }
        OpenAllMines();
    }
    private void GameOverEvent ()
    {
        _state = EState.Over;
        OpenAllMines();
    }
    private void OpenAllMines ()
    {
        for(int y = 0; y < _core.Board.Size.y; ++y)
        {
            for (int x = 0; x < _core.Board.Size.x; ++x)
            {
                if (_core.Board.GetMine(x, y))
                {
                    _renderer.EraseClose(x, y);
                }
            }
        }
    }

    private void Awake ()
    {
        _core.TemplateBoard = BoardLoader.LoadedBoard;

        {
            // Game Clear Evenets
            _core.OnGameClear += () => Debug.Log("Game Clear");
            //_core.OnGameClear += _renderer.DrawMines;
            _core.OnGameClear += GameClearEvent;
            _core.OnGameClear += OpenMenu;
            // Game Over Events
            _core.OnGameOver += () => Debug.Log("Game Over");
            //_core.OnGameOver += _renderer.DrawMines;
            _core.OnGameOver += GameOverEvent;
            _core.OnGameOver += OpenMenu;
        }

        {
            _core.OnFlagTile += _renderer.DrawFlag;
            _core.OnFlagTile += UpdateLeftMines;
            _core.OffFlagTile += _renderer.EraseFlag;
            _core.OffFlagTile += UpdateLeftMines;

            _core.OnOpenTile += (x, y) => _timer.StartTimer();
            _core.OnOpenTile += _renderer.EraseClose;
            _core.OffOpenTile += _renderer.DrawClose;

            _core.OnMineTile += UpdateOpenTileRender;
            _core.OffMineTile += UpdateOpenTileRender;
        }

        Restart();
    }

    private void Update ()
    {
        _timeBoard.text = _timer.TimeToString();
    }
}
