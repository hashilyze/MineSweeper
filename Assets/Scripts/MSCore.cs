using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct Tile
{
    [SerializeField] private bool _isBlock;
    private bool _isMine;
    private int _hint;
    private bool _isOpen;
    private bool _isFlag;

    public bool IsBlock
    {
        get => _isBlock;
        set => _isBlock = value;
    }
    public bool IsMine
    {
        get => _isMine;
        set => _isMine = value;
    }
    public int Hint
    {
        get => _hint;
        set => _hint = value;
    }
    public bool IsOpen
    {
        get => _isOpen;
        set => _isOpen = value;
    }
    public bool IsFlag
    {
        get => _isFlag;
        set => _isFlag = value;
    }

}

[System.Serializable]
public class Board
{
    private const int AdjacentRange = 1;

    [SerializeField] private int _classic;
    [SerializeField] private Tile[,] _tiles;
    [SerializeField] private int _defaultMineCount = 0;

    private int _mineCount = 0;
    private int _blockCount = 0;
    private int _openCount = 0;
    private int _flagCount = 0 ;


    public Tile this[int x, int y]
    {
        get => _tiles[x, y];
        set => _tiles[x, y] = value;
    }
    public Board Clone ()
    {
        Board clone = new Board();

        clone._classic = _classic;
        clone._tiles = (Tile[,])_tiles.Clone();
        clone._defaultMineCount = _defaultMineCount;

        return clone;
    }

    public Vector2Int Size => new Vector2Int(_tiles.GetLength(0), _tiles.GetLength(1));

    public int DefaultMineCount
    {
        get => _defaultMineCount;
        set => _defaultMineCount= value;
    }
    public int MineCount => _mineCount;
    public int BlockCount => _blockCount;
    public int OpenCount => _openCount;
    public int FlagCount => _flagCount;

    public bool GetMine (int x, int y) => _tiles[x, y].IsMine;
    public void SetMine(int x, int y, bool isMine)
    {
        if(_tiles[x, y].IsMine ^ isMine)
        {
            if (isMine)
            {
                ++_mineCount;
                ExcuteToAdjacent(x, y, (x, y) => ++_tiles[x, y].Hint);
            }
            else
            {
                --_mineCount;
                ExcuteToAdjacent(x, y, (x, y) => --_tiles[x, y].Hint);
            }
            _tiles[x, y].IsMine = isMine;
        }
    }

    public bool GetBlock (int x, int y) => _tiles[x, y].IsBlock;
    public void SetBlock (int x, int y, bool isBlock)
    {
        if (_tiles[x, y].IsBlock ^ isBlock)
        {
            if (isBlock)
                ++_blockCount;
            else
                --_blockCount;

            _tiles[x, y].IsBlock = isBlock;
        }
    }

    public bool GetOpen (int x, int y) => _tiles[x, y].IsOpen;
    public void SetOpen (int x, int y, bool isOpen)
    {
        if (_tiles[x, y].IsOpen ^ isOpen)
        {
            if (isOpen)
                ++_openCount;
            else
                --_openCount;

            _tiles[x, y].IsOpen = isOpen;
        }
    }

    public bool GetFlag (int x, int y) => _tiles[x, y].IsFlag;
    public void SetFlag (int x, int y, bool isFlag)
    {
        if (_tiles[x, y].IsFlag ^ isFlag)
        {
            if (isFlag)
                ++_flagCount;
            else
                --_flagCount;

            _tiles[x, y].IsFlag = isFlag;
        }
    }

    public int GetHint (int x, int y) => _tiles[x, y].Hint;


    public bool IsOutOfRange (int x, int y)
    {
        return !((0 <= x && x < Size.x) && (0 <= y && y < Size.y));
    }

    public void ExcuteToAdjacent (int x, int y, System.Action<int, int> action)
    {
        for (int y_ad = -AdjacentRange; y_ad <= AdjacentRange; ++y_ad)
        {
            for (int x_ad = -AdjacentRange; x_ad <= AdjacentRange; ++x_ad)
            {
                // Skip self tile
                if (x_ad == 0 && y_ad == 0)
                    continue;
                // Filter adjacent tiles
                int newX = x + x_ad, newY = y + y_ad;
                if (!IsOutOfRange(newX, newY) && !GetBlock(newX, newY))
                {
                    action(newX, newY);
                }
            }
        }
    }

    public int Classic => _classic;
    public void SetClassicBoard()
    {
        _tiles = new Tile[_classic, _classic];
    }

}


