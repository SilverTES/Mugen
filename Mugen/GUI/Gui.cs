using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Mugen.Core;
using Mugen.GFX;
using Mugen.Physics;
using System.Text.Json;
using Newtonsoft.Json;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;
using JsonConstructorAttribute = Newtonsoft.Json.JsonConstructorAttribute;

namespace Mugen.GUI
{
    public class Style : IClone<Style>
    {
        [JsonIgnore]
        public string _name = "";

        public class ColorValue
        {
            public static Color ColorFromName(string colorName)
            {

                if (colorName == "" || colorName == default)
                    return Color.Transparent;

                var props = typeof(Color).GetProperties();

                foreach (var p in props)
                    if (p.Name.ToLower() == colorName.ToLower())
                        return (Color)p.GetValue(null, null)!;

                // Default color if the color was not found
                return Color.Transparent;
            }

            public static Color ColorFromHexa(string colorHexa)
            {
                if (colorHexa[0] == '#')
                {
                    string colorString = colorHexa.TrimStart('#');

                    //Console.WriteLine("color Lenght = " + color.Length);

                    byte r = 0;
                    byte g = 0;
                    byte b = 0;
                    byte a = 255;

                    if (colorString.Length >= 3 && colorString.Length < 5)
                    {
                        string R = colorString.Substring(0, 1);
                        string G = colorString.Substring(1, 1);
                        string B = colorString.Substring(2, 1);
                        string A = "FF";

                        R += R;
                        G += G;
                        B += B;

                        if (colorString.Length == 4)
                        {
                            A = colorString.Substring(3, 1);
                            A += A;

                        }

                        colorString = R + G + B + A;

                        //Console.WriteLine("Decode 4Hexa = #" + color);
                    }


                    if (colorString.Length >= 6)
                    {
                        //Console.WriteLine("Decode 6 Hexa");

                        string R = colorString.Substring(0, 2);
                        string G = colorString.Substring(2, 2);
                        string B = colorString.Substring(4, 2);

                        try
                        {
                            r = byte.Parse(R, System.Globalization.NumberStyles.HexNumber);
                            g = byte.Parse(G, System.Globalization.NumberStyles.HexNumber);
                            b = byte.Parse(B, System.Globalization.NumberStyles.HexNumber);
                        }
                        catch (FormatException e)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("ERROR FORMAT RGB : " + e.Message);
                            Console.ResetColor();
                        }
                        finally
                        {
                            //r = 0;
                            //g = 0;
                            //b = 0;
                        }

                        if (colorString.Length == 8)
                        {
                            //Console.WriteLine("Decode 8 Hexa");

                            string A = colorString.Substring(6, 2);

                            try
                            {
                                a = byte.Parse(A, System.Globalization.NumberStyles.HexNumber);
                            }
                            catch (FormatException e)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("ERROR FORMAT ALPHA : " + e.Message);
                                Console.ResetColor();
                            }
                            finally
                            {
                                //a = 255;
                            }
                        }
                    }

                    ColorI resultColor = new ColorI { R = r, G = g, B = b, A = a };

                    //Console.WriteLine("{4} = {0} {1} {2} {3}", r, g, b, a, colorHexa);
                    //Console.WriteLine("ColorI result = " + resultColor);
                    //Console.WriteLine("ColorI result = " + resultColor.ToColor());

                    return resultColor.ToColor();
                }

                return Color.Transparent;
            }

            public static Color GetColor(string color)
            {
                if (color == "" || color == default)
                    return Color.Transparent;

                if (color[0] == '#')
                    return ColorFromHexa(color);
                else
                    return ColorFromName(color);
            }

            public static ColorValue MakeColor(string name = "")
            {
                ColorValue color = new ColorValue { _name = name };

                if (name != "")
                {
                    color._value = GetColor(name);
                }

                return color;
            }
            public static ColorValue MakeColor(Color value)
            {
                ColorValue color = new ColorValue { _value = value };
                return color;
            }

            [JsonProperty("value")]
            public Color _value { get; set; }
            [JsonProperty("color")]
            public string _name { get; set; } = "";

            [JsonIgnore]
            public Color Value => _name != "" ? _value = GetColor(_name) : _value;


            [JsonConstructor]
            public ColorValue() { }
            public ColorValue(string name)
            {
                _value = GetColor(name);
            }

