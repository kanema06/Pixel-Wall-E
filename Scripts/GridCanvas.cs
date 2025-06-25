using Godot;
using System;
using System.Collections.Generic;
using PixelWallE;

namespace Grid
{
public partial class GridCanvas : TileMapLayer
{
    private static GridCanvas _instance;
    public static GridCanvas Instance => _instance;
    [Export] public Rect2 GridArea = new Rect2(0, 0, 512, 512);
    [Export] public int DefaultGridSize = 16;
    [Export] public Color GridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    [Export] public Color PaintColor = Colors.Transparent;
    [Export] public int _currentBrushSize = 1;
    
    public int _currentGridSize;
    
    private Dictionary<Vector2I, Color> _paintedCells = new Dictionary<Vector2I, Color>();
    private Vector2I _gridDimensions;
    
     public override void _Ready()
    {
        _instance = this;
        var optionButton = GetNode<OptionButton>("Panel2/OptionButton");
        _currentGridSize = 32; 
        UpdateGridDimensions();
        QueueRedraw(); 
        
    }
    public override void _Process(double delta)
	{
       
	}
        public void _on_option_button_item_selected(long index)
    {
        UnPlaceTileInGrid(GodotCommands._currentPosition.X, GodotCommands._currentPosition.Y);
        switch(index)
        {
            case 0: _currentGridSize = 32; break;
            case 1: _currentGridSize = 16; break;
            case 2: _currentGridSize = 8; break;
            case 3: _currentGridSize = 4; break;
            case 4: _currentGridSize = 2; break;
            case 5: _currentGridSize = 1; break;
        }

        UpdateGridDimensions();
        QueueRedraw();
        ResetStats();
    }
    private void UpdateGridDimensions()
    {
        _gridDimensions = new Vector2I(
            (int)(GridArea.Size.X / _currentGridSize),
            (int)(GridArea.Size.Y / _currentGridSize));
    }

    public override void _Draw()
    {
        for (int x = 0; x <= _gridDimensions.X; x++)
        {
            var start = new Vector2(GridArea.Position.X + x * _currentGridSize, GridArea.Position.Y);
            var end = new Vector2(GridArea.Position.X + x * _currentGridSize, GridArea.Position.Y + GridArea.Size.Y);
            DrawLine(start, end, GridColor);
        }

        for (int y = 0; y <= _gridDimensions.Y; y++)
        {
            var start = new Vector2(GridArea.Position.X, GridArea.Position.Y + y * _currentGridSize);
            var end = new Vector2(GridArea.Position.X + GridArea.Size.X, GridArea.Position.Y + y * _currentGridSize);
            DrawLine(start, end, GridColor);
        }

        foreach (var cell in _paintedCells)
        {
            var pos = new Vector2(
                GridArea.Position.X + cell.Key.X * _currentGridSize,
                GridArea.Position.Y + cell.Key.Y * _currentGridSize);
            
            var rect = new Rect2(pos, new Vector2(_currentGridSize, _currentGridSize));
            DrawRect(rect, cell.Value, true);
        }
    }

    public void PaintGridCell(int x, int y, Color color)
    {
        if (x < 0 || y < 0 || x >= _gridDimensions.X || y >= _gridDimensions.Y)

        {
            GD.PrintErr($"Coordenadas fuera de rango: ({x}, {y})");
            return;
        }

        _paintedCells[new Vector2I(x, y)] = color;
         GD.PrintErr($"(coords ({x}, {y})");
        QueueRedraw();
    }

  public void PlaceTileInGrid(int gridX, int gridY, int atlas, Vector2I tileCoord)
{
    Vector2 pixelPos = new Vector2(
        (GridArea.Position.X + gridX * _currentGridSize)+_currentGridSize/2,
    (GridArea.Position.Y + gridY * _currentGridSize)+_currentGridSize/2
    );


    Vector2I tilePos = new Vector2I(
        (int)(pixelPos.X / TileSet.TileSize.X),
        (int)(pixelPos.Y / TileSet.TileSize.Y)
    );

    SetCell(tilePos, atlas, tileCoord, 0);

}
public void UnPlaceTileInGrid(int gridX, int gridY)
{
    Vector2 pixelPos = new Vector2(
        (GridArea.Position.X + gridX * _currentGridSize)+_currentGridSize/2,
    (GridArea.Position.Y + gridY * _currentGridSize)+_currentGridSize/2
    );

    Vector2I tilePos = new Vector2I(
        (int)(pixelPos.X / TileSet.TileSize.X),
        (int)(pixelPos.Y / TileSet.TileSize.Y)
    );
    SetCell(tilePos, -1, new Vector2I(0,0)); 
}
   
    public void ClearAllTiles()
    {
        Clear();
        _paintedCells.Clear();
        QueueRedraw();
    }
    public void ResetStats()
    {
        ClearAllTiles();
        GodotCommands._currentPosition=new Vector2I(-1, -1);
        GodotCommands.spawngrafico=false;
        _currentBrushSize=1;
        PaintColor=Colors.Transparent;

    }
    public Color GetCellColor(int x, int y)
{
    Vector2I key = new Vector2I(x, y);
    if (_paintedCells.TryGetValue(key, out Color color))
    {
        return color;
    }
    return Colors.Transparent; 
}  
public int CountColorInArea(string colorName, int x1, int y1, int x2, int y2)
{
    Color targetColor = GodotCommands.ColorNameToColor(colorName);
    int count = 0;
    
    int minX = Math.Min(x1, x2);
    int maxX = Math.Max(x1, x2);
    int minY = Math.Min(y1, y2);
    int maxY = Math.Max(y1, y2);
    
    minX = Math.Clamp(minX, 0, _gridDimensions.X - 1);
    maxX = Math.Clamp(maxX, 0, _gridDimensions.X - 1);
    minY = Math.Clamp(minY, 0, _gridDimensions.Y - 1);
    maxY = Math.Clamp(maxY, 0, _gridDimensions.Y - 1);
    
    for (int x = minX; x <= maxX; x++)
    {
        for (int y = minY; y <= maxY; y++)
        {
            if (GetCellColor(x, y) == targetColor)
            {
                count++;
            }
        }
    }
    
    return count;
}
}
}