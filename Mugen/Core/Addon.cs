using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mugen.Animation;
using Mugen.Physics;

namespace Mugen.Core
{
    public static class Addon
    {
        public class Base 
        {
            protected Node? _node = null;

            public Base(Node node)
            {
                _node = node;
            }

            public virtual void Init() { }
            public virtual void Update(GameTime gameTime) { }
            public virtual void Draw(GameTime gameTime) { }

        }

        public class Loop : Base
        {
            bool _run = false;
            bool _onEnd = false;
            bool _onBegin = false;
            public float _current = 0;   // current position
            public float _begin = 0;     // begin at
            public float _end = 0;       // end at
            float _direction = 0; // direction of the loop
            Loops _loopType = 0;

            public Loop(Node node) : base(node)
            { 
            }    
            public Node? SetLoop(float current, float begin, float end, float direction, Loops loopType)
            {
                _current = current;
                _begin = begin;
                _end = end;
                _direction = direction;
                _loopType = loopType;

                return _node;
            }
            public Node? Start()
            {
                _run = true;
                return _node;
            }
            public Node? ReStart()
            {
                _run = true;
                _current = _begin;
                return _node;
            }
            public Node? Stop()
            {
                _run = false;
                return _node;
            }
            public bool OnBegin() { return _onBegin; }
            public bool OnEnd() { return _onEnd; }
            public override void Update(GameTime gameTime)
            {
                if (_run)
                {
                    _current += _direction;

                    _onEnd = false;
                    _onBegin = false;

                    if (_direction > 0 && _current > _end)
                    {
                        _current = _end;
                        _onEnd = true;
                    }

                    if (_direction < 0 && _current < _begin)
                    {
                        _current = _begin;
                        _onBegin = true;
                    }

                    if (_loopType == Loops.ONCE)
                    {
                        if (_onEnd || _onBegin)
                            Stop();
                    }
                    else if (_loopType == Loops.REPEAT)
                    {
                        if (_onEnd)
                            _current = _begin;
                        if (_onBegin)
                            _current = _end;
                    }
                    else if (_loopType == Loops.PINGPONG)
                    {
                        if (_onEnd || _onBegin)
                            _direction = -_direction;
                    }

                }
            }
        }
        public class Draggable : Base
        {
            private Input.MouseControl _mouse = new();

            public bool _isDraggable = false;

            public bool _onDragged = false;
            public bool _offDragged = false;
            public bool _isDragged = false;

            public bool _isDragByRectNode = true;
            public bool _isRectLimitZoneActive = false;
            public bool IsLimitRect => _isRectLimitZoneActive;
            float _dragX = 0;
            float _dragY = 0;
            float relX = 0; // parent X
            float relY = 0; // parent Y

            public RectangleF _rectDraggable { get; private set; }
            public RectangleF _rectLimitZone = new RectangleF();

            public Draggable(Node node, Input.MouseControl mouse) : base(node)
            {
                _mouse = mouse;
            }

            public Node? _containerNode { get; private set; }