            public ColorValue(Color value)
            {
                _value = value;
            }


        }





        public enum Fill
        {
            NONE,
            SOLID,
            IMAGE
        }

        public enum HorizontalAlign
        {
            Center,
            Left,
            Right
        }
        public enum VerticalAlign
        {
            Middle,
            Top,
            Bottom
        }
        public class Space
        {
            #region Attributes
            [JsonProperty("top")] public float _top = 0;
            [JsonProperty("bottom")] public float _bottom = 0;
            [JsonProperty("left")] public float _left = 0;
            [JsonProperty("right")] public float _right = 0;
            #endregion

            public Space(float space)
            {
                _top = space;
                _bottom = space;
                _left = space;
                _right = space;
            }
        }

        #region Properties
        [JsonProperty("showFocus")] public bool _showFocus { get; set; } = true;
        [JsonProperty("useSkin")] public bool _useSkin { get; set; } = true;

        [JsonIgnore]
        public SkinGui _skinGui;

        // Text
        [JsonIgnore]
        public SpriteFont _font;
        [JsonProperty("fontName")]
        public string _fontName { get; set; } = "";

        [JsonProperty("textBorder")] public bool _textBorder { get; set; } = false;
        [JsonProperty("color")] public ColorValue _color { get; set; } = ColorValue.MakeColor(Color.Black);
        [JsonProperty("colorTextBorder")] public ColorValue _colorTextBorder { get; set; } = ColorValue.MakeColor(Color.Black);

        // Content
        [JsonIgnore]
        public RectangleF _rect = new RectangleF();

        [JsonProperty("horizontalAlign")] public HorizontalAlign _horizontalAlign { get; set; } = HorizontalAlign.Center;
        [JsonProperty("verticalAlign")] public VerticalAlign _verticalAlign { get; set; } = VerticalAlign.Middle;

        [JsonProperty("padding")] public Space _padding { get; set; } = new Space(0);

        [JsonIgnore]
        public RectangleF _rectPadding = new RectangleF();

        [JsonProperty("overColor")] public ColorValue _overColor { get; set; } = ColorValue.MakeColor(Color.Transparent);
        [JsonProperty("overBorderColor")] public ColorValue _overBorderColor { get; set; } = ColorValue.MakeColor(Color.Transparent);
        [JsonProperty("overBackgroundFillColor")] public ColorValue _overBackgroundFillColor { get; set; } = ColorValue.MakeColor(Color.Transparent);

        [JsonProperty("focusColor")] public ColorValue _focusColor { get; set; } = ColorValue.MakeColor(Color.Transparent);
        [JsonProperty("focusBorderColor")] public ColorValue _focusBorderColor { get; set; } = ColorValue.MakeColor(Color.Transparent);
        [JsonProperty("focusBackgroundFillColor")] public ColorValue _focusBackgroundFillColor { get; set; } = ColorValue.MakeColor(Color.Transparent);

        [JsonProperty("borderColor")] public ColorValue _borderColor { get; set; } = ColorValue.MakeColor(Color.Transparent);
        [JsonProperty("borderWidth")] public float _borderWidth = 1f;

        [JsonProperty("border")] public Space _border { get; set; } = new Space(0);

        [JsonIgnore]
        public RectangleF _rectBorder = new RectangleF();

        [JsonProperty("backGroundFill")] public Fill _backgroundFill { get; set; } = Fill.SOLID;
        [JsonProperty("backGroundColor")] public ColorValue _backgroundColor { get; set; } = ColorValue.MakeColor(Color.White);

        #endregion

        public Style(SpriteFont font)
        {
            //_font = GFX.GFX._defaultFont!;
            _font = font;

            int oX = 0;
            int oY = 0;

            _skinGui = new SkinGui(GFX.GFX._defaultSkinGui!)
                .SetSkinGui(Position.MIDDLE, new Skin() { _rect = new Rectangle(8 + oX, 8 + oY, 8, 8) })

                .SetSkinGui(Position.TOP_CENTER, new Skin() { _rect = new Rectangle(8 + oX, 0 + oY, 8, 8) })
                .SetSkinGui(Position.BOTTOM_CENTER, new Skin() { _rect = new Rectangle(8 + oX, 16 + oY, 8, 8) })
                .SetSkinGui(Position.LEFT_CENTER, new Skin() { _rect = new Rectangle(0 + oX, 8 + oY, 8, 8) })
                .SetSkinGui(Position.RIGHT_CENTER, new Skin() { _rect = new Rectangle(16 + oX, 8 + oY, 8, 8) })

                .SetSkinGui(Position.TOP_LEFT, new Skin() { _rect = new Rectangle(0 + oX, 0 + oY, 8, 8) })
                .SetSkinGui(Position.TOP_RIGHT, new Skin() { _rect = new Rectangle(16 + oX, 0 + oY, 8, 8) })
                .SetSkinGui(Position.BOTTOM_LEFT, new Skin() { _rect = new Rectangle(0 + oX, 16 + oY, 8, 8) })
                .SetSkinGui(Position.BOTTOM_RIGHT, new Skin() { _rect = new Rectangle(16 + oX, 16 + oY, 8, 8) });
        }

        public Style Clone()
        {
            Style clone = (Style)MemberwiseClone();
            return clone;
        }
    }

    //public class NodeGui
    //{
    //    public string _label;
    //    public Window _window;
    //    public Style _style;
    //    //public SpriteFont _font;
    //    //public Input.Mouse _mouse;
    //    public bool _showFocus;
    //    public Node _nodeLimitRect;
    //    public RectangleF _limitRect;
    //    public bool _isLimitRect;

    //    public NodeGui Clone()
    //    {
    //        NodeGui nodeGui = (NodeGui)MemberwiseClone();
    //        return nodeGui;
    //    }

    //}

    public class Skin
    {
        public Rectangle _rect; // rect of the frame pattern
        public Vector2 _origin = new Vector2(); // offset of the skin
        public Vector2 _renderPos = new Vector2(); // Position of the skin render
        public int _skinW = 0; // Pattern repeat W
        public int _skinH = 0; // Pattern repeat H

        public void SetRender(float renderPosX, float renderPosY, float originX = 0, float originY = 0, int skinW = 0, int skinH = 0)
        {
            _renderPos.X = renderPosX;
            _renderPos.Y = renderPosY;
            _origin.X = originX;
            _origin.Y = originY;
            _skinW = skinW;
            _skinH = skinH;
        }
    }

    public class SkinGui
    {
        public Texture2D _pattern; // Texture of Skin Gui
        Dictionary<Position, Skin> _skins = new Dictionary<Position, Skin>();

        int _skinW = 0;
        int _skinH = 0;

        Rectangle _rectGui;
        Rectangle _rectSkin;

        public SkinGui(Texture2D pattern)
        {
            _pattern = pattern;
        }

        public SkinGui SetSkinGui(Position position, Skin skin)
        {
            _skins[position] = skin;
            return this;
        }

        void Update(Node node)
        {
            //Update Skin Gui
            if (_skins.Count > 0)
            {
                _rectGui = (Rectangle)node._rect;

                foreach (KeyValuePair<Position, Skin> skin in _skins)
                {
                    _rectSkin = skin.Value._rect;

                    _skinW = _rectGui.Width / _rectSkin.Width;
                    _skinH = _rectGui.Height / _rectSkin.Height;

                    if (skin.Key == Position.MIDDLE)
                    {
                        skin.Value.SetRender(0, 0, 0, 0, _skinW, _skinH);
                    }

                    if (skin.Key == Position.TOP_CENTER || skin.Key == Position.BOTTOM_CENTER)
                    {
                        if (skin.Key == Position.TOP_CENTER) skin.Value.SetRender(0, -_rectSkin.Height, 0, 0, _skinW, 1);
                        if (skin.Key == Position.BOTTOM_CENTER) skin.Value.SetRender(0, _rectGui.Height, 0, 0, _skinW, 1);
                    }
                    if (skin.Key == Position.LEFT_CENTER || skin.Key == Position.RIGHT_CENTER)
                    {
                        if (skin.Key == Position.LEFT_CENTER) skin.Value.SetRender(-_rectSkin.Width, 0, 0, 0, 1, _skinH);
                        if (skin.Key == Position.RIGHT_CENTER) skin.Value.SetRender(_rectGui.Width, 0, 0, 0, 1, _skinH);
                    }

                    if (skin.Key == Position.TOP_LEFT) skin.Value.SetRender(0, 0, -_rectSkin.Width, -_rectSkin.Height);
                    if (skin.Key == Position.TOP_RIGHT) skin.Value.SetRender(_rectGui.Width, 0, 0, -_rectSkin.Height);
                    if (skin.Key == Position.BOTTOM_LEFT) skin.Value.SetRender(0, _rectGui.Height, -_rectSkin.Width, 0);
                    if (skin.Key == Position.BOTTOM_RIGHT) skin.Value.SetRender(_rectGui.Width, _rectGui.Height, 0, 0);

                }

            }
        }

        public void Render(SpriteBatch batch, Node node, Color color)
        {
            Update(node);
            //Draw Skin Gui
            if (_skins.Count > 0)
            {
                _rectGui = (Rectangle)node.AbsRectF;

                foreach (KeyValuePair<Position, Skin> skin in _skins)
                {
                    _rectSkin = skin.Value._rect;

                    if (skin.Value._skinW + skin.Value._skinH > 0)
                    {
                        GFX.GFX.Mosaic(batch, _rectGui,
                            _rectGui.X + skin.Value._origin.X + skin.Value._renderPos.X,
                            _rectGui.Y + skin.Value._origin.Y + skin.Value._renderPos.Y,
                            skin.Value._skinW, skin.Value._skinH,
                            _pattern,
                            _rectSkin,
                            color);
                    }
                    else
                    {
                        batch.Draw(_pattern, new Vector2(_rectGui.X, _rectGui.Y) + skin.Value._origin + skin.Value._renderPos, _rectSkin, color);
                    }
                }

            }
        }

    }

    public static class Gui
    {
        public enum Events
        {
            NONE = 0,
            IS_FOCUS,
            IS_MOUSE_CLICK,
            ON_MOUSE_CLICK,
            IS_MOUSE_IN,
            ON_MOUSE_IN,
            IS_MOUSE_OUT,
            ON_MOUSE_OUT,
            IS_KEYDOWN,
            IS_KEYUP,
            ON_PRESS,
            IS_PRESS,
        }
        public class Message
        {
            public Node? _node;
            public string? _message;
        }

        public class Base : Node
        {
            #region Attributes

            //protected NodeGui _nodeGui = new NodeGui();

            // Label
            public string _label = "";

            // Custumizable Label;
            public string _labelTop = "";
            public string _labelBottom = "";
            public string _labelLeft = "";
            public string _labelRight = "";

            public float _offsetTop = 0f;
            public float _offsetBottom = 0f;
            public float _offsetLeft = 0f;
            public float _offsetRight = 0f;

            //public Window _window;
            public Style _style;
            public Style Style => _style;
            //public SpriteFont _font;
            //public Input.Mouse _mouse;

            public Node? _nodeLimitRect = null;
            public RectangleF _limitRect = new RectangleF();
            public bool _isLimitRect = false;

            protected Input.MouseControl _mouse;

            bool _showSkin = true;
            bool _showOver = true;
            bool _isShowContent = true;
            public bool IsShowContent => _isShowContent;
            protected bool _isMouseOver = false;
            public bool IsMouseOver => _isMouseOver;
            public Addon.Draggable _drag;
            public Addon.Resizable _resize;

            //protected Event _event = new Event();
            //protected Action<Event> _eventCallBack = null;

            protected Queue<Message> _messages = new Queue<Message>(); // Message queue
            protected Action<Message>? _messageProc = null; // Messages process

            //protected Style _style = new Style();
            //protected bool _activeSkin = true;
            protected float _opacity = 1f;

            // Debug double ON_PRESS !    
            protected bool _onPress = false;

            #endregion

            public Node ShowContent() { _isShowContent = true; return this; }
            public Node HideContent() { _isShowContent = false; return this; }
            public Base(Input.MouseControl mouse, Style style)
            {
                
                _type = UID.Get<Gui.Base>();

                //_nodeGui = nodeGui.Clone();
                //_style = nodeGui._style;
                _mouse = mouse;
                _style = style;

                //Attach<Addon.Draggable>();
                //Attach<Addon.Resizable>();

                //_drag = Get<Addon.Draggable>();
                //_resize = Get<Addon.Resizable>();

                _drag = new Addon.Draggable(this, _mouse);
                _resize = new Addon.Resizable(this, _mouse);

                _resize.Init(6);

            }

            // Parent & Childs Management
            public override Node AppendTo(Node parent) // Child Append to Parent
            {
                if (null != parent)
                {
                    _isAppend = true;
                    _parent = parent;
                    _parent.Add(this);

                    if (_parent._type == UID.Get<Base>())
                        _style = _parent.This<Base>()._style;
                }

                UpdateRect();
                return this;
            }
            public override Node Append(Node child) // Parent Append Child
            {
                if (null != child)
                {
                    child._isAppend = true;
                    _parent = this;
                    Add(child);

                    if (child._type == UID.Get<Base>())
                        child.This<Base>()._style = _style;
                }

                UpdateRect();
                return this;
            }

            public Node SetMouse(Input.MouseControl mouse)
            {
                _mouse = mouse;
                return this;
            }
            public Node SetClickable(bool isClickable)
            {
                _navi._isClickable = isClickable;
                return this;
            }
            public Node SetFocusable(bool focusable)
            {
                if (null != _navi)
                    _navi._isFocusable = focusable;

                return this;
            }

            // Style Accessors
            public Node SetAlpha(float alpha)
            {
                _opacity = alpha;
                return this;
            }
            public Node SetStyle(Style style)
            {
                if (null != style)
                {
                    _style = style;
                    _style._rect = _rect;
                }

                return this;
            }
            [Obsolete]
            public Style GetStyle()
            {
                return _style;
            }


            // Message methods
            public Node OnMessage(Action<Message> messageProc)
            {
                _messageProc = messageProc;
                return this;
            }
            public Node RunMessageProc(Message message)
            {
                _messageProc?.Invoke(message);
                return this;
            }
            public Node PostMessage(Message message)
            {
                _messages.Enqueue(message);
                return this;
            }
            public Node PostMessage(Node node, string message)
            {
                PostMessage(new Message { _node = node, _message = message });
                return this;
            }
            public Node PostMessage(string message)
            {
                PostMessage(new Message { _node = this, _message = message });
                return this;
            }

            //public override Node Clone()
            //{
            //    //Window clone = (Window)MemberwiseClone();
            //    Base clone = (Base)base.Clone();
            //    //clone._drag = (Addon.Draggable)_drag.Clone(clone);
            //    //clone._resize = (Addon.Resizable)_resize.Clone(clone);
            //    clone._drag = _drag.Clone<Addon.Draggable>(clone);
            //    clone._resize = _resize.Clone<Addon.Resizable>(clone);
            //    //clone._nodeGui = _nodeGui.Clone();

            //    foreach (Node node in clone.GroupOf(UID.Get<Gui.Base>()))
            //    {
            //        Base window = (Base)node;

            //        if (null != window._drag)
            //        {
            //            if (ReferenceEquals(window._drag._containerNode, this))
            //                window._drag.SetLimitRect(clone);
            //            else
            //            {
            //                //if (null != window._drag._dragRect)
            //                window._drag.SetDragRect(Gfx.CloneAbsRectF(window._drag._dragRect));
            //                window._drag.SetLimitRect(Gfx.CloneAbsRectF(window._drag._dragRect));
            //            }
            //        }
            //    }


            //    return clone;
            //}

            public Base ShowSkin(bool showSkin = true)
            {
                _showSkin = showSkin;
                return this;
            }
            public Base ShowOver(bool showOver = true)
            {
                _showOver = showOver;
                return this;
            }
            public Base SetLabel(string label)
            {
                _label = label;
                return this;
            }
            public RectangleF GetLabelRect()
            {
                float w = _style._font.MeasureString(_label).X;
                float h = _style._font.MeasureString(_label).Y;

                RectangleF rectLabel = new RectangleF(_x, _y, w, h);

                if (_style._verticalAlign == Style.VerticalAlign.Top) { rectLabel.Y = _y; SetPivotY(0); }
                if (_style._verticalAlign == Style.VerticalAlign.Middle) { rectLabel.Y = _y - h / 2; SetPivotY(h / 2); }
                if (_style._verticalAlign == Style.VerticalAlign.Bottom) { rectLabel.Y = _y - h; SetPivotY(h); }

                if (_style._horizontalAlign == Style.HorizontalAlign.Left) { rectLabel.X = _x; SetPivotX(0); }
                if (_style._horizontalAlign == Style.HorizontalAlign.Center) { rectLabel.X = _x - w / 2; SetPivotX(w / 2); }
                if (_style._horizontalAlign == Style.HorizontalAlign.Right) { rectLabel.X = _x - w; SetPivotX(w); }


                return rectLabel;
            }
            public Base SetLabelTop(string labelTop, float offsetTop = 0f)
            {
                _labelTop = labelTop;
                _offsetTop = offsetTop;
                return this;
            }
            public Base SetLabelBottom(string labelBottom, float offsetBottom = 0f)
            {
                _labelBottom = labelBottom;
                _offsetBottom = offsetBottom;
                return this;
            }
            public Base SetLabelLeft(string labelLeft, float offsetLeft = 0f)
            {
                _labelLeft = labelLeft;
                _offsetLeft = offsetLeft;
                return this;
            }
            public Base SetLabelRight(string labelRight, float offsetRight = 0f)
            {
                _labelRight = labelRight;
                _offsetRight = offsetRight;
                return this;
            }
            public override Node Init()
            {
                InitAction();
                return this;
            }
            public override Node Update(GameTime gameTime)
            {
                UpdateAction();

                UpdateRect();
                SortZAscending();
                UpdateChildsSort(gameTime);



                // Poll Event 
                //_navi._onClick = false;
                _navi._isClick = false;
                _navi._isMouseOver = false;
                _isMouseOver = false;

                _resize.Update(gameTime);
                _drag.Update(gameTime);


                _isMouseOver = _navi._isClickable && Collision2D.PointInRect
                (
                    new Vector2
                    (
                        _mouse._x - (_parent != null ? _parent._rect.X : 0),
                        _mouse._y - (_parent != null ? _parent._rect.Y : 0)
                    ),
                    _rect
                )
                && !_mouse._isOverAny;


                if (_isMouseOver)
                {
                    if (null != _parent)
                    {
                        if (null != _parent._naviGate)
                        {
                            if (_parent._naviGate.IsNaviGate)
                            {
                                if (!_navi._isMouseOver) PostMessage("ON_MOUSE_OVER");

                                PostMessage("IS_MOUSE_IN");
                                PostMessage("IS_MOUSE_OVER");

                                _mouse._isOverAny = true;
                                _navi._isMouseOver = true;

                                //Console.Write("<Mouse over NodeGui " + _index + " >");

                                if (_mouse._onClick && !_mouse._isActiveReSize)// && !_nodeGui._mouse._isMove)
                                {
                                    //Console.Write("<Window is clicked " + _index + " >");
                                    _navi._onClick = _mouse._onClick;

                                    if (_navi._onClick)
                                        PostMessage("ON_CLICK");

                                    _navi._isClick = true;
                                    PostMessage("IS_CLICK");
                                }
                            }
                        }
                    }

                }


                if (null != _parent)
                {
                    _parent.GotoFront(_index, UID.Get<Base>());  // Move to Front Z order !!

                    if (null != _parent._naviGate)
                    {

                        if (_navi._isClick && _navi._isFocusable)
                        {
                            _parent._naviGate.SetNaviNodeFocusAt(_index);
                            PostMessage("IS_FOCUS");
                        }
                    }
                }

                while (_messages.Count > 0) // Message Process
                {
                    RunMessageProc(_messages.Peek());
                    _messages.Dequeue(); // Kill message 
                }

                return this;
            }
            public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
            {
                _style._rectBorder = RectangleF.Add
                (
                    AbsRectF,
                    new RectangleF
                    (
                        -_style._border._left,
                        -_style._border._top,
                        _style._border._left + _style._border._right,
                        _style._border._top + _style._border._bottom
                    )
                );
                _style._rectPadding = RectangleF.Add
                (
                    AbsRectF,
                    new RectangleF
                    (
                        _style._padding._left,
                        _style._padding._top,
                        -_style._padding._right * 2,
                        -_style._padding._bottom * 2
                    )
                );

                if (!_style._useSkin)
                {
                    // Draw normal 
                    Color color = new Color(0.5f, .5f, .5f, 0.8f);
                    GFX.GFX.FillRectangle(batch, (Rectangle)_style._rectBorder, _style._backgroundColor._value);

                    if (_navi._isMouseOver && _showOver)// || _isMouseOver)
                        GFX.GFX.Rectangle(batch, _style._rectBorder, _style._overColor._value, _style._borderWidth);
                    else
                        GFX.GFX.Rectangle(batch, _style._rectBorder, _style._borderColor._value, _style._borderWidth);
                }
                else
                {
                    // Draw SkinGui
                    if (_showSkin)
                        _style?._skinGui?.Render(batch, this, Color.White * _opacity);

                }

                if (null != _parent)
                {
                    if (null != _parent._naviGate)
                    {
                        if (_parent._naviGate.IsNaviGate)
                        {

                            if (_navi._isFocus && _style!._showFocus && _navi._isFocusable)
                            {
                                RenderFocus(batch);
                            }

                            if (_navi._isMouseOver && _navi._isFocusable && _showOver)
                            {
                                RenderOver(batch);
                            }

                        }
                    }
                }

                GFX.GFX.String(batch, _style!._font, _label, AbsX + AbsRect.Width / 2 - _oX, AbsY + AbsRect.Height / 2 - _oY,
                    _style._color._value, _style._horizontalAlign, _style._verticalAlign, _style._textBorder, Style._colorTextBorder._value);


                // Labels
                GFX.GFX.String(batch, _style._font, _labelTop, AbsX + _rect.Width / 2 - _oX, AbsY + _offsetTop - _oY,
                    _style._color._value, Style.HorizontalAlign.Center, Style.VerticalAlign.Bottom, _style._textBorder, Style._colorTextBorder._value);

                GFX.GFX.String(batch, _style._font, _labelBottom, AbsX + _rect.Width / 2 - _oX, AbsY + _rect.Height + _offsetBottom - _oY,
                    _style._color._value, Style.HorizontalAlign.Center, Style.VerticalAlign.Top, _style._textBorder, Style._colorTextBorder._value);

                GFX.GFX.String(batch, _style._font, _labelLeft, AbsX + _offsetLeft - _oX, AbsY + _rect.Height / 2 - _oY,
                    _style._color._value, Style.HorizontalAlign.Right, Style.VerticalAlign.Middle, _style._textBorder, Style._colorTextBorder._value);

                GFX.GFX.String(batch, _style._font, _labelRight, AbsX + _rect.Width + _offsetRight - _oX, AbsY + _rect.Height / 2 - _oY,
                    _style._color._value, Style.HorizontalAlign.Left, Style.VerticalAlign.Middle, _style._textBorder, Style._colorTextBorder._value);


                RenderAction(batch);

                SortZDescending();

                _resize.Draw(batch, _mouse, _style._font);

                if (_isShowContent)
                    DrawChilds(batch, gameTime);

                return this;
            }
            protected virtual Node RenderFocus(SpriteBatch batch)
            {
                GFX.GFX.FillRectangle(batch, (Rectangle)_style._rectBorder, _style._focusBackgroundFillColor._value);
                GFX.GFX.Rectangle(batch, _style._rectBorder, _style._focusBorderColor._value, _style._borderWidth);

                //if (Node._showNodeInfo)
                //batch.DrawRectangle(Gfx.AddRect(_drag._limitRect, new Rectangle(-2,-2,4,4)), Color.LightPink, 2);

                return this;
            }

            protected virtual Node RenderOver(SpriteBatch batch)
            {
                GFX.GFX.FillRectangle(batch, (Rectangle)_style._rectBorder, _style._overBackgroundFillColor._value);
                GFX.GFX.Rectangle(batch, _style._rectBorder, _style._overBorderColor._value, _style._borderWidth);

                //if (Node._showNodeInfo)
                //batch.DrawRectangle(Gfx.AddRect(_drag._limitRect, new Rectangle(-2,-2,4,4)), Color.LightPink, 2);

                return this;
            }

        }

        [Obsolete]
        public class Label : Base
        {
            public Label(Input.MouseControl mouse, Style style) : base(mouse, style)
            {
                _subType = UID.Get<Gui.Label>();
            }

            public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
            {
                if (null != _style)
                    if (_style._textBorder)
                        GFX.GFX.CenterBorderedStringXY(batch, _style._font, _label,
                            AbsX + AbsRect.Width / 2 - _oX, AbsY + AbsRect.Height / 2 - _oY, _style._color._value, _style._colorTextBorder._value);
                    else
                        GFX.GFX.CenterStringXY(batch, _style._font, _label,
                            AbsX + AbsRect.Width / 2 - _oX, AbsY + AbsRect.Height / 2 - _oY, _style._color._value);

                return this;
            }
        }

        public class Text : Base
        {
            public Text(Input.MouseControl mouse, Style style) : base(mouse, style)
            {
                _subType = UID.Get<Gui.Text>();
            }

            public override Node Update(GameTime gameTime)
            {
                base.Update(gameTime);

                _rect = GetLabelRect();
                UpdateRect();

                return this;
            }

            public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
            {
                if (null != _style)
                    GFX.GFX.String(batch, _style._font, _label, AbsX, AbsY, _style._color._value,
                        _style._horizontalAlign, _style._verticalAlign, _style._textBorder, _style._colorTextBorder._value);

                return this;
            }
        }

        public class Image : Base
        {
            public Texture2D _image;
            public Rectangle _rectSrc; // Rect source of the image ! 
            public Color _color;

            public Image(Input.MouseControl mouse, Style style,Texture2D image, Color color, Rectangle rectSrc = default(Rectangle)) : base(mouse, style)
            {
                _subType = UID.Get<Gui.Image>();

                _image = image;
                _color = color;

                if (default(Rectangle) == rectSrc)
                    _rectSrc = _image.Bounds;
                else
                    _rectSrc = rectSrc;
            }

            public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
            {
                base.Draw(batch, gameTime, indexLayer);
                //Rectangle rect = Gfx.TranslateRect(AbsRect, new Point(-(int)_oX, -(int)_oY));
                Rectangle rect = AbsRect;

                if (null != _image)
                    batch.Draw(_image, rect, _rectSrc, _color);

                //Draw.Rectangle(batch, rect, _style._borderColor._value, 2);

                return this;
            }

        }

        public class Button : Base
        {
            public Button(Input.MouseControl mouse, Style style) : base(mouse, style)
            {
                _subType = UID.Get<Gui.Button>();
                _navi._isClickable = true;
            }

            //public override Node Update()
            //{

            //    return base.Update();
            //}
            //public override Node Render(SpriteBatch batch)
            //{
            //    base.Render(batch);

            //    //if (_navi._isClick)
            //    //    batch.DrawRectangle(AbsRectF(), Color.Green, 1);

            //    //Color color = Color.Black;

            //    //if (null != _style) color = _style._color;

            //    if (null != _style)
            //        if (_style._textBorder)
            //            Draw.CenterBorderedStringXY(batch, _style._font, _label, 
            //                AbsX + AbsRect.Width/2-_oX, AbsY + AbsRect.Height/2-_oY, _style._color._value, _style._colorTextBorder._value);
            //        else
            //            Draw.CenterStringXY(batch, _style._font, _label, 
            //                AbsX + AbsRect.Width/2-_oX, AbsY + AbsRect.Height/2-_oY, _style._color._value);

            //    //batch.DrawCircle((int)AbsX() + AbsRect().Width / 2, (int)AbsY() + AbsRect().Height / 2, 4, 4, Color.Red);

            //    return this;
            //}

        }

        public class Slider : Base
        {
            private float _prevValue = 0f;
            public float Value { get; private set; } = 0f;
            public int IndexGui { get; private set; } = 0;
            public Vector2 CursorPos = new Vector2();
            public float Percent { get; private set; }


            int _step = 0; // by default -1 : 0.0f -> 1.0f

            private float _maxValue = 1f;
            public float MaxValue => _maxValue;
            private float _division = 1f;
            private float _stepX;


            bool _customCursor = false;

            bool _isValueChange = false;

            bool _setChangeValue = false; // True if use SetValue one time !
            float _valueChanged = 0f;

            Base _cursor;
            public Base Cursor => _cursor;

            public Slider(Input.MouseControl mouse, Style style, Gui.Base? cursor = null, int step = 0) : base(mouse, style)
            {
                _subType = UID.Get<Gui.Slider>();

                _navi._isClickable = true;

                _cursor = cursor!;

                if (_cursor == null)
                {
                    _cursor = new Base(mouse, style);
                    //_cursor._style._useSkin = false;
                    _customCursor = false;
                }
                else
                    _customCursor = true;



                _cursor._drag.SetDraggable(true);
                _cursor.AppendTo(this);

                _step = step;

                //_cursor.Get<Addon.Draggable>().SetLimitRect(this);
            }

            //public override Node Clone()
            //{
            //    Slider clone = (Slider)MemberwiseClone();

            //    clone._cursor = (Gui.Base)_cursor.Clone();

            //    return clone;
            //}

            public Node SetValue(float value, bool setChangeValue = true)
            {
                _valueChanged = value;
                _setChangeValue = setChangeValue;

                if (!_setChangeValue)
                {
                    _maxValue = _rect.Width - _cursor._rect.Width;
                    _division = _maxValue * (1f / _step);

                    if (_step >= 1)
                        _cursor._x = value * _division - _cursor._rect.Width / 2;
                    else
                    {
                        if (value > 1f)
                            _cursor._x = value - _cursor._rect.Width / 2;
                        else
                            _cursor._x = value * _maxValue;
                    }

                    _cursor.Update(new GameTime());

                    //_prevValue = Value = _cursor._x / _maxValue;
                    Value = _cursor._x / _maxValue;

                }

                return this;
            }

            public void Move(float step = 1f)
            {
                if (_step >= 1)
                {
                    int index = IndexGui;
                    index += (int)step;

                    if (index <= 0) index = 0;
                    if (index >= _step) index = _step;

                    SetValue(index);
                }
                else
                {
                    float value = Value;
                    value += step;

                    if (value > 1f && _step == 0)
                    {
                        if (value <= 0) value = 0;
                        if (value >= _maxValue) value = _maxValue;

                        SetValue(value);
                    }
                    else
                    if (_step < 0)
                    {
                        if (value <= 0) value = 0;
                        if (value >= 1) value = 1;

                        SetValue(value);
                    }

                }


            }


            public override Node Update(GameTime gameTime)
            {
                base.Update(gameTime);

                if (!_customCursor)
                    _cursor.SetSize(_rect.Height * .75f, _rect.Height);

                if (_setChangeValue)
                {
                    SetValue(_valueChanged, false);
                }

                _cursor._drag.SetLimitRect(_rect);

                // Direct Move Cursor by click !
                if (_navi._isMouseOver)
                {
                    if (_mouse._onClick)
                    {
                        //PostMessage("ON_CLICK");
                        _cursor._x = _mouse._x - AbsX - _cursor._rect.Width / 2 + _oX;
                    }
                }

                if (Misc.PointInRect(_mouse._x, _mouse._y, AbsRect))
                //if (_isMouseOver)
                {
                    if (_mouse._mouseWheelUp)
                    {
                        if (_step >= 1) Move(-1);
                        if (_step < 0) Move(-.05f);
                        if (_step == 0) Move(-10f);
                    }
                    if (_mouse._mouseWheelDown)
                    {
                        if (_step >= 1) Move(1);
                        if (_step < 0) Move(.05f);
                        if (_step == 0) Move(10f);
                    }
                }


                // Calculate Value , Percent, Index & Cursor Pos
                _maxValue = _rect.Width - _cursor._rect.Width;

                _division = _maxValue * (1f / _step);

                if (_step >= 1) // _cursor._drag.IsDrag() )
                {
                    _stepX = (int)Math.Round(_cursor._x / _division) * _division;

                    if (!_cursor._drag._isDragged)
                    {
                        _cursor._x = _stepX;
                    }

                }

                if (_cursor._drag._onDragged) PostMessage("ON_DRAG_CURSOR");
                if (_cursor._drag._offDragged) PostMessage("OFF_DRAG_CURSOR");

                Value = _cursor._x / _maxValue;

                if (Value != _prevValue)
                {
                    _prevValue = Value;

                    PostMessage("IS_CHANGE");

                    _isValueChange = true;

                }

                if (!_cursor._drag._isDragged && _isValueChange)
                {
                    _isValueChange = false;
                    PostMessage("ON_CHANGE");
                }


                if (_step >= 1)
                {
                    IndexGui = (int)(Value * _step);

                    if (IndexGui <= 0) IndexGui = 0;
                    if (IndexGui >= _step) IndexGui = _step;
                }
                else
                {
                    if (_step > 0)
                    {
                        IndexGui = (int)(Value * _maxValue);

                        if (IndexGui <= 0) IndexGui = 0;
                        if (IndexGui >= 1) IndexGui = 1;
                    }
                    else
                    {
                        IndexGui = (int)Value;

                        if (IndexGui <= 0) IndexGui = 0;
                        if (IndexGui >= _maxValue) IndexGui = (int)_maxValue;
                    }


                }

                _cursor.Update(gameTime);

                Percent = Value * 100;

                CursorPos.X = AbsX + _cursor._x + _cursor._rect.Width / 2;
                CursorPos.Y = AbsY;

                return this;
            }

            public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
            {
                base.Draw(batch, gameTime, indexLayer);

                GFX.GFX.FillRectangle(batch, AbsRect, _style._backgroundColor._value);

                GFX.GFX.Line(batch,
                    AbsX + _cursor._rect.Width / 2 - _oX, AbsY + _rect.Height / 2 - _oY,
                    AbsX - _cursor._rect.Width / 2 + _rect.Width - _oX, AbsY + _rect.Height / 2 - _oY,
                    _style._borderColor._value, 3);

                GFX.GFX.Line(batch,
                    AbsX + _cursor._rect.Width / 2 - _oX, AbsY + _rect.Height / 2 - _oY,
                    _cursor.AbsX, AbsY + _rect.Height / 2 - _oY,
                    _style._overColor._value, 3);

                for (int i = 0; i <= _step; i++)
                {
                    GFX.GFX.Point(batch,
                        AbsX + i * _division + (_cursor._rect.Width / 2) - _oX, AbsY + _rect.Height / 2 - _oY,
                        5, i <= IndexGui ? _style._overColor._value : _style._color._value);
                }

                //Draw.Point(batch,
                //    AbsX + _stepX + (_cursor._rect.Width / 2), AbsY + _rect.Height / 2,
                //    6, Color.DeepPink);

                //Draw.Rectangle(batch, _rect, _style._borderColor._value, _navi._isOver ? 4 : 1);

                if (!_customCursor)
                    GFX.GFX.FillRectangle(batch, _cursor.AbsRect, _cursor._navi._isMouseOver ? _style._overColor._value : _style._color._value);
                else
                    _cursor.Draw(batch, gameTime, indexLayer);


                //Draw.String(batch, _style._font, _label, AbsX + _rect.Width / 2, AbsY - _style._font.LineSpacing, _style._color._value);

                //Draw.Rectangle(batch, _cursor.AbsRect, _style._borderColor._value, _cursor._navi._isOver ? 4 : 1);
                //_cursor.Render(batch);

                //Draw.FillRectangle(batch, _cursorPos, new Vector2(8, _rect.Height), _style._colorTextBorder._value);

                //Draw.String(batch, _style._font, Percent.ToString(), Cursor.X, Cursor.Y, _style._color._value,
                //    Style.HorizontalAlign.Center, Style.VerticalAlign.Bottom );


                //Draw.String(batch, _style._font, "Value = " + Value + " : " + Index, AbsX + _rect.Width / 2, AbsY + _rect.Height, _style._color._value,
                //    Style.HorizontalAlign.Center, Style.VerticalAlign.Top);

                return this;
            }

        }

        public class CheckBox : Base
        {
            bool _isChecked = false;
            public bool IsChecked => _isChecked;

            public CheckBox(Input.MouseControl mouse, Style style) : base(mouse, style)
            {
                _subType = UID.Get<Gui.CheckBox>();
                _navi._isClickable = true;
            }

            public override Node Update(GameTime gameTime)
            {
                base.Update(gameTime);

                if (_navi._isMouseOver)
                {
                    if (_mouse._onClick)
                    {
                        //PostMessage("ON_CLICK");
                        _isChecked = !_isChecked;

                        if (_isChecked)
                            PostMessage("ON_CHECK");
                        else
                            PostMessage("OFF_CHECK");

                    }
                }


                return this;
            }

            public override Node Draw(SpriteBatch batch, GameTime gameTime, int indexLayer)
            {
                base.Draw(batch, gameTime, indexLayer);

                if (_navi._isMouseOver)
                    GFX.GFX.FillRectangle(batch, AbsRect, _style._overColor._value * .4f);
                //else
                //    Draw.FillRectangle(batch, AbsRect, _style._borderColor._value * .4f);

                GFX.GFX.Rectangle(batch, AbsRect, _style._borderColor._value, 2);

                if (_isChecked)
                    GFX.GFX.FillRectangle(batch, RectangleF.Extend(AbsRect, -4), _style._color._value);

                //Draw.String(batch, _style._font, _label, AbsX + _rect.Width / 2 + 8, AbsY, _style._color._value, Style.HorizontalAlign.Left);

                return this;
            }
        }

    }
}
