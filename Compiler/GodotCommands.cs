using System;
using System.Collections.Generic;
using Grid;
using System.Drawing;
using System.Numerics;
using Godot;

namespace PixelWallE
{

    public static class GodotCommands
    {
        public static Vector2I _currentPosition;
        public static bool spawngrafico=false;
        public static void ExecuteGodotSpawn(int x, int y)
        {

            GridCanvas.Instance.PlaceTileInGrid(x, y, 7 + 8 * GridTOWalle[512 / GridCanvas.Instance._currentGridSize], new Vector2I(0, 0));
            _currentPosition = new Vector2I(x, y);
            spawngrafico=true;
        }
        public static void ExecuteGodotColor(string color)
        {
            GridCanvas.Instance.PaintColor = ColorNameToColor(color);
            if(spawngrafico)
            GridCanvas.Instance.PlaceTileInGrid(_currentPosition.X, _currentPosition.Y, colortowalle[ColorNameToColor(color)] + 8 * GridTOWalle[512 / GridCanvas.Instance._currentGridSize], new Vector2I(0, 0));
        }
        public static void ExecuteGodotSize(int k)
        {
            GridCanvas.Instance._currentBrushSize = k;
        }
        public static void ExecuteGodotDrawLine(int dirX, int dirY, int distance)
        {
            if (!IsValidDirection(dirX, dirY))
            {
                return;
            }

            Vector2I startPos = _currentPosition;
            int gridSize = 512 / GridCanvas.Instance._currentGridSize;

            Vector2I endPos = new Vector2I(
                startPos.X + dirX * distance,
                startPos.Y + dirY * distance
            );

            endPos.X = Math.Clamp(endPos.X, 0, gridSize - 1);
            endPos.Y = Math.Clamp(endPos.Y, 0, gridSize - 1);
            Vector2I current = startPos;

            PaintWithBrushSize(current.X, current.Y);

            for (int i = 1; i < distance; i++)
            {
                current = new Vector2I(
                    current.X + dirX,
                    current.Y + dirY
                );

                current.X = Math.Clamp(current.X, 0, gridSize - 1);
                current.Y = Math.Clamp(current.Y, 0, gridSize - 1);

                PaintWithBrushSize(current.X, current.Y);
            }

            GridCanvas.Instance.UnPlaceTileInGrid(_currentPosition.X, _currentPosition.Y);
            if(GridCanvas.Instance._currentBrushSize>1)
            {
                _currentPosition=new Vector2I(endPos.X+(dirX*GridCanvas.Instance._currentBrushSize/2), endPos.Y+(dirY*GridCanvas.Instance._currentBrushSize/2));
            }
            else{
            _currentPosition = endPos;
            }
            GridCanvas.Instance.PlaceTileInGrid(
                _currentPosition.X,
                _currentPosition.Y,
                colortowalle[GridCanvas.Instance.PaintColor] + 8 * GridTOWalle[gridSize],
                new Vector2I(0, 0)
            );
        }



        public static void ExecuteGodotDrawCircle(int dirX, int dirY, int radius)
        {
            if (!IsValidDirection(dirX, dirY))
            {
                GD.PrintErr("Error: Invalid direction parameters");
                return;
            }

            // Calcular centro
            Vector2I center = new Vector2I(
                _currentPosition.X + dirX * radius,
                _currentPosition.Y + dirY * radius
            );

            // Ajustar al lienzo
            int canvasSize = 512 / GridCanvas.Instance._currentGridSize;
            center.X = Math.Clamp(center.X, 0, canvasSize - 1);
            center.Y = Math.Clamp(center.Y, 0, canvasSize - 1);

            if (radius <= 10)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    for (int x = -radius; x <= radius; x++)
                    {
                        double distanceSquared = x * x + y * y;
                        double radiusSquared = radius * radius;

                        if (Math.Abs(distanceSquared - radiusSquared) < radius + 0.5)
                        {
                            PaintWithBrushSize(center.X + x, center.Y + y);
                        }
                    }
                }
            }
            else
            {
                int x = 0;
                int y = radius;
                int d = 3 - 2 * radius;

                while (y >= x)
                {
                    PaintCirclePoints(center.X, center.Y, x, y);

                    if (d < 0)
                    {
                        d = d + 4 * x + 6;
                    }
                    else
                    {
                        d = d + 4 * (x - y) + 10;
                        y--;
                    }
                    x++;

                    if (y > x)
                    {
                        PaintCirclePoints(center.X, center.Y, x - 1, y);
                        PaintCirclePoints(center.X, center.Y, x, y - 1);
                    }
                }
            }

