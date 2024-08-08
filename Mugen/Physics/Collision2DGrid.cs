using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.Core;
using Mugen.GFX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Physics
{
    public class Collision2DGrid
    {
        float _originX;
        float _originY;

        int _gridW;
        int _gridH;
        int _cellSize;

        List<List<Collide.Cell>> _collideCells = new List<List<Collide.Cell>>();  // Map2D of Cell
        public List<Collide.Zone> _collideZones = new List<Collide.Zone>();   // List of Collide::zone in one Cell : Always Refreshed

        public Collision2DGrid(int gridW, int gridH, int cellSize)
        {
            _cellSize = cellSize;

            _gridW = gridW;
            _gridH = gridH;

            //_vec2dCell.resize(_gridW);
            ListExtra.Resize(_collideCells, _gridW);

            for (int x = 0; x < _gridW; ++x)
            {
                //_vec2dCell[x].resize(_gridH);

                //_vec2dCell.Add(new List<Collide.Cell>());
                _collideCells[x] = new List<Collide.Cell>();

                ListExtra.Resize(_collideCells[x], _gridH);
                for (int y = 0; y < _gridH; ++y)
                {
                    _collideCells[x][y] = new Collide.Cell();
                    //_vec2dCell[x].Add (new Collide.Cell());
                }
            }
        }
        public Collide.Cell? GetCell(int x, int y)
        {
            if (x < 0 || x >= _gridW ||
                y < 0 || y >= _gridH)
                return null;

            return _collideCells[x][y];
        }
        public void ClearAll()
        {
            for (int x = 0; x < _gridW; ++x)
            {
                for (int y = 0; y < _gridH; ++y)
                {
                    if (GetCell(x, y) != null)
                    {
                        if (GetCell(x, y)!._vecCollideZoneInCell.Count > 0)
                        {
                            //foreach (Collide.Zone it in cell(x, y)._vecCollideZoneInCell)
                            //    if (it != null)
                            //    {
                            //        it = null;
                            //    }

                            GetCell(x, y)!._vecCollideZoneInCell.Clear();

                        }
                    }
                }
            }
        }

        public void InsertNodeInCell(int index, RectangleF rect, Node node)
        {
            float x = rect.X - _originX - _cellSize;
            float y = rect.Y - _originY - _cellSize;

            int left = (int)Math.Max(0, x / _cellSize);
            int top = (int)Math.Max(0, y / _cellSize);
            int right = (int)Math.Min((float)_gridW - 1, (x + rect.Width - 1) / _cellSize);
            int bottom = (int)Math.Min((float)_gridH - 1, (y + rect.Height - 1) / _cellSize);

            for (int cx = left; cx <= right; ++cx)
            {
                for (int cy = top; cy <= bottom; ++cy)
                {
                    if (GetCell(cx, cy) != null)
                        GetCell(cx, cy)!._vecCollideZoneInCell.Add(new Collide.Zone(index, rect, node));
                }
            }

        }
        public void InsertZoneInCell(Collide.Zone collideZone)
        {
            float x = collideZone._rect.X - _originX - _cellSize;
            float y = collideZone._rect.Y - _originY - _cellSize;

            int left = (int)Math.Max(0, x / _cellSize);
            int top = (int)Math.Max(0, y / _cellSize);
            int right = (int)Math.Min((float)_gridW - 1, (x + collideZone._rect.Width - 1) / _cellSize);
            int bottom = (int)Math.Min((float)_gridH - 1, (y + collideZone._rect.Height - 1) / _cellSize);

            for (int cx = left; cx <= right; ++cx)
            {
                for (int cy = top; cy <= bottom; ++cy)
                {
                    if (GetCell(cx, cy) != null)
                        GetCell(cx, cy)!._vecCollideZoneInCell.Add
                        (
                            new Collide.Zone
                            (
                            collideZone._index,
                            collideZone._rect,
                            collideZone._node
                            )
                        );
                }
            }
        }

        public List<Collide.Zone> FindNear(List<Collide.Zone> _vecZoneTemp, RectangleF rect)
        {
            float x = rect.X - _originX - _cellSize;
            float y = rect.Y - _originY - _cellSize;

            int left = (int)Math.Max(0, x / _cellSize);
            int top = (int)Math.Max(0, y / _cellSize);
            int right = (int)Math.Min((float)_gridW - 1, (x + rect.Width - 1) / _cellSize);
            int bottom = (int)Math.Min((float)_gridH - 1, (y + rect.Height - 1) / _cellSize);

            for (int cx = left; cx <= right; ++cx)
            {
                for (int cy = top; cy <= bottom; ++cy)
                {
                    if (GetCell(cx, cy) != null)
                        for (int i = 0; i < GetCell(cx, cy)!._vecCollideZoneInCell.Count; ++i)
                        {
                            Collide.Zone collideZone = GetCell(cx, cy)!._vecCollideZoneInCell[i];

                            //if (collideZone->_index != index)
                            _vecZoneTemp.Add(collideZone);
                        }

                }
            }
            return _vecZoneTemp;
        }
        public List<Collide.Zone> FindNear(List<Collide.Zone> _vecZoneTemp, Collide.Zone zoneTest)
        {
            float x = zoneTest._rect.X - _originX - _cellSize;
            float y = zoneTest._rect.Y - _originY - _cellSize;

            int left = (int)Math.Max(0, x / _cellSize);
            int top = (int)Math.Max(0, y / _cellSize);
            int right = (int)Math.Min((float)_gridW - 1, (x + zoneTest._rect.Width - 1) / _cellSize);
            int bottom = (int)Math.Min((float)_gridH - 1, (y + zoneTest._rect.Height - 1) / _cellSize);

            for (int cx = left; cx <= right; ++cx)
            {
                for (int cy = top; cy <= bottom; ++cy)
                {
                    if (GetCell(cx, cy) != null)
                        for (int i = 0; i < GetCell(cx, cy)!._vecCollideZoneInCell.Count; ++i)
                        {
                            _vecZoneTemp.Add(GetCell(cx, cy)!._vecCollideZoneInCell[i]);
                        }

                }
            }

            return _vecZoneTemp;
        }

        public Collide.Zone? GetNearest(RectangleF rect)
        {
            return null;
        }
        public void SetPosition(int x, int y)
        {
            _originX = x;
            _originY = y;
        }

        public void Update()
        {

        }
        public void Render(SpriteBatch batch, SpriteFont font, Color color)
        {
            for (int x = 0; x < _gridW; ++x)
            {
                for (int y = 0; y < _gridH; ++y)
                {

                    float px = x * _cellSize + _originX;
                    float py = y * _cellSize + _originY;


                    GFX.GFX.Rectangle(batch, new RectangleF(.5f + px, .5f + py, .5f + _cellSize, .5f + _cellSize), color, 1);

                    //al_draw_textf(
                    //	font,
                    //	al_map_rgb(250,250,0),
                    //	px + 2, py + 2, 0,
                    //	"%i,%i = %i",
                    //	x, y, cell(x, y)->_vecCollideZoneInCell.size()
                    //);

                    //if (cell(x, y) != null)
                    //{

                    //}
                    if (GetCell(x, y) != null)
                    {
                        batch.DrawString(font, x + "," + y, new Vector2(px + 2, py), color);

                        if (GetCell(x, y)!._vecCollideZoneInCell.Count > 0)
                        {
                            //log("not empty !\n");

                            for (int i = 0; i < GetCell(x, y)!._vecCollideZoneInCell.Count; ++i)
                            {
                                //al_draw_textf(font, al_map_rgb(25, 205, 255),
                                //              px + 2, py + 12 + (i * 12), 0,
                                //              "vec[%i]=%i", i, cell(x, y)->_vecCollideZoneInCell[i]->_index
                                //);
                                batch.DrawString(font, i + "=> " + GetCell(x, y)!._vecCollideZoneInCell[i]._node._index, new Vector2(px + 2, py + 10 + (i * 10)), color);
                            }
                        }

                    }
                }
            }
        }
    }
}
