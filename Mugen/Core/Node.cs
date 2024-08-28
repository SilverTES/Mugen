using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mugen.Physics;
using Mugen.GUI;
using Mugen.Event;

namespace Mugen.Core
{
    public interface IClone<T>
    {
        T Clone();
    }
    public class Navi
    {
        public bool _onMouseOver = false;  // trigger :Mouse over the Node
        public bool _isMouseOver = false;  // status : Mouse over the Node

        public bool _onMouseOut = false;   // trigger : Mouse out of Node
        public bool _isMouseOut = false;   // statuc : Mouse out of Node

        public bool _isClickable = false; // status : Node is Clickable 
        public bool _onClick = false; // trigger : Node is Clicked
        public bool _isClick = false; // status : Node is Clicked

        public bool _onPress = false; // trigger : on pressed navi
        public bool _isPress = false; // status : is pressed navi

        public bool _onRelease = false; // trigger : on pressed navi
        public bool _isRelease = false; // status : is pressed navi

        public bool _isFocusable = true; // focusable by default !
        public bool _onFocus = false; // trigger : is focused navi
        public bool _isFocus = false; // status : is focused navi
    }
    public class Node : ZIndex, IClone<Node>
    {
        #region Attributes

        public static Node _nodeRoot = new Node("root");

        // Debug 
        public static bool _showNodeInfo = false;

        // Identity 
        public int _type = Const.NoIndex; // Type of Node
        public int _subType = Const.NoIndex; // SubType of Node 
        public List<int> _class = new List<int>(); // List of Class Selector 
        public string _name = ""; // name of the node

        // Animation
        //public int _curFrame { get; private set; } // Current frame of Clip
        //public int _nbFrame = 0;

        // Status
        //public bool _isPlay = true;    // if Clip is playing
        public bool _isActive = true;  // if Clip is active -> update()
        public bool _isVisible = true; // if Clip is visible -> render()
        public bool _isAppend = false; // If append to an Parent Clip !

        // Gui
        //public Input.MouseEvent _mouse;
        public Navi _navi = new();

        // Addon
        private List<Addon.Base> _addons = new();

        // NaviNode - Navigate
        public NaviGate? _naviGate = null; // need to create new instance in node or derived !!
        public Dictionary<Position, Node> _naviNodes = new Dictionary<Position, Node>();

        // Legacy
        public Node? _parent = null;
        public Node? _master = null;
        public Node? _original = null;

        // World2D
        public float _x;
        public float _y;
        public float _alpha = 1f;

        public float AbsXF;
        public float AbsYF;

        public int AbsX;
        public int AbsY;

        public Vector2 XY = new Vector2(); // Relative position for update
        public Vector2 AbsXY = new Vector2(); // Absolute position for render


        //public ZIndex _zIndex = new ZIndex {_z = 0, _index = 0};
        public RectangleF _rect = new RectangleF();
        public Rectangle AbsRect = new Rectangle();
        public RectangleF AbsRectF = new RectangleF();

        public RectangleF _rectView = new RectangleF(); // Rect View : Camera or other

        // Offset : _rect._x, _rect._y <-> _x, _y
        public float _oX = 0;
        public float _oY = 0;
        public Vector2 OXY = new Vector2();

        // Collision Check
        public HashSet<int> _vecCollideBy = new HashSet<int>(); // std::set for avoid duplicate elements

        // Messaging
        public MessageEvent? _message = null;
        public bool HasMessage()
        {
            return null != _message;
        }
        public void EndMessage()
        {
            _message = null;
        }
        public void PollMessage()
        {
            if (null != _message)
            {
                OnMessage();
                _message = null;
            }
        }
        protected virtual void OnMessage() { }

        // map of Collide Zone(Rect) of Clip
        public Dictionary<int, Collide.Zone> _collideZones = new Dictionary<int, Collide.Zone>();

        public IContainer<Node> _childs = new IContainer<Node>();

