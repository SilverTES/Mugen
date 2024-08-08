using Mugen.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Physics
{
    public static class Collide
    {
        public class Zone : IClone<Zone>
        {

            public int _index;

            public bool _isCollidable = true; // ----- Optimization  Test collision only if is true !
            public int _collideLevel = 0;  // Can make collide or not by choose Level !
            public RectangleF _rect; // Rect of the Zone !
            public Node _node; // Node who own this CollideZone
            public bool _isCollide = false;
            public Zone? _zoneCollideBy = null;
            public HashSet<Zone> _vecCollideBy = new HashSet<Zone>(); // list of other CollideZone who hit this

            public Zone(int index, RectangleF rect, Node node)
            {
                _index = index;
                _rect = rect;
                _node = node;

                //_vecCollideBy.Clear();
            }

            public Zone Clone()
            {
                Zone clone = (Zone)MemberwiseClone();
                //clone._rect = new RectangleF();
                //clone._vecCollideBy = new HashSet<Zone>();

                //for (int i =0; i< _vecCollideBy.Count;i++)
                //{
                //    clone._vecCollideBy[i] = _vecCollideBy[i];
                //}
                return clone;
            }

        }

        public class Cell
        {
            public List<Zone> _vecCollideZoneInCell = new List<Zone>(); // Contain all index of CollideZone in the Cell
        }
    }
}