            public Node? SetDraggable(bool isDraggable)
            {
                _isDraggable = isDraggable;
                return _node;
            }
            public Node? SetLimitRect(bool isLimitRect)
            {
                _isRectLimitZoneActive = isLimitRect;
                return _node;
            }
            public Node? SetLimitRect(RectangleF limitRect)
            {
                _isRectLimitZoneActive = true;
                _rectLimitZone = limitRect.CloneSize();
                return _node;
            }
            public Node? SetLimitRect(Node containerNode)
            {
                if (null != containerNode)
                {
                    _isRectLimitZoneActive = true;
                    _containerNode = containerNode;
                    _rectLimitZone = _containerNode._rect.CloneSize();
                }
                return _node;
            }
            public RectangleF DragRect()
            {
                return _rectDraggable;
            }
            public Node? SetDragRect(RectangleF dragRect)
            {
                _rectDraggable = dragRect;

                return _node;
            }
            public Node? SetDragRectNode(bool isDragRectNode)
            {
                _isDragByRectNode = isDragRectNode;

                return _node;
            }
            public override void Update(GameTime gameTime)
            {
                _onDragged = false;
                _offDragged = false;

                if (_isDragByRectNode)
                {
                    _node!.UpdateRect();
                    _rectDraggable = _node.AbsRect;

                }

                relX = 0;
                relY = 0;

                if (null != _node!._parent)
                {
                    relX = _node._parent.AbsX;
                    relY = _node._parent.AbsY;
                }

                if (null != _containerNode) // Update Rect of container if exist
                {
                    relX = _containerNode.AbsX;
                    relY = _containerNode.AbsY;

                }

                _node.UpdateRect();

                if (null != _mouse && _isDraggable)
                {
                    _node._navi._isMouseOver = Collision2D.PointInRect(new Vector2(_mouse._x, _mouse._y), _rectDraggable);

                    if (_node._navi._isMouseOver)
                    {
                        _mouse._isOverAny = true;
                    }

                    if (_isDragged && _mouse._isMove && _node._navi._isFocus)
                    {
                        _node._x = _mouse._x - _dragX;
                        _node._y = _mouse._y - _dragY;
                    }


                    if (_mouse._onClick)
                    {
                        if (_node._navi._isMouseOver)
                        {
                            if (!_isDragged && (_mouse._button == 1 && !_mouse._isActiveDrag && !_mouse._isActiveReSize))
                            {
                                if (!_mouse._isActiveDrag)
                                {
                                    _onDragged = true;
                                    _isDragged = true;
                                    _mouse._isActiveDrag = true;
                                    _node._navi._onClick = true;
                                }
                            }
                        }
                    }
                    
                    if (_mouse._offClick)
                    {
                        if (_isDragged)
                        {
                            _isDragged = false;
                            _offDragged = true;
                        }
                    }

                    if (_isDragged)
                    {
                        _node._navi._isFocus = true;

                        _dragX = _mouse._x - _node._x;
                        _dragY = _mouse._y - _node._y;

                        _node.UpdateRect();
                    }



                }

                if (_isRectLimitZoneActive)
                {
                    if (_node._rect.X < _rectLimitZone.X)
                    {
                        _node._x = _rectLimitZone.X + _node._oX;
                        //_mouse->_x = _rect.x + _dragX;
                        _node.UpdateRect();
                        //std::cout << "< Out of LimitRect LEFT >";
                    }
                    if (_node._rect.Y < _rectLimitZone.Y)
                    {
                        _node._y = _rectLimitZone.Y + _node._oY;
                        //_mouse->_y = _rect.y + _dragY;
                        _node.UpdateRect();
                        //std::cout << "< Out of LimitRect TOP >";
                    }
                    if (_node._rect.X > _rectLimitZone.Width - _node._rect.Width)
                    {
                        _node._x = _rectLimitZone.Width - _node._rect.Width + _node._oX;
                        //_mouse->_x = _rect.x + _dragX;
                        _node.UpdateRect();
                        //std::cout << "< Out of LimitRect RIGHT >";
                    }
                    if (_node._rect.Y > _rectLimitZone.Height - _node._rect.Height)
                    {
                        _node._y = _rectLimitZone.Height - _node._rect.Height + _node._oY;
                        //_mouse->_y = _rect.y + _dragY;
                        _node.UpdateRect();
                        //std::cout << "< Out of LimitRect BOTTOM >";
                    }
                }
            }
            public void Draw(SpriteBatch batch)
            {
                RectangleF rect = _rectLimitZone;
                rect.Offset(relX, relY);
                //batch.DrawRectangle(_limitRect, Color.Red, 1);
                GFX.GFX.Rectangle(batch, rect, Color.Red, 1);
                GFX.GFX.Rectangle(batch, _rectDraggable, Color.YellowGreen, 1);
            }

        }
        public class Resizable : Base
        {
            private Input.MouseControl _mouse = new();

            bool _isResizable = false;

            bool _isResize = false;
            int _tolerance = 4;