        public List<ZIndex> _zIndexs = new List<ZIndex>();
        public ZIndex _frontZ = new ZIndex { _z = int.MaxValue, _index = 0 };
        public ZIndex _backZ = new ZIndex { _z = 0, _index = 0 };

        //public Animate _animate = new Animate();
        //public AnimateVec2 _animateVec2 = new AnimateVec2();

        // Action queue script Minor methods
        public Action<Node>? _createAction = null;
        public Action<Node>? _initAction = null;
        public Action<Node>? _updateAction = null;
        public Action<Node, SpriteBatch>? _renderAction = null;

        protected int _state;
        protected int _prevState;

        #endregion

        #region Methodes
        // --- Manage States
        public void SetState(int state)
        {
            // Exit previous state
            ExitState();
            // change state to new state
            _prevState = _state;
            _state = state;
            // Enter new state
            EnterState();
        }
        public void BackState()
        {
            SetState(_prevState);
        }
        protected virtual void ExitState() {}
        protected virtual void EnterState() {}
        protected virtual void RunState(GameTime gameTime) {}
        // --- Navigation
        public Node SetAsNaviNodeFocus()
        {
            if (null != _parent)
                if (null != _parent._naviGate)
                    _parent._naviGate.SetNaviNodeFocusAt(_index);

            return this;
        }
        public Node SetNaviNode(Position direction, Node node)
        {
            _naviNodes[direction] = node;
            return this;
        }
        public static void SetNaviNodeHorizontal(Node nodeLeft, Node nodeRight)
        {
            nodeLeft._naviNodes[Position.RIGHT] = nodeRight;
            nodeRight._naviNodes[Position.LEFT] = nodeLeft;
        }
        public static void SetNaviNodeVertical(Node nodeUp, Node nodeDown)
        {
            nodeUp._naviNodes[Position.DOWN] = nodeDown;
            nodeDown._naviNodes[Position.UP] = nodeUp;
        }

        // --- Constructor
        public Node() { }
        public Node(string name) { _name = name; }
        ~Node()
        {
            //Console.WriteLine("Kill Node");
        }
        //public Node SetGame<T>(T game) where T : Game
        //{   
        //    _game = game;
        //    return this;
        //}
        //public T GetGame<T>() where T : Game
        //{
        //    return (T)_game;
        //}

        // --- Clonage
        public Node GetMaster(Node node)
        {
            Node? master = null;

            if (null != node._original)
                master = GetMaster(node._original);
            else
                master = node;

            return master;
        }
        public virtual Node Clone()
        {
            Node clone = (Node)MemberwiseClone();

            clone._initAction = _initAction;
            clone._updateAction = _updateAction;

            //clone._animate = new Animate();

            //clone._navi = _navi.Clone();

            //clone._button = _button.Clone();

            clone._childs = new IContainer<Node>();
            for (int i = 0; i < _childs.Count(); i++)
            {
                //clone.Append(_childs.At(i).Clone());
                if (null != _childs.At(i))
                    _childs.At(i)!.Clone().AppendTo(clone);
            }

            //clone._node = new Dictionary<string, Node>();
            //foreach (KeyValuePair<string, Node> node in _node)
            //{
            //    clone._node[node.Key] = node.Value.Clone();
            //}

            clone._collideZones = new Dictionary<int, Collide.Zone>();
            foreach (KeyValuePair<int, Collide.Zone> it in _collideZones)
            {
                clone._collideZones[it.Key] = it.Value.Clone();
            }

            clone._zIndexs = new List<ZIndex>();
            for (int i = 0; i < _zIndexs.Count(); i++)
            {
                clone._zIndexs.Add(_zIndexs[i]);
            }

            // -- Clone Addons
            //clone._addon = new Dictionary<int, Addon.Base>();
            //foreach (KeyValuePair<int, Addon.Base> entry in _addon)
            //{
            //    clone._addon[entry.Key] = entry.Value.Clone<Addon.Base>(clone);

            //    //clone._addon[entry.Key]._node = clone; // Change component owner !! Important !
            //}

            return clone;
        }
        public Node Add(Node node)
        {
            if (null != node)
            {
                node._parent = this;
                _childs.Add(node);
            }
            return this;
        }
        //private Node SetParent(Node parent)
        //{
        //    _parent = parent;
        //    return this;
        //}
        public virtual Node AppendTo(Node parent) // Child Append to Parent
        {
            if (null != parent)
            {
                _isAppend = true;
                _parent = parent;
                _parent.Add(this);
            }

            UpdateRect();
            return this;
        }
        public virtual Node Append(Node child) // Parent Append Child
        {
            if (null != child)
            {
                child._isAppend = true;
                _parent = this;
                Add(child);
            }

            UpdateRect();
            return this;
        }