            GridCanvas.Instance.UnPlaceTileInGrid(_currentPosition.X, _currentPosition.Y);
            _currentPosition = center;
            GridCanvas.Instance.PlaceTileInGrid(_currentPosition.X, _currentPosition.Y,
                colortowalle[GridCanvas.Instance.PaintColor] + 8 * GridTOWalle[canvasSize],
                new Vector2I(0, 0));
        }

        public static void ExecuteGodotDrawRectangle(int dirX, int dirY, int distance, int width, int height)
        {
            if (!IsValidDirection(dirX, dirY))
            {
                GD.PrintErr("Error: Invalid direction parameters");
                return;
            }

            Vector2I center = new Vector2I(
                _currentPosition.X + dirX * distance,
                _currentPosition.Y + dirY * distance
            );

            center.X = Math.Clamp(center.X, 0, 512 / GridCanvas.Instance._currentGridSize - 1);
            center.Y = Math.Clamp(center.Y, 0, 512 / GridCanvas.Instance._currentGridSize - 1);

            int halfWidth = width / 2;
            int halfHeight = height / 2;

            int left = Math.Clamp(center.X - halfWidth, 0, 512 / GridCanvas.Instance._currentGridSize - 1);
            int right = Math.Clamp(center.X + halfWidth, 0, 512 / GridCanvas.Instance._currentGridSize - 1);
            int top = Math.Clamp(center.Y - halfHeight, 0, 512 / GridCanvas.Instance._currentGridSize - 1);
            int bottom = Math.Clamp(center.Y + halfHeight, 0, 512 / GridCanvas.Instance._currentGridSize - 1);

            for (int x = left; x <= right; x++)
            {
                PaintWithBrushSize(x, top);       
                PaintWithBrushSize(x, bottom);   
            }

            for (int y = top + 1; y < bottom; y++)
            {
                PaintWithBrushSize(left, y);     
                PaintWithBrushSize(right, y);    
            }

            GridCanvas.Instance.UnPlaceTileInGrid(_currentPosition.X, _currentPosition.Y);
            _currentPosition = center;
            GridCanvas.Instance.PlaceTileInGrid(_currentPosition.X, _currentPosition.Y, colortowalle[GridCanvas.Instance.PaintColor] + 8 * GridTOWalle[512 / GridCanvas.Instance._currentGridSize], new Vector2I(0, 0));
        }


        public static void ExecuteGodotFill()

        {
            if (GridCanvas.Instance.PaintColor == Colors.Transparent) return;
            Godot.Color targetColor = GetPixelColor(_currentPosition.X, _currentPosition.Y);
            if (targetColor == GridCanvas.Instance.PaintColor) return;
            // Flood fill algorithm
            Queue<Vector2I> pixelsToCheck = new Queue<Vector2I>();
            HashSet<Vector2I> visited = new HashSet<Vector2I>();
            pixelsToCheck.Enqueue(_currentPosition);
            while (pixelsToCheck.Count > 0)
            {
                Vector2I current = pixelsToCheck.Dequeue();
                if (visited.Contains(current)) continue;
                visited.Add(current);
                if (GetPixelColor(current.X, current.Y) != targetColor) continue;
                  GridCanvas.Instance.PaintGridCell(current.X, current.Y, GridCanvas.Instance.PaintColor);
                // Check adjacent pixels
                if (current.X > 0) pixelsToCheck.Enqueue(new Vector2I(current.X - 1, current.Y));
                if (current.X < 512 / GridCanvas.Instance._currentGridSize - 1) pixelsToCheck.Enqueue(new Vector2I(current.X + 1, current.Y));
                if (current.Y > 0) pixelsToCheck.Enqueue(new Vector2I(current.X, current.Y - 1));
                if (current.Y < 512 / GridCanvas.Instance._currentGridSize - 1) pixelsToCheck.Enqueue(new Vector2I(current.X, current.Y + 1));
            }
        }

        public static int GetActualX() => _currentPosition.X;
        public static int GetActualY() => _currentPosition.Y;
        public static int GetCanvasSize() => 512 / GridCanvas.Instance._currentGridSize;
        public static int GetColorCount(string color, int x1, int y1, int x2, int y2)
        {
            return GridCanvas.Instance.CountColorInArea(color, x1, y1, x2, y2);
        }
        public static int IsBrushColor(string color) => GridCanvas.Instance.PaintColor == ColorNameToColor(color) ? 1 : 0;
        public static int IsBrushSize(int size) => GridCanvas.Instance._currentBrushSize == size ? 1 : 0;
        public static int IsCanvasColor(string color, int vertical, int horizontal)
        {
            int x = _currentPosition.X + horizontal;
            int y = _currentPosition.Y + vertical;

            if (x < 0 || x >= 512 / GridCanvas.Instance._currentGridSize || y < 0 || y >= 512 / GridCanvas.Instance._currentGridSize)
                return 0;

            return GetPixelColor(x, y) == ColorNameToColor(color) ? 1 : 0;
        }













        private static Godot.Color GetPixelColor(int x, int y)
        {
            return GridCanvas.Instance.GetCellColor(x, y);
        }
        private static void PaintWithBrushSize(int x, int y)
        {
            if (GridCanvas.Instance.PaintColor == Colors.Transparent)
                return;

            int halfSize = GridCanvas.Instance._currentBrushSize / 2;

            for (int i = -halfSize; i <= halfSize; i++)
            {
                for (int j = -halfSize; j <= halfSize; j++)
                {
                    int px = x + i;
                    int py = y + j;

                    if (px >= 0 && px < 512 / GridCanvas.Instance._currentGridSize && py >= 0 && py < 512 / GridCanvas.Instance._currentGridSize)
                    {
                        GridCanvas.Instance.PaintGridCell(px, py, GridCanvas.Instance.PaintColor);
                    }
                }
            }
        }
        private static void PaintCirclePoints(int cx, int cy, int x, int y)
        {
            PaintWithBrushSize(cx + x, cy + y);
            PaintWithBrushSize(cx - x, cy + y);
            PaintWithBrushSize(cx + x, cy - y);
            PaintWithBrushSize(cx - x, cy - y);
            PaintWithBrushSize(cx + y, cy + x);
            PaintWithBrushSize(cx - y, cy + x);
            PaintWithBrushSize(cx + y, cy - x);
            PaintWithBrushSize(cx - y, cy - x);
        }
        private static bool IsValidDirection(int dirX, int dirY)
        {
            return dirX >= -1 && dirX <= 1 && dirY >= -1 && dirY <= 1;
        }
        public static Godot.Color ColorNameToColor(string colorName)
        {
            switch (colorName.ToLower())
            {
                case "red": return Colors.Red;
                case "blue": return Colors.Blue;
                case "green": return Colors.Green;
                case "yellow": return Colors.Yellow;
                case "orange": return Colors.Orange;
                case "purple": return Colors.Purple;
                case "black": return Colors.Black;
                case "white": return Colors.White;
                case "transparent": return Colors.Transparent;
                default: return Colors.Transparent;
            }
        }
        public static Dictionary<Godot.Color, int> colortowalle = new Dictionary<Godot.Color, int>()
        {
            {Colors.Black, 0},
            {Colors.Blue, 1},
            {Colors.Green, 2},
            {Colors.Orange, 3},
            {Colors.Purple, 4},
            {Colors.Red, 5},
            {Colors.White, 6},
            {Colors.Yellow, 7},
            {Colors.Transparent, 7}
        };
        public static Dictionary<int, int> GridTOWalle = new Dictionary<int, int>()
        {
            {16, 0},
            {32, 1},
            {64, 2},
            {128, 3},
            {256, 4},
            {512, 5}
        };





    }

}