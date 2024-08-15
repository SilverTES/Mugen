using Microsoft.Xna.Framework;


namespace Mugen.Input
{
        // Manage button delay/tempo
        public class ButtonControl
        {
            public class State
            {
                public bool _noRepeat = false; // noRepeat button
                public bool _onePress = false; // button once pressed no delay;
                public bool _repeat = false;  // repeat the button
                public bool _onPress = false; // button on pressed with delay
                public bool _isPress = false; // button is pressed no delay
                public int _tempoToRepeat = 0; // tempo before repeat button
                public int _tempoRepeat = 0;   // tempo for repeat button

                //public ButtonEvent Clone()
                //{
                //    ButtonEvent clone = (ButtonEvent)MemberwiseClone();

                //    return clone;
                //}
            };

            public static Dictionary<string, State> _mapButtonState = new Dictionary<string, State>();

            //public Button Clone()
            //{
            //    Button clone = (Button)MemberwiseClone();

            //    clone._mapButtonEvent = new Dictionary<string, ButtonEvent>();

            //    foreach (KeyValuePair<string, ButtonEvent> it in _mapButtonEvent)
            //    {
            //        clone._mapButtonEvent[it.Key] = it.Value.Clone();
            //    }

            //    return clone;
            //}

            public static bool OnePress(string name, bool button)
            {
                if (!_mapButtonState.ContainsKey(name))
                    _mapButtonState.Add(name, new State());

                if (button)
                {
                    if (!_mapButtonState[name]._noRepeat)
                    {
                        _mapButtonState[name]._noRepeat = true;
                        _mapButtonState[name]._onePress = true;
                    }
                    else
                    {
                        _mapButtonState[name]._onePress = false;
                    }
                }
                else
                {
                    _mapButtonState[name]._noRepeat = false;
                    _mapButtonState[name]._onePress = false;
                }

                return _mapButtonState[name]._onePress;
            }
            public static bool OnePress(string name)
            {
                return _mapButtonState[name]._onePress;
            }
            public static bool OnPress(string name, bool button, int delayToRepeat = 30, int delayRepeat = 4)
            {
                if (!_mapButtonState.ContainsKey(name))
                    _mapButtonState.Add(name, new State());

                _mapButtonState[name]._onPress = false;
                _mapButtonState[name]._isPress = button;

                if (button)
                {
                    if (_mapButtonState[name]._tempoToRepeat == 0)
                        _mapButtonState[name]._onPress = true;

                    ++_mapButtonState[name]._tempoToRepeat;

                    if (_mapButtonState[name]._repeat)
                    {
                        ++_mapButtonState[name]._tempoRepeat;
                        if (_mapButtonState[name]._tempoRepeat > delayRepeat)
                        {
                            _mapButtonState[name]._onPress = true;
                            _mapButtonState[name]._tempoRepeat = 0;
                        }
                    }
                }
                else
                {
                    _mapButtonState[name]._tempoToRepeat = 0;
                }


                if (_mapButtonState[name]._tempoToRepeat > delayToRepeat)
                {
                    if (delayToRepeat > 0) // if -1 then no repeat
                        _mapButtonState[name]._repeat = true;
                }
                else
                {
                    _mapButtonState[name]._repeat = false;
                }

                return _mapButtonState[name]._onPress;
            }
            public static bool OnPress(string name)
            {
                if (_mapButtonState.ContainsKey(name))
                    return _mapButtonState[name]._onPress;
                else
                    return false;
            }
            public static bool IsPress(string name)
            {
                if (_mapButtonState.ContainsKey(name))
                    return _mapButtonState[name]._isPress;
                else
                    return false;
            }
        };

        public class MouseControl
        {
            // Position
            private Point _position;
            public int _x = 0;
            public int _y = 0;
            public int AbsX { get; private set; } = 0;
            public int AbsY { get; private set; } = 0;
            public int _prevX = 0;
            public int _prevY = 0;

            // Button
            public int _button = 0;
            public bool _lastIsClick = false;
            public bool _isClick = false; // status button is pressed
            public bool _onClick = false; // trigger button on Click
            public bool _offClick = false; // trigger button off Click

            // Event
            /// <summary>
            /// Should be true if mouse is over something
            /// </summary>
            public bool _isOverAny = false; // mouse is over something !
            public bool _isMove = false; // mouse is move or not !
            public bool _onMove = false; // trigger mouse move !
            public bool _offMove = false; // trigger mouse stop move !

            public bool _moveUP = false; // mouse is move UP or not !
            public bool _moveDOWN = false; // mouse is move DOWN or not !
            public bool _moveLEFT = false; // mouse is move LEFT or not !
            public bool _moveRIGHT = false; // mouse is move RIGHT or not !

            public bool _up = false;   // mouse is up or not !
            public bool _down = false; // mouse is down or not !

            // Draggable
            public bool _isActiveDrag = false; // mouse is drag or not !
            public bool _isActiveReSize = false; // mouse is resize or not !

            // Mouse Wheel Control
            public int _prevWheelValue = 0;
            public bool _mouseWheelUp = false;
            public bool _mouseWheelDown = false;

            public MouseControl Clone()
            {
                return (MouseControl)MemberwiseClone();
            }
            public void SetPosition(Point position)
            {
                _position = position;
                _x = _position.X;
                _y = _position.Y;
            }
            public Point GetPosition()
            {
                return _position;
            }

            public void Update(int mouseX, int mouseY, int mouseButton, int _scrollWheelValue = 0, int cameraX = 0, int cameraY = 0)
            {
                _mouseWheelUp = false;
                _mouseWheelDown = false;

                if (_scrollWheelValue > _prevWheelValue)
                {
                    _mouseWheelDown = true;
                    _prevWheelValue = _scrollWheelValue;
                }
                if (_scrollWheelValue < _prevWheelValue)
                {
                    _mouseWheelUp = true;
                    _prevWheelValue = _scrollWheelValue;
                }

                _position.X = _x = mouseX;
                _position.Y = _y = mouseY;
                
                AbsX = _x + cameraX;
                AbsY = _y + cameraY;

                // Check Mouse Move
                if (_prevX != _x || _prevY != _y)
                {
                    if (!_isMove) _onMove = true; else _onMove = false;

                    _isMove = true;

                    if (_y < _prevY) _moveUP = true;
                    if (_y > _prevY) _moveDOWN = true;
                    if (_x < _prevX) _moveLEFT = true;
                    if (_x > _prevX) _moveRIGHT = true;

                    _prevX = _x;
                    _prevY = _y;
                }
                else
                {
                    if (_isMove) _offMove = true; else _offMove = false;

                    _isMove = false;
                    _moveUP = false;
                    _moveDOWN = false;
                    _moveLEFT = false;
                    _moveRIGHT = false;
                }

                //_isMove = false;
                if (!_isClick) _isOverAny = false;
                _button = mouseButton;

                _lastIsClick = _isClick;
                _onClick = false;
                _offClick = false;



                if ((_button > 0) && !_down)
                {

                    _down = true;
                    //log("< Mouse DOWN >");
                }

                if (!(_button > 0) && !_up)
                {

                    _up = true;
                    //log("< Mouse UP >");
                }

                if (!(_button > 0))
                {
                    _isActiveDrag = false;
                    _isActiveReSize = false;
                    _down = false;
                    _isClick = false;
                }
                else
                {
                    _isClick = true;
                    _up = false;
                    //log("< Mouse Pressed >");
                }


                if (_isClick != _lastIsClick)
                {
                    _lastIsClick = _isClick;

                    if (_isClick)
                        _onClick = true;
                    else
                        _offClick = true;

                }
            }


        };
}