        // --- Derived Node 
        public T This<T>() where T : Node
        {
            return (T)this;
        }
        // --- Minor Action
        public Node OnCreateAction(Action<Node> createAction)
        {
            _createAction = createAction;
            _createAction?.Invoke(this);
            return this;
        }
        public Node CreateAction()
        {
            _createAction?.Invoke(this);
            return this;
        }
        public Node OnInitAction(Action<Node> initAction)
        {
            _initAction = initAction;
            return this;
        }
        public Node InitAction()
        {
            _initAction?.Invoke(this);
            return this;
        }
        public Node OnUpdateAction(Action<Node> updateAction)
        {
            _updateAction = updateAction;
            return this;
        }
        public void UpdateAction()
        {
            _updateAction?.Invoke(this);
        }
        public Node OnRenderAction(Action<Node, SpriteBatch> renderAction)
        {
            _renderAction = renderAction;
            return this;
        }
        public void RenderAction(SpriteBatch batch)
        {
            _renderAction?.Invoke(this, batch);
        }

        //public Node AddAnimate(String name, Func<float, float, float, float, float> easing, Tweening tweening)
        //{
        //    _animate.Add(name, easing, tweening);
        //    return this;
        //}

        // --- Identity
        public Node SetType(int type)
        {
            _type = type;
            return this;
        }
        public Node SetName(String name)
        {
            _name = name;
            return this;
        }

        // --- Childs
        public void KillMe()
        {
            //Misc.log(_index + " begin Kill !\n");
            //_parent._childs.Del(_index);
            _parent!._childs.Delete(this);
            //Misc.log(_index + " end Kill !\n");
            //_parent._vecClip.del(this);

        }
        public IContainer<Node> Childs()
        {
            return _childs;
        }
        public int NbNode()
        {
            return _childs.Count();
        }
        public int NbActive()
        {
            return _childs.NbActive();
        }
        public Node? Index(int index)
        {
            return _childs.At(index);
        }

