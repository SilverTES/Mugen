using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.Core;
using Mugen.GFX;
using TiledSharp;

namespace Mugen.Map2D
{

    public class TileMapLayer<T> where T : Tile, new()
    {
        public Map2D<T>? _map2D;
        public Dictionary<int, TileShape> _tileShapes = new Dictionary<int, TileShape>();
        public TileMapLayer()
        {

        }
    }


    public class TileMap2D<TileType> where TileType : Tile, new()
    {
        public class TiledProperties
        {
            bool _mapLoaded = false;
            string _jsonFileName = "";
            int _idLayer = -1;
            int _idTileSet = -1;
            Dictionary<int, string> _mapTiledProperty = new Dictionary<int, string>();

            public void LoadTiledJSON // Create tiles and their properties in _map2D !
            (
                String jsonFileName,
                int idLayer,
                int idTileSet,
                String ileSetName,
                //Asset::Manager* asset,
                String path,
                Dictionary<int, string> mapTiledProperty
            )
            {
                //if (nullptr == asset)
                //{
                //    mlog("- Error loadTiledJson : asset == nullptr", 1);
                //    return _clip;
                //}

                _jsonFileName = jsonFileName;
                _idLayer = idLayer;
                _idTileSet = idTileSet;
                _mapTiledProperty = mapTiledProperty;

                //Json mapTiledJSON = File::loadJson(_jsonFileName);

                //std::string tileSetFileName = mapTiledJSON["tilesets"][idTileSet]["image"];

                //asset->add(new Asset::Bitmap(tileSetName, (path + tileSetFileName).c_str()));

                //setAtlas(asset->GET_BITMAP(tileSetName));

                //int tw = mapTiledJSON["tilesets"][idTileSet]["tilewidth"];
                //int th = mapTiledJSON["tilesets"][idTileSet]["tileheight"];
                //unsigned mw = mapTiledJSON["layers"][idLayer]["width"];
                //unsigned mh = mapTiledJSON["layers"][idLayer]["height"];
                //int nbCol = mapTiledJSON["tilesets"][idTileSet]["columns"];


                //setTileSize(tw, th);
                //setMapSize(mw, mh);


                //for (unsigned y = 0; y < mh; ++y)
                //{
                //    for (unsigned x = 0; x < mw; ++x)
                //    {
                //        int id = mapTiledJSON["layers"][idLayer]["data"][x + y * mw];
                //        int firstgid = mapTiledJSON["tilesets"][idTileSet]["firstgid"];
                //        id = id - firstgid;

                //        if (id != 0)
                //        {
                //            int my = round(id / nbCol);
                //            int mx = id - my * nbCol;

                //            VAR tx = mx * tw;
                //            VAR ty = my * th;

                //            Tile* tile = new Tile(0, id, Rect{ tx, ty, (VAR)tw, (VAR)th });

                //            if (nullptr != mapTiledJSON["tilesets"][idTileSet]["tileproperties"][std::to_string(id)]["isCollidable"])
                //            {
                //                int isCollidable = mapTiledJSON["tilesets"][idTileSet]["tileproperties"][std::to_string(id)]["isCollidable"];
                //                tile->_isCollidable = isCollidable;
                //            }

                //            // Iterate all tiles property !
                //            auto it = mapTiledProperty.begin();
                //            while (it != mapTiledProperty.end())
                //            {
                //                if (nullptr != mapTiledJSON["tilesets"][idTileSet]["tileproperties"][std::to_string(id)][mapTiledProperty[it->first]])
                //                    tile->_mapProperty[it->first] = mapTiledJSON["tilesets"][idTileSet]["tileproperties"][std::to_string(id)][mapTiledProperty[it->first]];
                //                ++it;
                //            }

                //            setTile(x, y, tile);
                //        }
                //    }
                //}

                _mapLoaded = true;

            }

