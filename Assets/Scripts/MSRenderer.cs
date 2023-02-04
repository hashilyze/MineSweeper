using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TilePack
{
    public Sprite[] OpenImages;
    public Sprite CloseImage;
    public Sprite BlockImage;
    public Sprite MineImage;
    public Sprite FlagImage;
}

public class MSRenderer : MonoBehaviour
{
    private const string OpenTileName = "OpenTile";
    private const string CloseTileName = "CloseTile";
    private const string MarkTileName = "MarkTile";

    [SerializeField] private MSCore _core;
    [SerializeField] private Camera _camera;

    [SerializeField] TilePack _tilePack;

    private SpriteRenderer[,] _openTiles;
    private SpriteRenderer[,] _closeTiles;
    private SpriteRenderer[,] _markTiles;

    private Transform _openTileGroup;
    private Transform _closeTileGroup;
    private Transform _markTileGroup;


    [ContextMenu("DrawNewBoard")]
    public void DrawNewBoard ()
    {
        // Camera setting
        _camera.orthographicSize = Mathf.Max(_core.Board.Size.x, _core.Board.Size.y) * 0.5f;

        // Destory old objects
        ClearBoard();

        // Scene objects initialize
        {
            transform.position = new Vector2(-_core.Board.Size.x * 0.5f, -_core.Board.Size.y * 0.5f);

            // Generate tile group
            _openTileGroup = new GameObject(OpenTileName).transform;
            _closeTileGroup = new GameObject(CloseTileName).transform;
            _markTileGroup = new GameObject(MarkTileName).transform;

            _openTileGroup.parent
                = _closeTileGroup.parent
                = _markTileGroup.parent
                = transform;

            _openTileGroup.localPosition
                = _closeTileGroup.localPosition
                = _markTileGroup.localPosition
                = Vector3.zero;

            // Generate template
            _openTiles = new SpriteRenderer[_core.Board.Size.x, _core.Board.Size.y];
            _closeTiles = new SpriteRenderer[_core.Board.Size.x, _core.Board.Size.y];
            _markTiles = new SpriteRenderer[_core.Board.Size.x, _core.Board.Size.y];
            for (int y = 0; y < _core.Board.Size.y; ++y)
            {
                for (int x = 0; x < _core.Board.Size.x; ++x)
                {
                    // Under Tile
                    GameObject openTile = new GameObject(OpenTileName);
                    openTile.transform.parent = _openTileGroup;
                    _openTiles[x, y] = openTile.AddComponent<SpriteRenderer>();
                    openTile.transform.localPosition = new Vector3(x, y, 0.0f);
                    // Cover Tile
                    GameObject closeTile = new GameObject(CloseTileName);
                    closeTile.transform.parent = _closeTileGroup;
                    _closeTiles[x, y] = closeTile.AddComponent<SpriteRenderer>();
                    closeTile.transform.localPosition = new Vector3(x, y, -1.0f);
                    // Mark Tile
                    GameObject markTile = new GameObject(MarkTileName);
                    markTile.transform.parent = _markTileGroup;
                    _markTiles[x, y] = markTile.AddComponent<SpriteRenderer>();
                    markTile.transform.localPosition = new Vector3(x, y, -2.0f);
                }
            }
        }

        // Draw tiles
        for (int y = 0; y < _core.Board.Size.y; ++y)
        {
            for (int x = 0; x < _core.Board.Size.x; ++x)
            {
                //Draw close tiles
                DrawClose(x, y);
                //Draw open tiles
                DrawOpen(x, y);
            }
        }
        // * Debug
        //DrawMines();
    }

    public void DrawOpen(int x, int y) 
    {
        if (_core.Board.GetMine(x, y))
        {
            _openTiles[x, y].sprite = _tilePack.MineImage;
        }
        else
        {
            _openTiles[x, y].sprite = _tilePack.OpenImages[_core.Board.GetHint(x, y)];
        }
    }
    public void EraseOpen (int x, int y)
    {
        _openTiles[x, y].sprite = null;
    }

    public void DrawClose(int x, int y)
    {
        _closeTiles[x, y].sprite = _tilePack.CloseImage;
    }
    public void EraseClose (int x, int y)
    {
        _closeTiles[x, y].sprite = null;
    }

    public void DrawFlag (int x, int y)
    {
        _markTiles[x, y].sprite = _tilePack.FlagImage;
    }
    public void EraseFlag (int x, int y)
    {
        _markTiles[x, y].sprite = null;
    }

    public void ClearBoard ()
    {
        if (_openTileGroup != null)
        {
            Destroy(_openTileGroup.gameObject);
            _openTiles = null;
        }
        if (_closeTileGroup != null)
        {
            Destroy(_closeTileGroup.gameObject);
            _closeTiles = null;
        }
        if (_markTileGroup != null)
        {
            Destroy(_markTileGroup.gameObject);
            _markTiles = null;
        }
    }

    public void DrawCloseTiles ()
    {
        for (int y = 0; y < _core.Board.Size.y; ++y)
        {
            for (int x = 0; x < _core.Board.Size.x; ++x)
            {
                //Draw close tiles
                DrawClose(x, y);
            }
        }
    }
    public void DrawBackgroundTiles ()
    {
        for (int y = 0; y < _core.Board.Size.y; ++y)
        {
            for (int x = 0; x < _core.Board.Size.x; ++x)
            {
                DrawOpen(x, y);
            }
        }
    }

    public void ResetBoard ()
    {
        ClearBoard();

        transform.position = new Vector2(-_core.Board.Size.x * 0.5f, -_core.Board.Size.y * 0.5f);

        // Generate tile group
        _openTileGroup = new GameObject(OpenTileName).transform;
        _closeTileGroup = new GameObject(CloseTileName).transform;
        _markTileGroup = new GameObject(MarkTileName).transform;

        _openTileGroup.parent
            = _closeTileGroup.parent
            = _markTileGroup.parent
            = transform;

        _openTileGroup.localPosition
            = _closeTileGroup.localPosition
            = _markTileGroup.localPosition
            = Vector3.zero;

        // Generate template
        _openTiles = new SpriteRenderer[_core.Board.Size.x, _core.Board.Size.y];
        _closeTiles = new SpriteRenderer[_core.Board.Size.x, _core.Board.Size.y];
        _markTiles = new SpriteRenderer[_core.Board.Size.x, _core.Board.Size.y];
        for (int y = 0; y < _core.Board.Size.y; ++y)
        {
            for (int x = 0; x < _core.Board.Size.x; ++x)
            {
                // Under Tile
                GameObject openTile = new GameObject(OpenTileName);
                openTile.transform.parent = _openTileGroup;
                _openTiles[x, y] = openTile.AddComponent<SpriteRenderer>();
                openTile.transform.localPosition = new Vector3(x, y, 0.0f);
                // Cover Tile
                GameObject closeTile = new GameObject(CloseTileName);
                closeTile.transform.parent = _closeTileGroup;
                _closeTiles[x, y] = closeTile.AddComponent<SpriteRenderer>();
                closeTile.transform.localPosition = new Vector3(x, y, -1.0f);
                // Mark Tile
                GameObject markTile = new GameObject(MarkTileName);
                markTile.transform.parent = _markTileGroup;
                _markTiles[x, y] = markTile.AddComponent<SpriteRenderer>();
                markTile.transform.localPosition = new Vector3(x, y, -2.0f);
            }
        }
    }
}