        public int KillAll()
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                        {
                            _childs.At(i)!.KillMe();
                            nbKill++;
                        }
                    }
                }

            return nbKill;
        }
        public int KillAllAndKeep(string name)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                        {
                            if (_childs.At(i)!._name != name)
                            {
                                _childs.At(i)!.KillMe();
                                nbKill++;
                            }
                        }
                    }
                }

            return nbKill;
        }
        public int KillAll(string name)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)!._name == name)
                            {
                                _childs.At(i)!.KillMe();
                                nbKill++;
                            }
                    }
                }
            return nbKill;
        }
        public int KillAllAndKeep(int type)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)!._type != type)
                            {
                                _childs.At(i)!.KillMe();
                                nbKill++;
                            }
                    }
                }
            return nbKill;
        }
        public int KillAll(int type)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)!._type == type)
                            {
                                _childs.At(i)!.KillMe();
                                nbKill++;
                            }
                    }
                }
            return nbKill;
        }
        public int KillAllAndKeep(int[] types)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                        {
                            bool keep = false;

                            for (int t = 0; t < types.Length; t++)
                            {
                                if (_childs.At(i)!._type == types[t])
                                {
                                    keep = true;
                                    break;
                                }
                            }

                            if (!keep)
                            {
                                _childs.At(i)!.KillMe();
                                nbKill++;
                            }
                        }

                    }
                }
            return nbKill;
        }
        public int KillAll(int[] types)
        {
            int nbKill = 0;
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        for (int t = 0; t < types.Length; t++)
                        {
                            if (null != _childs.At(i))
                                if (_childs.At(i)!._type == types[t])
                                {
                                    _childs.At(i)!.KillMe();
                                    nbKill++;
                                }

                        }
                    }
                }
            return nbKill;
        }
        public int IndexByName(String name)
        {
            if (null != _childs)
                if (_childs.Count() > 0)
                {
                    for (int i = 0; i < _childs.Count(); ++i)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)!._name == name)
                                return i;
                    }
                }
            return 0;
        }


        public List<Node> GroupAll()
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        nodes.Add(_childs.At(i)!);
                }
            return nodes;
        }
        public List<Node> GroupOf(String name)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        if (_childs.At(i)!._name == name)
                            nodes.Add(_childs.At(i)!);
                }
            return nodes;
        }
        public List<Node> GroupOf(int type)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        if (_childs.At(i)!._type == type)
                            nodes.Add(_childs.At(i)!);
                }
            return nodes;
        }
        public List<Node> GroupOf(int[] types)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    for (int t = 0; t < types.Length; t++)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)!._type == types[t])
                                nodes.Add(_childs.At(i)!);
                    }
                }
            return nodes;
        }
        public List<Node> SubGroupOf(int subType)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        if (_childs.At(i)!._subType == subType)
                            nodes.Add(_childs.At(i)!);
                }
            return nodes;
        }
        public List<Node> SubGroupOf(int[] subTypes)
        {
            List<Node> nodes = new List<Node>();
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    for (int t = 0; t < subTypes.Length; t++)
                    {
                        if (null != _childs.At(i))
                            if (_childs.At(i)!._subType == subTypes[t])
                                nodes.Add(_childs.At(i)!);
                    }
                }
            return nodes;
        }

        public void LogAll()
        {
            Console.WriteLine("- Node: name = \"{0}\" | index = {1} | type = {2}", _name, _index, _type);
            if (_childs.Count() > 0)
                for (int i = 0; i < _childs.Count(); ++i)
                {
                    if (null != _childs.At(i))
                        Console.WriteLine("   - Child Node: name = \"{0}\" | index = {1} | type = {2}", _childs.At(i)!._name, _childs.At(i)!._index, _childs.At(i)!._type);
                }
        }


        // --- Status
        //public Node SetCollidable(bool isCollidable)
        //{
        //    _isCollidable = isCollidable;
        //    return this;
        //}
        //public Node SetCameraMoveFactor(float cameraMoveFactor)
        //{
        //    _cameraMoveFactor = cameraMoveFactor;
        //    return this;
        //}
        public Node SetRectView(RectangleF rectView)
        {
            _rectView = rectView;
            return this;
        }
        public Node UpdateRectView(float x, float y, float width, float height)
        {
            _rectView.X = x;
            _rectView.Y = x;
            _rectView.Width = width;
            _rectView.Height = height;
            return this;
        }
        public bool InRectView()
        {
            if (null != _parent)
                return Misc.PointInRect(AbsX, AbsY, _parent._rectView);

            return false;
        }
        public bool InRectView(RectangleF rectView)
        {
            return Misc.PointInRect(AbsX, AbsY, rectView);
        }
        public Node SetActive(bool isActive)
        {
            _isActive = isActive;

            //if (_isActive)
            //    _curFrame = 0;

            return this;
        }
        public Node SetVisible(bool isVisible)
        {
            _isVisible = isVisible;
            return this;
        }
        //public Node SetMouse(Input.Mouse mouse)
        //{
        //    _mouse = mouse;
        //    return this;
        //}

        // --- World 2D
        public void UpdateRect()
        {
            _rect.X = _x - _oX;
            _rect.Y = _y - _oY;

            OXY.X = _oX;
            OXY.Y = _oY;
            // For determinate relative & absolute Clip position !
            if (null != _parent)
            {
                AbsRectF.X = _rect.X + _parent.AbsRectF.X;
                AbsRectF.Y = _rect.Y + _parent.AbsRectF.Y;

                AbsX = (int)(AbsXF = _parent.AbsRectF.X + _x);
                AbsY = (int)(AbsYF = _parent.AbsRectF.Y + _y);
            }
            else
            {
                AbsRectF.X = _rect.X;
                AbsRectF.Y = _rect.Y;

                AbsX = (int)(AbsXF = _x);
                AbsY = (int)(AbsYF = _y);

            }

            XY.X = _x;
            XY.Y = _y;

            AbsRectF.Width = _rect.Width;
            AbsRectF.Height = _rect.Height;

            AbsXY.X = AbsX;
            AbsXY.Y = AbsY;

            AbsRect.X = (int)AbsRectF.X;
            AbsRect.Y = (int)AbsRectF.Y;
            AbsRect.Width = (int)AbsRectF.Width;
            AbsRect.Height = (int)AbsRectF.Height;


            // Update Childs Rect !
            for (int index = 0; index < _childs.Count(); index++)
                if (null != _childs.At(index))
                    _childs.At(index)!.UpdateRect();

        }

        public Node SetPosition(float x, float y)
        {
            _x = x;
            _y = y;
            UpdateRect();
            return this;
        }
        public Node SetPosition(Vector2 position)
        {
            _x = position.X;
            _y = position.Y;
            UpdateRect();
            return this;
        }
        public Node SetPosition(Position position, Node? parent = null)
        {
            float pW = 1, pH = 1;

            if (null == parent)
            {
                if (null != _parent)
                {
                    pW = _parent._rect.Width;
                    pH = _parent._rect.Height;
                }
                else
                {
                    return this;
                }
            }
            else
            {
                pW = parent._rect.Width;
                pH = parent._rect.Height;
            }

            switch (position)
            {
                case Position.M:
                    _x = pW / 2;
                    _y = pH / 2;
                    break;
                case Position.NW:
                    _x = 0;
                    _y = 0;
                    break;
                case Position.NE:
                    _x = pW;
                    _y = 0;
                    break;
                case Position.SW:
                    _x = 0;
                    _y = pH;
                    break;
                case Position.SE:
                    _x = pW;
                    _y = pH;
                    break;
                case Position.N:
                    _y = 0;
                    break;
                case Position.S:
                    _y = pH;
                    break;
                case Position.W:
                    _x = 0;
                    break;
                case Position.E:
                    _x = pW;
                    break;
                case Position.NM:
                    _x = pW / 2;
                    _y = 0;
                    break;
                case Position.SM:
                    _x = pW / 2;
                    _y = pH;
                    break;
                case Position.WM:
                    _x = 0;
                    _y = pH / 2;
                    break;
                case Position.EM:
                    _x = pW;
                    _y = pH / 2;
                    break;
                default:
                    break;
            }

            UpdateRect();
            return this;
        }
        public Node SetSize(float w, float h)
        {
            AbsRectF.Width = _rect.Width = w;
            AbsRectF.Height = _rect.Height = h;
            UpdateRect();
            return this;
        }
        public Node SetSize(Vector2 size)
        {
            SetSize(size.X, size.Y);
            return this;
        }
        public Node SetX(Position position)
        {
            if (null != _parent)
            {
                RectangleF pR = _parent._rect;

                switch (position)
                {
                    case Position.M:
                        _x = pR.Width / 2;
                        break;
                    case Position.W:
                        _x = pR.X;
                        break;
                    case Position.E:
                        _x = pR.Width;
                        break;
                    default:
                        break;
                }
            }
            return this;
        }
        public Node SetY(Position position)
        {
            if (null != _parent)
            {
                RectangleF pR = _parent._rect;

                switch (position)
                {
                    case Position.M:
                        _y = pR.Height / 2;
                        break;
                    case Position.N:
                        _y = pR.Y;
                        break;
                    case Position.S:
                        _y = pR.Height;
                        break;
                    default:
                        break;
                }
            }
            return this;
        }

        //public Node SetPosition(Position positionX , Position positionY)
        //{
        //    SetX(positionX);
        //    SetY(positionY);
        //    return this;
        //}
        //public Node SetPosition(float x, Position positionY)
        //{
        //    SetX(x);
        //    SetY(positionY);
        //    return this;
        //}
        //public Node SetPosition(Position positionX, float y)
        //{
        //    SetX(positionX);
        //    SetY(y);
        //    return this;
        //}

        public Node SetX(float x)
        {
            _x = x;
            UpdateRect();
            return this;
        }
        public Node SetY(float y)
        {
            _y = y;
            UpdateRect();
            return this;
        }
        public Node SetZ(int z)
        {
            _z = z;
            return this;
        }
        public Node SetPivotX(float oX)
        {
            _oX = oX;
            UpdateRect();
            return this;
        }
        public Node SetPivotY(float oY)
        {
            _oY = oY;
            UpdateRect();
            return this;
        }
        public Node SetPivot(float oX, float oY)
        {
            SetPivotX(oX);
            SetPivotY(oY);
            return this;
        }
        public Node SetPivot(Position position)
        {
            float pW = _rect.Width;
            float pH = _rect.Height;

            switch (position)
            {
                case Position.M:
                    _oX = pW / 2;
                    _oY = pH / 2;
                    break;
                case Position.NW:
                    _oX = 0;
                    _oY = 0;
                    break;
                case Position.NE:
                    _oX = pW;
                    _oY = 0;
                    break;
                case Position.SW:
                    _oX = 0;
                    _oY = pH;
                    break;
                case Position.SE:
                    _oX = pW;
                    _oY = pH;
                    break;
                case Position.N:
                    _oY = 0;
                    break;
                case Position.S:
                    _oY = pH;
                    break;
                case Position.W:
                    _oX = 0;
                    break;
                case Position.E:
                    _oX = pW;
                    break;
                case Position.NM:
                    _oX = pW / 2;
                    _oY = 0;
                    break;
                case Position.SM:
                    _oX = pW / 2;
                    _oY = pH;
                    break;
                case Position.WM:
                    _oX = 0;
                    _oY = pH / 2;
                    break;
                case Position.EM:
                    _oX = pW;
                    _oY = pH / 2;
                    break;
                default:
                    break;
            }
            return this;
        }
        // Main methods 

        // Addons
        public int AddAddon(Addon.Base addon) // return index of the addons added ! 
        {
            _addons.Add(addon);

            return _addons.Count-1;
        }

        public virtual Node Init() { return this; }
        public virtual Node Done() { return this; }
        public Node InitChilds()
        {
            for (int i = 0; i < _childs.Count(); i++)
                _childs.At(i)!.Init();

            return this;
        }
        public Node DoneChilds()
        {
            for (int i = 0; i < _childs.Count(); i++)
                _childs.At(i)!.Done();

            return this;
        }

        public virtual Node Update(GameTime gameTime) { UpdateAddons(gameTime); return this; }
        public virtual Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer) { return this; } // NonPremultiplied
        public void UpdateAddons(GameTime gameTime) 
        {
            for (int i = 0; i < _addons.Count; i++)
            {
                _addons[i].Update(gameTime);
            }
        }

        //public void DrawAddons(GameTime gameTime)
        //{
        //    for (int i = 0; i < _addons.Count; i++)
        //    {
        //        _addons[i].Draw(gameTime);
        //    }
        //}

        public void GotoFront(int index, int type = -1)
        {
            var node = _childs.At(index);
            if (null == node) 
                return;
            
            List<Node> listNode;

            if (type == -1)
                listNode = GroupAll();
            else
                listNode = GroupOf(type);

            listNode = listNode.OrderByDescending(v => v._z).ToList(); // sort group by z !!

            int indexBeginSwap = 0;

            for (int i = 0; i < listNode.Count; i++)
            {
                if (listNode[i]._index == node._index)
                {
                    indexBeginSwap = i;
                    break;
                }
            }

            for (int i = indexBeginSwap; i < listNode.Count - 1; i++)
            {
                float tmpZ = listNode[i]._z;
                listNode[i]._z = listNode[i + 1]._z;
                listNode[i + 1]._z = tmpZ;

                if (listNode[i]._z == listNode[i + 1]._z) // Avoid conflict same z
                    listNode[i]._z--;

                Misc.Swap<Node>(listNode, i, i + 1);
            }

            for (int i = indexBeginSwap; i < listNode.Count; i++)
            {
                _childs.SetAt(listNode[i]._index, listNode[i]);


            }

        }

        public ZIndex FrontZ()
        {
            return _frontZ;
        }
        public ZIndex BackZ()
        {
            return _backZ;
        }
        public Node UpdateChilds(GameTime gameTime)
        {
            for (int index = 0; index < _childs.Count(); index++)
                if (null != _childs.At(index))
                    if (_childs.At(index)!._isActive)
                        _childs.At(index)!.Update(gameTime);
            return this;
        }
        public Node UpdateChildsSort(GameTime gameTime)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _zIndexs.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                        {
                            if (_childs.At(ZIndex(index))!._isActive)
                            {
                                // --- Find the front Z
                                if (_childs.At(ZIndex(index))!._z < _frontZ._z)
                                {
                                    _frontZ._z = _childs.At(ZIndex(index))!._z;
                                    _frontZ._index = index;
                                }
                                // --- Find the back Z
                                if (_childs.At(ZIndex(index))!._z > _backZ._z)
                                {
                                    _backZ._z = _childs.At(ZIndex(index))!._z;
                                    _backZ._index = index;
                                }

                                _childs.At(ZIndex(index))!.Update(gameTime);
                            }
                        }
                    }
            }
            return this;
        }
        public Node DrawChilds(SpriteBatch batch, GameTime gameTime, int indexLayer = 0)
        {
            for (int index = 0; index < _childs.Count(); ++index)
            {
                if (!_childs.IsEmpty())
                    if (index < _zIndexs.Count)
                    {
                        if (null != _childs.At(ZIndex(index)))
                            if (_childs.At(ZIndex(index))!._isVisible)
                                _childs.At(ZIndex(index))!.Draw(batch, gameTime, indexLayer);
                    }
                    else
                    {
                        if (null != _childs.At(index))
                            if (_childs.At(index)!._isVisible)
                                _childs.At(index)!.Draw(batch, gameTime, indexLayer);
                    }
            }
            return this;
        }
        public int ZIndex(int index)
        {
            if (index < 0 || index >= _zIndexs.Count) 
                return -1;

            return _zIndexs[index]._index;
        }
        public ZIndex? GetChildZIndex(int index)
        {
            if (index < 0 || index >= _zIndexs.Count)
                return null;

            return _zIndexs[index];
        }
        public Node SortZDescending()  // Descending Sort
        {
            SortZIndexDescending(_childs._objects!);
            return this;
        }
        public Node SortZAscending() // Ascending Sort
        {
            SortZIndexAscending(_childs._objects!);
            return this;
        }
        private void SortZIndexDescending(List<Node> vecEntity)
        {
            // Resize listZIndex if smaller than listObj
            if (_zIndexs.Count < vecEntity.Count)
            {
                //mlog("- Resize ZIndex !\n");
                for (int index = _zIndexs.Count; index < vecEntity.Count; ++index)
                {
                    _zIndexs.Add(new ZIndex());
                }
            }

            for (int index = 0; index < _zIndexs.Count; ++index)
            {
                if (null != _zIndexs[index])
                    _zIndexs[index]._index = index;
                else
                    continue;

                if (index >= 0 && index < vecEntity.Count)
                    if (null != vecEntity[index])
                    {
                        _zIndexs[index]._z = vecEntity[index]._z;
                    }
                    else
                        _zIndexs[index]._z = 0;
            }

            _zIndexs = _zIndexs.OrderByDescending(v => v._z).ToList();
            //_zIndexs.Sort((index1, index2) => index2._z.CompareTo(index1._z));
        }
        private void SortZIndexAscending(List<Node> vecEntity)
        {
            // Resize listZIndex if smaller than listObj
            if (_zIndexs.Count < vecEntity.Count)
            {
                //mlog("- Resize ZIndex !\n");
                for (int index = _zIndexs.Count; index < vecEntity.Count; ++index)
                {
                    _zIndexs.Add(new ZIndex());
                }
            }

            for (int index = 0; index < _zIndexs.Count; ++index)
            {
                if (null != _zIndexs[index])
                    _zIndexs[index]._index = index;
                else
                    continue;

                if (index >= 0 && index < vecEntity.Count)
                    if (null != vecEntity[index])
                    {
                        _zIndexs[index]._z = vecEntity[index]._z;
                    }
                    else
                        _zIndexs[index]._z = 0;
            }

            _zIndexs = _zIndexs.OrderBy(v => v._z).ToList();
            //_zIndexs.Sort((index1, index2) => index1._z.CompareTo(index2._z));

        }
        // Collisions methods !
        public Node SetCollideZone(int index, RectangleF rect)
        {
            Collide.Zone collideZone = new Collide.Zone
            (
                index,
                new RectangleF
                (
                    AbsX + rect.X,
                    AbsY + rect.Y,
                    rect.Width,
                    rect.Height
                ),
                this
            );
            _collideZones[index] = collideZone;

            return this;
        }
        public Collide.Zone? GetCollideZone(int index)
        {
            if (null != _collideZones[index])
                return _collideZones[index];

            return null;
        }
        public Node UpdateCollideZone(int index, RectangleF rect, bool isCollidable = true, int collideLevel = 0)
        {
            if (null != GetCollideZone(index))
            {
                GetCollideZone(index)!._rect = rect;
                GetCollideZone(index)!._isCollidable = isCollidable;
                GetCollideZone(index)!._collideLevel = collideLevel;
            }

            return this;
        }
        public Node DrawCollideZone(SpriteBatch batch, int index, Color color)
        {
            if (null != GetCollideZone(index) && _showNodeInfo)
                GFX.GFX.Rectangle(batch, GetCollideZone(index)!._rect, color, 0);

            return this;
        }

        // Frames methods !
        //public int CurFrame()
        //{
        //    return _curFrame;
        //}
        //public bool IsPlay()
        //{
        //    return _isPlay;
        //}
        //public void NextFrame()
        //{
        //    if (_isPlay)
        //        ++_curFrame;
        //}
        //public void PrevFrame()
        //{
        //    if (_isPlay)
        //        --_curFrame;
        //}
        //public bool OnFrame(int frame)
        //{
        //    return _curFrame == frame;
        //}
        public virtual void Start()
        {

        }
        //public void Stop()
        //{
        //    _isPlay = false;
        //    _curFrame = 0;
        //}
        //public void Pause()
        //{
        //    _isPlay = false;
        //}
        //public void Resume()
        //{
        //    _isPlay = true;
        //}
        //public void StartAt(int frame)
        //{
        //    _curFrame = frame;
        //    Start();
        //}
        //public void StopAt(int frame)
        //{
        //    _curFrame = frame;
        //    Stop();
        //}

        #endregion

    }
}