            public void ReLoadTileProperty() // Reload only tile properties, don't recreate tile in _map2D !
            {
                if (_mapLoaded)
                {
                    //Json mapTiledJSON = File::loadJson(_jsonFileName);

                    //unsigned mw = mapTiledJSON["layers"][_idLayer]["width"];
                    //unsigned mh = mapTiledJSON["layers"][_idLayer]["height"];

                    //for (unsigned y = 0; y < mh; ++y)
                    //{
                    //    for (unsigned x = 0; x < mw; ++x)
                    //    {
                    //        int id = mapTiledJSON["layers"][_idLayer]["data"][x + y * mw];
                    //        int firstgid = mapTiledJSON["tilesets"][_idTileSet]["firstgid"];
                    //        id = id - firstgid;

                    //        if (id != 0)
                    //        {
                    //            Tile* tile = getTile(x, y);

                    //            if (nullptr != mapTiledJSON["tilesets"][_idTileSet]["tileproperties"][std::to_string(id)]["isCollidable"])
                    //            {
                    //                int isCollidable = mapTiledJSON["tilesets"][_idTileSet]["tileproperties"][std::to_string(id)]["isCollidable"];
                    //                tile->_isCollidable = isCollidable;
                    //            }

                    //            // Iterate all tiles property !
                    //            auto it = _mapTiledProperty.begin();
                    //            while (it != _mapTiledProperty.end())
                    //            {
                    //                if (nullptr != mapTiledJSON["tilesets"][_idTileSet]["tileproperties"][std::to_string(id)][_mapTiledProperty[it->first]])
                    //                    tile->_mapProperty[it->first] = mapTiledJSON["tilesets"][_idTileSet]["tileproperties"][std::to_string(id)][_mapTiledProperty[it->first]];
                    //                ++it;
                    //            }
                    //        }
                    //    }
                    //}

                }
            }
        }

        #region Attributes

        int _tileW;
        int _tileH;
        int _mapW;
        int _mapH;

        Rectangle _rect; // rect of the TileMap2D
        Rectangle _rectView; // rect of the View

        //List<Map2D<TileMap>> _layers = new List<Map2D<TileMap>>();
        //List<Texture2D> _tileSets = new List<Texture2D>();

        //List<TileMapLayer<TileType>> _layers = new List<TileMapLayer<TileType>>();

        //public int LayerCount => _layers.Count;

        #endregion