            int _x1 = 0;
            int _y1 = 0;
            int _x2 = 0;
            int _y2 = 0;
            int _minW = 8;
            int _minH = 8;

            //float _dragX = 0;
            //float _dragY = 0;

            public bool _isMouseOver = false;

            bool _isOverN = false;
            bool _isOverS = false;
            bool _isOverW = false;
            bool _isOverE = false;
            bool _isOverNW = false;
            bool _isOverNE = false;
            bool _isOverSW = false;
            bool _isOverSE = false;

            bool _isDragN = false;
            bool _isDragS = false;
            bool _isDragW = false;
            bool _isDragE = false;
            bool _isDragNW = false;
            bool _isDragNE = false;
            bool _isDragSW = false;
            bool _isDragSE = false;

            public Resizable(Node node, Input.MouseControl mouse) : base(node)
            {
                _mouse = mouse;
            }
            public bool IsResize()
            {
                return _isResize;
            }
            public Node? SetResizable(bool isResizable)
            {
                _isResizable = isResizable;
                return _node;
            }
            public Node? Init(int tolerance)
            {
                _tolerance = tolerance;
                SetResize();
                return _node;
            }
            public Node? SetTolerance(int tolerance)
            {
                _tolerance = tolerance;
                return _node;
            }
            private Node? SetResize()
            {
                _x1 = (int)_node!._rect.X;
                _y1 = (int)_node._rect.Y;
                _x2 = (int)_node._rect.X + (int)_node._rect.Width;
                _y2 = (int)_node._rect.Y + (int)_node._rect.Height;

                return _node;
            }
            private Node? GetResize()
            {
                _node!._x = _node._rect.X = _x1 - ((null != _node._parent) ? (int)_node._parent._x : 0);
                _node._y = _node._rect.Y = _y1 - ((null != _node._parent) ? (int)_node._parent._y : 0);

                _node._rect.Width = _x2 - _node._rect.X - ((null != _node._parent) ? (int)_node._parent._x : 0);
                _node._rect.Height = _y2 - _node._rect.Y - ((null != _node._parent) ? (int)_node._parent._y : 0);

                return _node;

            }
            public override void Update(GameTime gameTime)
            {
                if (_isResizable)
                {
                    if (_mouse._isActiveReSize)
                    {
                        //if (null != _node._parent)
                        //{
                        //    if (null != _node._parent._navigate)
                        //        if (_node._parent._navigate.IsNaviGate())
                        //            _node._navigate.SetFocus(_node._index);
                        //}

                        GetResize();
                    }
                    else
                        SetResize();

                    if (_mouse._up)
                    {
                        _isResize = false;

                        _isOverN = false;
                        _isOverS = false;
                        _isOverW = false;
                        _isOverE = false;
                        _isOverNW = false;
                        _isOverNE = false;
                        _isOverSW = false;
                        _isOverSE = false;

                        _isDragN = false;
                        _isDragS = false;
                        _isDragW = false;
                        _isDragE = false;
                        _isDragNW = false;
                        _isDragNE = false;
                        _isDragSW = false;
                        _isDragSE = false;
                    }

                    int x = (int)_mouse._x;
                    int y = (int)_mouse._y;

                    // Test isOVER 
                    bool isOver = false;

                    if (Misc.ValueInRange(x, _node!._rect.X + _tolerance * 2, _node._rect.X + _node._rect.Width - _tolerance * 2) &&
                        Misc.ValueInRange(y, _node._rect.Y - _tolerance, _node._rect.Y + _tolerance) &&
                        !_mouse._isActiveReSize)
                        isOver = _isOverN = true;

                    if (Misc.ValueInRange(x, _node._rect.X + _tolerance * 2, _node._rect.X + _node._rect.Width - _tolerance * 2) &&
                        Misc.ValueInRange(y, _node._rect.Y + _node._rect.Height - _tolerance, _node._rect.Y + _node._rect.Height + _tolerance) &&
                        !_mouse._isActiveReSize)
                        isOver = _isOverS = true;

                    if (Misc.ValueInRange(x, _node._rect.X - _tolerance, _node._rect.X + _tolerance) &&
                        Misc.ValueInRange(y, _node._rect.Y + _tolerance * 2, _node._rect.Y + _node._rect.Height - _tolerance * 2) &&
                        !_mouse._isActiveReSize)
                        isOver = _isOverW = true;

                    if (Misc.ValueInRange(x, _node._rect.X + _node._rect.Width - _tolerance, _node._rect.X + _node._rect.Width + _tolerance) &&
                        Misc.ValueInRange(y, _node._rect.Y + _tolerance * 2, _node._rect.Y + _node._rect.Height - _tolerance * 2) &&
                        !_mouse._isActiveReSize)
                        isOver = _isOverE = true;

                    if (Misc.CircleInCircle(x, y, 0, _node._rect.X, _node._rect.Y, _tolerance * 2) && !_mouse._isActiveReSize)
                        isOver = _isOverNW = true;

                    if (Misc.CircleInCircle(x, y, 0, _node._rect.X + _node._rect.Width, _node._rect.Y, _tolerance * 2) && !_mouse._isActiveReSize)
                        isOver = _isOverNE = true;

                    if (Misc.CircleInCircle(x, y, 0, _node._rect.X, _node._rect.Y + _node._rect.Height, _tolerance * 2) && !_mouse._isActiveReSize)
                        isOver = _isOverSW = true;

                    if (Misc.CircleInCircle(x, y, 0, _node._rect.X + _node._rect.Width, _node._rect.Y + _node._rect.Height, _tolerance * 2) && !_mouse._isActiveReSize)
                        isOver = _isOverSE = true;

                    _isMouseOver = isOver;

                    // Test isCLICK
                    if (_node._navi._isFocus) // Resizable only if focused
                    {
                        if (_isOverN && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragN = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                        if (_isOverS && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragS = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                        if (_isOverW && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragW = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                        if (_isOverE && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragE = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }

                        if (_isOverNW && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragNW = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                        if (_isOverNE && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragNE = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                        if (_isOverSW && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragSW = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                        if (_isOverSE && _mouse._down && !_isResize && !_mouse._isActiveDrag)
                        {
                            _isDragSE = true;
                            _isResize = true;
                            _mouse._isActiveReSize = true;
                        }
                    }

                    // test isDRAG
                    if (_isDragN && _mouse._isMove && _isResize)
                    {
                        if (_node._rect.Height > _minH || _mouse._moveUP) // Limit Vertical Resize 
                            _y1 = _mouse._y;
                    }

                    if (_isDragS && _mouse._isMove && _isResize)
                    {
                        if (_node._rect.Height > _minH || _mouse._moveDOWN) // Limit Vertical Resize 
                            _y2 = _mouse._y;
                    }

                    if (_isDragW && _mouse._isMove && _isResize)
                    {
                        if (_node._rect.Width > _minW || _mouse._moveLEFT) // Limit Horizontal Resize 
                            _x1 = _mouse._x;
                    }

                    if (_isDragE && _mouse._isMove && _isResize)
                    {
                        if (_node._rect.Width > _minW || _mouse._moveRIGHT) // Limit Horizontal Resize 
                            _x2 = _mouse._x;
                    }

                    if (_isDragNW && _mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > _minH || _mouse._moveUP) &&
                            (_node._rect.Width > _minW || _mouse._moveLEFT))
                        {
                            _x1 = _mouse._x;
                            _y1 = _mouse._y;
                        }
                    }
                    if (_isDragNE && _mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > _minH || _mouse._moveUP) &&
                            (_node._rect.Width > _minW || _mouse._moveRIGHT))
                        {
                            _x2 = _mouse._x;
                            _y1 = _mouse._y;
                        }
                    }
                    if (_isDragSW && _mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > _minH || _mouse._moveDOWN) &&
                            (_node._rect.Width > _minW || _mouse._moveLEFT))
                        {
                            _x1 = _mouse._x;
                            _y2 = _mouse._y;
                        }
                    }
                    if (_isDragSE && _mouse._isMove && _isResize)
                    {
                        if ((_node._rect.Height > _minH || _mouse._moveDOWN) &&
                            (_node._rect.Width > _minW || _mouse._moveRIGHT))
                        {
                            _x2 = _mouse._x;
                            _y2 = _mouse._y;
                        }
                    }

                }

            }
            public void Draw(SpriteBatch batch, Input.MouseControl mouse, SpriteFont font)
            {
                //if (_isResize && Node._showNodeInfo)
                //    batch.DrawRectangle
                //    (
                //        new RectangleF((float)_x1 + .5f, (float)_y1 + .5f, (float)_x2-_x1 + 1.5f, (float)_y2-_y1 + 1.5f), 
                //        new Color(85,250,0,55), 4
                //    );

                if (_node!._navi._isFocus && _isResizable && Node._showNodeInfo)
                {
                    string str = String.Format("x1 {0}, y1 {1} : x2 {2}, y2 {3} : {4}", _x1, _y1, _x2, _y2,
                        (mouse._isActiveReSize) ? "GetResize()" : "SetResize()");
                    GFX.GFX.FillRectangle(batch, new Rectangle(mouse._x, mouse._y, (int)font.MeasureString(str).X + 2, (int)font.MeasureString(str).Y + 2), Color.Black);
                    batch.DrawString(font, str, new Vector2(mouse._x + 4, mouse._y + 4), Color.Gold);
                }

                if (!mouse._down)// && _node._navi._isFocus)
                {
                    int padding = 6;
                    int W = _x2 - _x1;
                    int H = _y2 - _y1;
                    int x = mouse._x;
                    int y = mouse._y;

                    if (_isOverN)
                    {
                        GFX.GFX.Triangle(batch, x + .5f, _y1 + .5f - padding,
                                                x + .5f - 8, _y1 + .5f - padding + 8,
                                                x + .5f + 8, _y1 + .5f - padding + 8,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverS)
                    {
                        GFX.GFX.Triangle(batch, x + .5f, _y2 + .5f + padding,
                                                x + .5f - 8, _y2 + .5f + padding - 8,
                                                x + .5f + 8, _y2 + .5f + padding - 8,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverW)
                    {
                        GFX.GFX.Triangle(batch, _x1 + .5f - padding, y + .5f,
                                                _x1 + .5f - padding + 8, y + .5f - 8,
                                                _x1 + .5f - padding + 8, y + .5f + 8,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverE)
                    {
                        GFX.GFX.Triangle(batch, _x2 + .5f + padding, y + .5f,
                                                _x2 + .5f + padding - 8, y + .5f - 8,
                                                _x2 + .5f + padding - 8, y + .5f + 8,
                                                new Color(255, 0, 0));
                    }


                    if (_isOverNW)
                    {
                        GFX.GFX.Triangle(batch, _x1 + .5f - padding, _y1 + .5f - padding,
                                                _x1 + .5f - padding + 10, _y1 + .5f - padding,
                                                _x1 + .5f - padding, _y1 + .5f - padding + 10,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverNE)
                    {
                        GFX.GFX.Triangle(batch, _x2 + .5f + padding, _y1 + .5f - padding,
                                                _x2 + .5f + padding - 10, _y1 + .5f - padding,
                                                _x2 + .5f + padding, _y1 + .5f - padding + 10,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverSW)
                    {
                        GFX.GFX.Triangle(batch, _x1 + .5f - padding, _y2 + .5f + padding,
                                                _x1 + .5f - padding + 10, _y2 + .5f + padding,
                                                _x1 + .5f - padding, _y2 + .5f + padding - 10,
                                                new Color(255, 0, 0));
                    }
                    if (_isOverSE)
                    {
                        GFX.GFX.Triangle(batch, _x2 + .5f + padding, _y2 + .5f + padding,
                                                _x2 + .5f + padding - 10, _y2 + .5f + padding,
                                                _x2 + .5f + padding, _y2 + .5f + padding - 10,
                                                new Color(255, 0, 0));
                    }
                }

            }
        }

    }
}
