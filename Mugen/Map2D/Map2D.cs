using Microsoft.Xna.Framework;
using Mugen.Core;


namespace Mugen.Map2D
{
    public class Map2D<T> where T : Tile, new()
    {
        public int _mapW { get; private set; }
        public int _mapH { get; private set; }

        public int _tileW { get; private set; }
        public int _tileH { get; private set; }

        List<List<T>> _map2D = new List<List<T>>();

        public Map2D(int mapW, int mapH, int tileW, int tileH)
        {
            _mapW = mapW;
            _mapH = mapH;
            _tileW = tileW;
            _tileH = tileH;

            //Console.WriteLine("--- Map2D will resized !");
            ResizeVecObject2D(_mapW, _mapH);
            //Console.WriteLine("--- Map2D is resized !");

        }

        public List<List<T>> GetMap()
        {
            return _map2D;
        }
        public Map2D<Tile> GetMapTile()
        {
            Map2D<Tile> map2DTile = new Map2D<Tile>(_mapW, _mapH, _tileW, _tileH);

            for (int i = 0; i < _map2D.Count; i++)
            {
                for (int j = 0; j < _map2D[i].Count; j++)
                {
                    map2DTile.Put(i, j, _map2D[i][j]);
                }
            }
            return map2DTile;
        }
        public void ResizeVecObject2D(int mapW, int mapH)
        {
            _mapW = mapW;
            _mapH = mapH;

            //resizeVec<OBJECT>(_vecObject2D,_mapW);
            _map2D.Resize(_mapW, new List<T>());

            for (int i = 0; i < _map2D.Count; i++)
            {
                _map2D[i] = new List<T>();
            }

            for (int x = 0; x < _mapW; ++x)
            {
                //resizeVecPtr<OBJECT>(_vecObject2D[x], _mapH);
                _map2D[x].Resize(_mapH);

                //ListExtra.Resize(_vecObject2D[x], _mapW);

                for (int y = 0; y < _mapH; ++y)
                {
                    _map2D[x][y] = new T();
                }
            }
        }
        public void KillAll()
        {
            for (int x = 0; x < _mapW; ++x)
            {
                for (int y = 0; y < _mapH; ++y)
                {
                    if (null != _map2D[x][y])
                    {
                        //std::cout << "delete at : " << x << " , " << y << " address = "<< _vecOject2D[x][y]<<  " \n";
                        //delete _vecObject2D[x][y];
                        _map2D[x][y] = default!;

                    }
                }
                _map2D[x].Clear();
            }
            _map2D.Clear();
        }
        public void FillObject2D(T tile)
        {
            for (int x = 0; x < _mapW; ++x)
            {
                for (int y = 0; y < _mapH; ++y)
                {
                    Put(x, y, tile);
                }
            }
        }
        public void FillObject2D<Type>() where Type : T, new()
        {
            for (int x = 0; x < _mapW; ++x)
            {
                for (int y = 0; y < _mapH; ++y)
                {
                    Type tile = new Type();
                    Put(x, y, tile);
                }
            }
        }
        public bool IsInMap(int x, int y)
        {
            if (x < 0 || x > _mapW - 1 ||
                y < 0 || y > _mapH - 1)
                return false;

            return true;
        }
        public bool IsInMap(Point point)
        {
            if (point.X < 0 || point.X > _mapW - 1 ||
                point.Y < 0 || point.Y > _mapH - 1)
                return false;

            return true;
        }
        public T? Get(int x, int y)
        {
            if (x < 0 || x > _mapW - 1 ||
                y < 0 || y > _mapH - 1)
                //return default(T);
                return null;
            else
                return _map2D[x][y];
        }
        public void Put(int x, int y, T tile)
        {
            if (x < 0 || x > _mapW - 1 ||
                y < 0 || y > _mapH - 1)
                return;
            //if (null != _map2D[x][y])
            _map2D[x][y] = tile;
        }

        public override string ToString()
        {
            return "[MapW=" + _mapW + ":MapH=" + _mapH + ":TileW=" + _tileW + ":TileH=" + _tileH + "]";
        }

    }
}