        public static TileMapLayer<TileType> CreateLayer(TmxMap tmxMap, TmxLayer tmxLayer, Texture2D? texTileSet = null)
        {

            int tileW = tmxMap.Tilesets[0].TileWidth;
            int tileH = tmxMap.Tilesets[0].TileHeight;

            TileMapLayer<TileType> layer = new TileMapLayer<TileType>();

            layer._map2D = new Map2D<TileType>(tmxMap.Width, tmxMap.Height, tileW, tileH);
            //layer._map2D.FillObject2D<TileMap>(); // Fill Map2D

            int nbTileRow = tmxMap.Tilesets[0].Columns!.Value;


            // Read all available tileset and stock in dictionnary tileset
            if (tmxMap.Tilesets[0].Tiles.Count > 0)
                for (int i = 0; i < tmxMap.Tilesets[0].Tiles.Count; ++i)
                {
                    if (tmxMap.Tilesets[0].Tiles.ContainsKey(i))
                    {
                        var tileset = tmxMap.Tilesets[0].Tiles[i];

                        // Parse Type if integer
                        bool successParse = int.TryParse(tileset.Type, out int type);
                        if (!successParse) type = Const.NoIndex;

                        int offsetX = 0;
                        int offsetY = 0;

                        Vector2[]? polygon = null;

                        if (tileset.ObjectGroups.Count > 0)
                        {
                            if (tileset.ObjectGroups[0].Objects.Count > 0)
                            {
                                int nbPoints = tileset.ObjectGroups[0].Objects[0].Points.Count + 1;


                                // Read Polygon Collision
                                if (tileset.ObjectGroups[0].Objects[0].Points.Count > 0)
                                {

                                    // Origin of the first vertex of the polygon
                                    offsetX = (int)tileset.ObjectGroups[0].Objects[0].X;
                                    offsetY = (int)tileset.ObjectGroups[0].Objects[0].Y;

                                    int index = 0; // index polygon !
                                    polygon = new Vector2[nbPoints];
                                    foreach (var point in tileset.ObjectGroups[0].Objects[0].Points)
                                    {
                                        polygon[index] = new Vector2(offsetX + (float)point.X, offsetY + (float)point.Y);
                                        ++index;
                                    }
                                    polygon[nbPoints - 1] = polygon[0];
                                }

                            }

                        }

                        TileShape globalTileSet = new TileShape(tileset.Id, type, offsetX, offsetY, tileW, tileH, polygon!);
                        // Add global property of this tileset : iterate tileSet custom properties
                        //for(int p=0; p<tmxMap.Tilesets[0].Tiles[i].Properties.Count; ++p)
                        foreach (var property in tmxMap.Tilesets[0].Tiles[i].Properties)
                        {
                            //var property = tmxMap.Tilesets[0].Tiles[i].Properties[p].;

                            globalTileSet.AddProperty(property.Key.ToString(), property.GetType().ToString(), property.Value.ToString());
                            Console.WriteLine("Property = " + property);
                        }


                        layer._tileShapes.Add(tileset.Id, globalTileSet);

                        Console.WriteLine($"Save tile set : {tileset.Id}");
                    }
                }


            //for (int layerIndex = 0; layerIndex < tmxMap.Layers.Count; layerIndex++)
            {
                int row = 0;
                int line = 0;

                float layerOpacity = (float)tmxLayer.Opacity;

                // Iterate all tiles of the TmxLayer and put tile in proper location !
                for (int i = 0; i < tmxLayer.Tiles.Count; i++)
                {
                    int gid = tmxLayer.Tiles[i].Gid;

                    //Console.Write(gid+",");

                    if (gid > 0)
                    {
                        //Console.Write("X");

                        int tileSetId = gid - 1;

                        int tilesetX = tileSetId % nbTileRow;
                        int tilesetY = (int)Math.Floor(tileSetId / (double)nbTileRow);

                        Rectangle tilsetRect = new Rectangle(tilesetX * tileW, tilesetY * tileH, tileW, tileH);

                        //float x = row * _tileW;
                        //float y = line * _tileH;
                        //_batch.Draw(tileSet, new Vector2(x, y), tilsetRect, Color.White * layerOpacity);

                        TileType tile = new TileType();
                        tile._id = tileSetId;
                        tile._type = 0;
                        tile._rectSrc = tilsetRect;
                        tile._texTileSet = texTileSet;
                        tile._layerOpacity = layerOpacity;


                        //int type;
                        tile._strType = "";

                        if (tmxMap.Tilesets[0].Tiles.ContainsKey(tileSetId))
                        {

                            tile._strType = tmxMap.Tilesets[0].Tiles[tileSetId].Type;

                            //Console.WriteLine(tileMap._strType);

                            bool successParse = int.TryParse(tile._strType, out int type);

                            if (successParse)
                            {
                                tile._type = type;

                                if (type > 0)
                                {
                                    if (type > 1)
                                        tile._isCollidable = true;
                                }
                            }

                            // Read Pass Level from tileset !
                            if (tmxMap.Tilesets[0].Tiles[tileSetId].Properties.ContainsKey("passLevel"))
                            {
                                int passLevel = int.Parse(tmxMap.Tilesets[0].Tiles[tileSetId].Properties["passLevel"]);

                                tile.SetPassLevel(passLevel);
                            }

                        }

                        if (layer._tileShapes.ContainsKey(tileSetId))
                            tile.SetTileShape(layer._tileShapes[tileSetId]);

                        layer._map2D.Put(row, line, tile);
                    }
                    else
                    {
                        layer._map2D.Put(row, line, new TileType());
                    }

                    row++;
                    if (row == tmxMap.Width)
                    {
                        row = 0;
                        line++;
                        //Console.WriteLine();
                    }
                }
            }
            return layer;
        }
        public TileMapLayer<TileType> CreateLayer()
        {

            Map2D<TileType> map2D = new Map2D<TileType>(_mapW, _mapH, _tileW, _tileH);

            //map2D.FillObject2D<TileMap>();

            TileMapLayer<TileType> layer = new TileMapLayer<TileType>() { _map2D = map2D };
            //_layers.Add(layer);
            //_tileSets.Add(tileSet);

            return layer;
            //return _layerMap2D.Count - 1;
        }
        [Obsolete]
        public TileMap2D<TileType> LoadFromTmxMap(TmxMap tmxMap, TmxLayer tmxLayer, TileMapLayer<TileType> layer, Texture2D texTileSet)
        {

            int _tileW = tmxMap.Tilesets[0].TileWidth;
            int _tileH = tmxMap.Tilesets[0].TileHeight;

            int nbTileRow = texTileSet.Width / _tileW;
            int nbTileLine = texTileSet.Height / _tileH;

            //for (int layerIndex = 0; layerIndex < tmxMap.Layers.Count; layerIndex++)
            {
                int row = 0;
                int line = 0;

                float layerOpacity = (float)tmxLayer.Opacity;

                for (int i = 0; i < tmxLayer.Tiles.Count; i++)
                {
                    int gid = tmxLayer.Tiles[i].Gid;

                    if (gid != 0)
                    {
                        int tilesetId = gid - 1;

                        int tilesetX = tilesetId % nbTileRow;
                        int tilesetY = (int)Math.Floor((double)tilesetId / (double)nbTileLine);

                        Rectangle tilesetRect = new Rectangle(tilesetX * _tileW, tilesetY * _tileH, _tileW, _tileH);

                        //float x = row * _tileW;
                        //float y = line * _tileH;
                        //_batch.Draw(tileSet, new Vector2(x, y), tilsetRect, Color.White * layerOpacity);

                        TileType tile = new TileType();
                        tile._id = tilesetId;
                        tile._type = 0;
                        tile._isCollidable = false;
                        tile._rectSrc = tilesetRect;
                        tile._layerOpacity = layerOpacity;

                        if (int.Parse(tmxMap.Tilesets[0].Tiles[tilesetId].Type) > 0)
                            tile._isCollidable = true;

                        layer._map2D!.Put(row, line, tile);
                    }

                    row++;
                    if (row == tmxMap.Width)
                    {
                        row = 0;
                        line++;
                    }
                }
            }
            return this;
        }
        public TileMap2D<TileType> Setup(Rectangle rectView, int mapW = 1, int mapH = 1, int tileW = 1, int tileH = 1, int x = 0, int y = 0)
        {
            _tileW = tileW;
            _tileH = tileH;
            _mapW = mapW;
            _mapH = mapH;

            _rect.X = x;
            _rect.Y = y;
            _rect.Width = mapW * tileW;
            _rect.Height = mapH * tileH;
            _rectView = rectView;

            //_map2D = new Map2D<Tile>(mapW, mapH);

            return this;
        }
        public TileMap2D<TileType> SetRectView(Rectangle rectView)
        {
            _rectView = rectView;
            return this;
        }
        public TileMap2D<TileType> SetMapSize(int mapW, int mapH)
        {
            _mapW = mapW;
            _mapH = mapH;
            return this;
        }
        public TileMap2D<TileType> SetTileSize(int tileW, int tileH)
        {
            _tileW = tileW;
            _tileH = tileH;
            return this;
        }
        public TileMap2D<TileType> SetPosition(int x, int y)
        {
            _rect.X = x;
            _rect.Y = y;
            return this;
        }