public class MSCore : MonoBehaviour
{
    public System.Action OnGameClear;
    public System.Action OnGameOver;

    // ChangedOpen(Close) / ChangedFlag / (ChangedMine / ChangedHint)

    // Call when mine placed or deplaced
    public System.Action<int, int> OnMineTile;
    public System.Action<int, int> OffMineTile;
    // Call when tile opend or closed
    public System.Action<int, int> OnOpenTile;
    public System.Action<int, int> OffOpenTile;
    // Call when flag marked or demarked
    public System.Action<int, int> OnFlagTile;
    public System.Action<int, int> OffFlagTile;

    [SerializeField] private Board _templatedBoard;
    [SerializeField] private Board _board;
    private IEnumerator<int> _randomGenerator;
    private bool _isFirst = false;

    public Board Board => _board;
    public Board TemplateBoard
    {
        get => _templatedBoard;
        set => _templatedBoard = value;
    } 

    public void ResetBoard ()
    {
        if (_templatedBoard.Classic > 0)
        {
            _templatedBoard.SetClassicBoard();
        }

        _board = _templatedBoard.Clone();
    }

    public void GenerateMines ()
    {
        _isFirst = true;
        Vector2Int size = _board.Size;
        int[] tileIndexes = new int[size.x * size.y];
        for (int i = 0; i != tileIndexes.Length; ++i)
        {
            tileIndexes[i] = i;
        }

        _randomGenerator = Shuffle(tileIndexes);
        for(int k = 0; k < _board.DefaultMineCount && _randomGenerator.MoveNext(); ++k)
        {
            int index = _randomGenerator.Current;
            Vector2Int offset = new Vector2Int(index % size.x, index / size.x);
            if(_board.GetBlock(offset.x, offset.y))
            {
                --k;
                continue;
            }
                
            _board.SetMine(offset.x, offset.y, true);
            OnMineTile?.Invoke(offset.x, offset.y);
        }
    }

    public void OpenTile (int x, int y)
    {
        // Skip disopenable tiles
        if (_board.GetBlock(x, y)) return;
        if (_board.GetOpen(x, y)) return;
        if (_board.GetFlag(x, y)) return;

        if (_isFirst)
        {
            FirstTurnRole(x, y);
            _isFirst = false;
        }

        // Game Over
        if(_board.GetMine(x, y))
        {
            OnGameOver();
            return;
        }

        // Open this tile
        _board.SetOpen(x, y, true);
        OnOpenTile?.Invoke(x, y);
        // Game Clear
        if(_board.OpenCount + _board.MineCount == _board.Size.x * _board.Size.y - _board.BlockCount)
        {
            OnGameClear();
            return;
        }

        // Auto open adjacent tiles unless mine
        if (_board.GetHint(x, y) == 0)
        {
            _board.ExcuteToAdjacent(x, y, OpenTile);
        }
    }

    public void ToggleFlagTile (int x, int y)
    {
        // Can not mark on opened tile
        if (_board.GetOpen(x, y)) return;

        // Change the mark (none -> flag -> (question ->) none)
        if (_board.GetFlag(x, y))
        {
            _board.SetFlag(x, y, false);
            OffFlagTile?.Invoke(x, y);
        }
        else
        {
            _board.SetFlag(x, y, true);
            OnFlagTile?.Invoke(x, y);
        }
    }

    private void FirstTurnRole (int x, int y)
    {
        if(_board.GetMine(x, y))
        {
            // Replace mine to avoid first turn game over
            _board.SetMine(x, y, false);
            OffMineTile?.Invoke(x, y);

            if (_randomGenerator.MoveNext())
            {
                int index = _randomGenerator.Current;
                int newX = index % _board.Size.x, newY = index / _board.Size.x;
                _board.SetMine(newX, newY, true);
                OnMineTile?.Invoke(newX, newY);
            }
        }
    }

    private IEnumerator<int> Shuffle (int[] permutation)
    {
        if(permutation.Length == 1)
            yield return permutation[0];

        for (int i = 0; i < permutation.Length; ++i)
        {
            int ranNum = Random.Range(i, permutation.Length);
            Swap(ref permutation[ranNum], ref permutation[i]);
            yield return permutation[i];
        }
    }
    private void Swap<T> (ref T lhs, ref T rhs)
    {
        T tmp = lhs;
        lhs = rhs;
        rhs = tmp;
    }
}