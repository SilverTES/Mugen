using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.Core;
using Mugen.GFX;

namespace Mugen.Physics
{
    public class Collision2DQuadtree
    {
        private int MaxNodes = 10;
        private RectangleF _bounds;
        private List<Collide.Zone> _collideZones;
        private Collision2DQuadtree[] _children;

        public Collision2DQuadtree(RectangleF bounds, int maxNodes = 10)
        {
            _bounds = bounds;
            _collideZones = new List<Collide.Zone>();
            _children = new Collision2DQuadtree[4];
            MaxNodes = maxNodes;
        }
        public void SetMaxNodes(int maxNodes)
        {
            MaxNodes = maxNodes;
        }
        private void Subdivide()
        {
            float subWidth = _bounds.Width / 2;
            float subHeight = _bounds.Height / 2;
            float x = _bounds.X;
            float y = _bounds.Y;

            _children[0] = new Collision2DQuadtree(new RectangleF(x, y, subWidth, subHeight));
            _children[1] = new Collision2DQuadtree(new RectangleF(x + subWidth, y, subWidth, subHeight));
            _children[2] = new Collision2DQuadtree(new RectangleF(x, y + subHeight, subWidth, subHeight));
            _children[3] = new Collision2DQuadtree(new RectangleF(x + subWidth, y + subHeight, subWidth, subHeight));
        }

        private int GetIndex(RectangleF rect)
        {
            int index = -1;
            double verticalMidpoint = _bounds.X + (_bounds.Width / 2);
            double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);

            bool topQuadrant = (rect.Y < horizontalMidpoint && rect.Y + rect.Height < horizontalMidpoint);
            bool bottomQuadrant = (rect.Y > horizontalMidpoint);
            bool leftQuadrant = (rect.X < verticalMidpoint && rect.X + rect.Width < verticalMidpoint);
            bool rightQuadrant = (rect.X > verticalMidpoint);

            if (topQuadrant && leftQuadrant)
            {
                index = 0;
            }
            else if (topQuadrant && rightQuadrant)
            {
                index = 1;
            }
            else if (bottomQuadrant && leftQuadrant)
            {
                index = 2;
            }
            else if (bottomQuadrant && rightQuadrant)
            {
                index = 3;
            }

            return index;
        }

        public void Insert(Collide.Zone entry)
        {
            if (_children[0] != null)
            {
                int index = GetIndex(entry._rect);

                if (index != -1)
                {
                    _children[index].Insert(entry);
                    return;
                }
            }

            _collideZones.Add(entry);

            if (_collideZones.Count > MaxNodes && _children[0] == null)
            {
                Subdivide();

                for (int i = _collideZones.Count - 1; i >= 0; i--)
                {
                    int index = GetIndex(_collideZones[i]._rect);
                    if (index != -1)
                    {
                        _children[index].Insert(_collideZones[i]);
                        _collideZones.RemoveAt(i);
                    }
                }
            }
        }

        public List<Collide.Zone> Retrieve(List<Collide.Zone> returnObjects, RectangleF rect)
        {
            int index = GetIndex(rect);
            if (_children[0] != null && index != -1)
            {
                _children[index].Retrieve(returnObjects, rect);
            }

            for (int i = 0; i < _collideZones.Count; i++)
            {
                returnObjects.Add(_collideZones[i]);
            }

            return returnObjects.Distinct().ToList();
        }

        public void Clear()
        {
            _collideZones.Clear();

            for (int i = 0; i < _children.Length; i++)
            {
                if (_children[i] != null)
                {
                    _children[i].Clear();
                    _children[i] = null;
                }
            }
        }

        public void Update(Node container)
        {
            if (container == null) return;
            if (container._childs.Count() == 0) return; // Si le conteneur n'a pas d'enfants, on sort de la fonction

            Collision2D.ResetAllZone(container);
            // Update logic for the quadtree can be added here if needed
            Clear();


            for (int i = 0; i < container._childs.Count(); i++) // Boucle for classique pour itérer sur la liste des Nodes
            {
                Node? node = container._childs.At(i);
                if (node == null) continue;
                for (int j = 0; j < node._collideZones.Count; j++) // Boucle for classique pour les CollideZones
                {
                    Insert(node._collideZones[j]);
                }
            }

            for (int i = 0; i < container._childs.Count(); i++) // Boucle for classique pour le premier Node
            {
                Node? nodeA = container._childs.At(i);
                if (nodeA == null) continue;
                for (int j = 0; j < nodeA._collideZones.Count; j++) // Boucle for classique pour la première CollideZone
                {
                    Collide.Zone collideZoneA = nodeA._collideZones[j];

                    List<Collide.Zone> possibleCollisions = [];
                    Retrieve(possibleCollisions, collideZoneA._rect);

                    for (int k = 0; k < possibleCollisions.Count; k++) // Boucle for classique pour les collisions potentielles
                    {
                        Collide.Zone collisionEntryB = possibleCollisions[k];
                        Node nodeB = collisionEntryB._node;
                        Collide.Zone collideZoneB = collisionEntryB;

                        if (nodeA != nodeB || collideZoneA != collideZoneB)
                        {
                            if (Collision2D.RectRect(collideZoneA._rect, collideZoneB._rect))
                            {
                                // Collision détectée
                                Collision2D.MakeCollideZone(collideZoneA, collideZoneB);
                            }
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch batch, Color color, float thickness = 1f)
        {
            batch.Rectangle(_bounds, color, thickness); // Dessine les limites du nœud actuel

            if (_children[0] != null)
            {
                for (int i = 0; i < _children.Length; i++)
                {
                    _children[i].Draw(batch, color); // Appel récursif pour les enfants
                }
            }
        }

    }
}