        public Rectangle GetTileRect(int x, int y, int extend = 0)
        {
            Rectangle rect = new Rectangle
            (
                _tileW * x - extend,
                _tileH * y - extend,
                _tileW + extend * 2,
                _tileH + extend * 2
            );

            return rect;
        }
        public void ShowGrid(SpriteBatch batch, Color color)
        {
            if (Node._showNodeInfo)
                GFX.GFX.Grid
                (
                    batch,
                    _rect.X,
                    _rect.Y,
                    _rect.Width,
                    _rect.Height,
                    _tileW,
                    _tileH,
                    color
                );
        }
        public void ShowView(SpriteBatch batch, Color color)
        {
            if (Node._showNodeInfo)
                GFX.GFX.Rectangle(batch, _rectView, color);
        }
        public void ShowInfo(SpriteBatch batch, TileMapLayer<TileType> layer, SpriteFont font, Color color)
        {
            if (Node._showNodeInfo)
            {
                //if (Array.IsExist(_layers, indexLayer))

                for (int x = 0; x < layer._map2D!._mapW; ++x)
                {
                    if (x * _tileW + _rect.X < _rectView.X - _tileW || x * _tileW + _rect.X > _rectView.X + _rectView.Width)
                        continue;

                    for (int y = 0; y < layer._map2D._mapH; ++y)
                    {

                        if (y * _tileH + _rect.Y < _rectView.Y - _tileH || y * _tileH + _rect.Y > _rectView.Y + _rectView.Height)
                            continue;

                        Tile tile = layer._map2D.Get(x, y)!;

                        if (null != tile)
                        {
                            //Draw::rect(Rect{x*_tileW+_rect._x, y*_tileH+_rect._y, _tileW, _tileH}, al_map_rgb(150,106,150),0);

                            //if (tile._rect.Width < 1 || tile._rect.Height < 1)
                            //    continue;

                            //Draw::rect(Rect{x*_tileW+_rect._x, y*_tileH+_rect._y, _tileW, _tileH}, al_map_rgb(150,106,150),0);

                            batch.DrawString
                            (
                                font,
                                tile._type + ":" + (tile._isCollidable ? 1 : 0),
                                new Vector2(x * _tileW + 4 + _rect.X, y * _tileH + 2 + _rect.Y),
                                color
                            );

                            batch.DrawString
                            (
                                font,
                                tile._subType + " " + tile._passLevel,
                                new Vector2(x * _tileW + 4 + _rect.X, y * _tileH + 12 + _rect.Y),
                                color
                            );

                        }
                    }
                }
            }
        }
        public void Render(SpriteBatch batch, TileMapLayer<TileType> layer, int absX = 0, int absY = 0, float opacity = 1f)
        {
            _rect.X = absX;
            _rect.Y = absY;

            if (null != layer)
            {
                int beginX = (_rectView.X - _tileW - _rect.X) / _tileW;
                int endX = (_rectView.X + _rectView.Width - _rect.X) / _tileW + 1;

                int beginY = (_rectView.Y - _tileH - _rect.Y) / _tileH;
                int endY = (_rectView.Y + _rectView.Height - _rect.Y) / _tileH + 1;

                for (int x = beginX; x < endX; ++x)
                //for (int x = 0; x < layer._map2D._mapW; ++x)
                {
                    //if (x * _tileW + _rect.X < _rectView.X - _tileW || x * _tileW + _rect.X > _rectView.X + _rectView.Width) continue;

                    for (int y = beginY; y < endY; ++y)
                    //for (int y = 0; y < layer._map2D._mapH; ++y)
                    {

                        //if (y * _tileH + _rect.Y < _rectView.Y - _tileH || y * _tileH + _rect.Y > _rectView.Y + _rectView.Height) continue;

                        Tile tile = layer._map2D!.Get(x, y)!;

                        //batch.DrawCircle
                        //(
                        //    (int)Math.Floor(x * _tileW + _rect.X) + _tileW/2, 
                        //    (int)Math.Floor(y * _tileH + _rect.Y) + _tileH/2, 
                        //    8, 8, 
                        //    Color.Violet
                        //);

                        if (null != tile)
                        {


                            if (tile._rectSrc.Width < 1 || tile._rectSrc.Height < 1)
                                continue;

                            //batch.Draw
                            //(
                            //    _atlas, 
                            //    new Rectangle
                            //    (
                            //        (int)Math.Floor(x * _tileW + _rect.X), (int)Math.Floor(y * _tileH + _rect.Y), _tileW, _tileH
                            //    ), 
                            //    tile._rect, 
                            //    color
                            //);


                            tile.Render(
                                batch,
                                new Rectangle
                                (
                                    x * _tileW + _rect.X, y * _tileH + _rect.Y, _tileW, _tileH
                                ),
                                    tile._color * tile._alpha * tile._layerOpacity * opacity
                                );

                        }

                    }
                }

            }

        }

