using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }
    public TetrominoData[] tetrominos;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public RectInt Bounds {
        get {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize); 
        }
    }
    
    // Awake() initilizes the gameboard
    private void Awake() {
        Debug.Log("board.Awake");
        this.tilemap = GetComponentInChildren<Tilemap>();
        this.activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < this.tetrominos.Length; i++) {
            this.tetrominos[i].Initialize();
        }
    }

    // Start() starts the tetris game by spawning the first piece
    private void Start() {
        SpawnPiece();
    }

    // SpawnPiece spawns a random tetris piece at the top of the gameboard
    public void SpawnPiece() {
        int random = Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];
        
        this.activePiece.Initialize(this, this.spawnPosition, data);
        if (IsValidPosition(this.activePiece, this.spawnPosition))
        {
            Set(this.activePiece);
        }
        else
        {
            GameOver();
        }
        
    }

    private void GameOver()
    {
        this.tilemap.ClearAllTiles();
    }

    // Clear draws the tiles for the parameterized `piece` onto the gameboard
    public void Set(Piece piece) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            Debug.Log(tilePosition);
            this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }
    
    // Clear resets all of the cells the parameterized `piece` occupies back to null
    public void Clear(Piece piece) {
       for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            Debug.Log(tilePosition);
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    // IsValidPosition verifys that a parameterized `piece` can be translated by the
    // parameterized `position` without going out of gameboards bounds or overlapping
    // with other pieces in the gameboard
    public bool IsValidPosition(Piece piece, Vector3Int position) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + position;
            if (!Bounds.Contains((Vector2Int)tilePosition)) {
                return false;
            }
            if (tilemap.HasTile(tilePosition)) {
                return false;
            }
        }
        return true;
    }

    
    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row)) {
                LineClear(row);
            } else {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position)) {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

}
