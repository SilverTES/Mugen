using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.Core;


namespace Mugen.Map2D
{
    public class TileShape
    {
        public Dictionary<string, string> _properties = new Dictionary<string, string>();
        public Vector2[]? _polygonCollision = null;

        public int _id = Const.NoIndex;
        public int _type = Const.NoIndex;
        public int _offsetX = 0;
        public int _offsetY = 0;
        public int _width = 0;
        public int _height = 0;
        public TileShape()
        {

        }
        public TileShape(int id, int type, int offsetX, int offsetY, int width, int height, Vector2[] polygonCollision)
        {
            _id = id;
            _type = type;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _width = width;
            _height = height;
            _polygonCollision = polygonCollision;
        }
        public void AddProperty(string name, string type, string value)
        {
            _properties[name] = type + "=" + value;
        }
    }

    public interface ITile // Need for generic Tile : example -> Astar grid of Tile
    {
        Tile GetTile();
    }

    public class Tile : ITile
    {

        public TileShape? _tileShape = null; // reference of the tileset for this tile

        public Color _color = Color.White; // Color of the render tile
        public float _alpha = 1f; // alpha of the render tile
        public float _layerOpacity = 1f; // opacity of the layer
        public string _strType = ""; // main string type
        public int _id = Const.NoIndex;            // main id or index  
        public int _type = Const.NoIndex;          // main type 
        public int _subType = Const.NoIndex;       // For other type
        public bool _isCollidable = false; // Actor Can collide this
        public int _passLevel { get; private set; } = 0; // Pathfindind Can pass if passLevel > _passLevel
        public int _passLevelInit { get; private set; } = 0; // Original passLevel of the tile ! use for reset tile passLevel !
        public Rectangle _rectSrc = new Rectangle(); // Tile Rect : Get size W & H by this !
        public Texture2D? _texTileSet;
        public Tile()
        {
            _passLevelInit = _passLevel;
        }

        public Tile(int id, int type, bool isCollidable = false, Rectangle rect = new Rectangle(), Texture2D? texTileSet = null, int passLevel = 0)
        {
            _id = id;
            _type = type;
            _isCollidable = isCollidable;
            _rectSrc = rect;
            _passLevel = passLevel;
            _passLevelInit = _passLevel;
            _texTileSet = texTileSet;
        }

        public void SetTexTileSet(Texture2D texTileSet)
        {
            _texTileSet = texTileSet;
        }
        public void SetTileShape(TileShape tileShape)
        {
            _tileShape = tileShape;
        }
        public Tile GetTile()
        {
            return this;
        }

        public void SetPassLevel(int passLevel)
        {
            _passLevel = passLevel;
            _passLevelInit = passLevel;
        }

        public void UpdatePassLevel(int passLevel)
        {
            _passLevel = passLevel;
        }

        public void InitPassLevel()
        {
            _passLevel = _passLevelInit;
        }

        public override string ToString()
        {
            return
                $"strType={_strType} id={_id}\n" +
                $"type={_type} subType={_subType}\n" +
                $"isCollidable={_isCollidable}\n" +
                $"passLevel={_passLevel} passLevelInit={_passLevelInit}";
        }

        public virtual void Render(SpriteBatch batch, Rectangle rectDest, Color color)
        {
            if (null != _texTileSet)
            {
                batch.Draw
                (
                    _texTileSet,
                    rectDest,
                    _rectSrc,
                    color
                );
            }
        }

    }
    //public class TileMap : Tile
    //{
    //    //public Dictionary<int, int> _mapProperty = new Dictionary<int, int>();



    //    public TileMap() : base()
    //    {
    //    }
    //    public TileMap(int id, int type, Rectangle rectSrc = new Rectangle(), bool isCollidable = false) : base()
    //    {
    //        _id = id;
    //        _type = type;
    //        _isCollidable = isCollidable;
    //        _rectSrc = rectSrc;
    //    }

    //    public void SetTileSet(TileShape tileSet)
    //    {
    //        _tileShape = tileSet;
    //    }

    //    public virtual void Render(SpriteBatch batch, Rectangle rectDest, Color color)
    //    {
    //        if (null != _texTileSet)
    //        {
    //            batch.Draw
    //            (
    //                _texTileSet,
    //                rectDest,
    //                _rectSrc,
    //                color
    //            );
    //        }
    //    }
    //}

    public class TileTest : Tile
    {

        //public TileTest() : base()
        //{

        //}
        public override void Render(SpriteBatch batch, Rectangle rectDest, Color color)
        {
            GFX.GFX.FillRectangle(batch, rectDest, color);
        }
    }
}