        public void RenderPolygonCollision(SpriteBatch batch, TileMapLayer<TileType> layer, Color color, float opacity = 1f, int absX = 0, int absY = 0, float thickness = 1f, Color colorFirstLine = default, float thicknessFirstLine = 1f)
        {
            _rect.X = absX;
            _rect.Y = absY;

            if (null != layer)
            {
                int beginX = (_rectView.X - _tileW - _rect.X) / _tileW;
                int endX = (_rectView.X + _rectView.Width - _rect.X) / _tileW + 1;

                int beginY = (_rectView.Y - _tileH - _rect.Y) / _tileH;
                int endY = (_rectView.Y + _rectView.Height - _rect.Y) / _tileH + 1;

                for (int x = beginX; x < endX; ++x)
                //for (int x = 0; x < layer._map2D._mapW; ++x)
                {
                    //if (x * _tileW + _rect.X < _rectView.X - _tileW || x * _tileW + _rect.X > _rectView.X + _rectView.Width) continue;

                    for (int y = beginY; y < endY; ++y)
                    //for (int y = 0; y < layer._map2D._mapH; ++y)
                    {

                        //if (y * _tileH + _rect.Y < _rectView.Y - _tileH || y * _tileH + _rect.Y > _rectView.Y + _rectView.Height) continue;

                        Tile tile = layer._map2D!.Get(x, y)!;

                        if (null != tile)
                        {

                            if (tile._rectSrc.Width < 1 || tile._rectSrc.Height < 1)
                                continue;


                            if (layer._tileShapes.ContainsKey(tile._id))
                            {
                                Vector2 offset = new Vector2(x * _tileW + _rect.X, y * _tileH + _rect.Y);

                                if (null != layer._tileShapes[tile._id]._polygonCollision)
                                {
                                    GFX.GFX.PolyLine(batch, layer._tileShapes[tile._id]._polygonCollision!, color * opacity, thickness, offset);


                                    if (colorFirstLine != default)
                                    {
                                        //Draw first Line
                                        GFX.GFX.Line(batch,
                                            layer._tileShapes[tile._id]._polygonCollision![0] + offset,
                                            layer._tileShapes[tile._id]._polygonCollision![1] + offset,
                                            colorFirstLine * opacity, thicknessFirstLine);

                                        //Draw last Line
                                        GFX.GFX.Line(batch,
                                            layer._tileShapes[tile._id]._polygonCollision![layer._tileShapes[tile._id]._polygonCollision!.Length - 2] + offset,
                                            layer._tileShapes[tile._id]._polygonCollision![layer._tileShapes[tile._id]._polygonCollision!.Length - 1] + offset,
                                            colorFirstLine * opacity, thicknessFirstLine);

                                    }


                                }
                            }

                        }

                    }
                }

            }

        }
    }
}
