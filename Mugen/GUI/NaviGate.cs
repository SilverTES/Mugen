using Mugen.Core;
using Mugen.Physics;


namespace Mugen.GUI
{
    public class NaviGate
    {
        // Manage navigation between clips who contain focusable GUI
        Node _node; // node where the navigate is attached !
        bool _isNaviGate = false; // status : false : parent navigate , true : navigate this !
        public bool IsNaviGate => _isNaviGate;
        bool _onBackNaviGate = false; // trigger : true if can backNavigate !
        public bool OnBackNaviGate => _onBackNaviGate;
        public Node? _prevNaviGateNode = null; // if null current node is probably "root" !
        Node _prevFocusNaviNode = Node._nodeRoot; // if null current node is probably "root" !
        Position _prevDirection;

        int _idFocusNaviNode = -1; // current focused NaviNode  : -1 focus nothing !
        int _prevIdFocusNaviNode = -1; // previous Focused NaviNode !

        bool _onChangeFocus = false; // trigger : true if focus changed
        public bool OnChangeFocus => _onChangeFocus;
        //Input.Button.ButtonEvent _submitButton = new Input.Button.ButtonEvent();

        public NaviGate(Node node)
        {
            _node = node;
        }

        public Node SetNaviNodeFocusAt(int idFocusNaviNode)
        {
            _idFocusNaviNode = idFocusNaviNode;
            return _node;
        }
        public Node SetNaviGate(bool isNaviGate, Node? prevNaviGateNode = null, int idFocusNaviNode = -1)
        {
            _isNaviGate = isNaviGate;

            //Console.WriteLine("_isNavigate = " + _isNavigate);

            if (idFocusNaviNode != -1)
                _idFocusNaviNode = idFocusNaviNode;

            if (_isNaviGate)
            {
                // check if prevNaviClip is valid Navigable Clip !
                if (null != prevNaviGateNode)
                {
                    if (null != prevNaviGateNode._naviGate)
                    {
                        _prevNaviGateNode = prevNaviGateNode;
                        _prevNaviGateNode._naviGate.SetNaviGate(false);
                    }
                }
            }
            else
            {
                // --- Stop navigation , reset navi Node status !
                foreach (Node it in _node.GroupAll())
                {
                    if (null != it)
                    {
                        if (null != it._navi)
                        {
                            it._navi._onPress = false;
                            it._navi._isPress = false;
                            it._navi._isFocus = false;
                        }
                    }
                }

            }
            return _node;
        }
        public Node MoveNaviGateTo(Node toNode, Node? prevNaviGateNode = null, bool isNavigate = true, int idFocusNaviNode = -1)
        {
            if (null != toNode._naviGate)
            {
                _isNaviGate = false; // Very Important : Lose the navigatable move to child navigate !

                toNode._naviGate.SetNaviGate(isNavigate, prevNaviGateNode, idFocusNaviNode);
            }

            return _node;
        }
        public Node MoveNaviGateToFocusedChild(Node? prevNaviGateNode = null, int idFocusNaviNode = -1)
        {
            MoveNaviGateTo(FocusNaviNode(), prevNaviGateNode, true, idFocusNaviNode);

            //Console.WriteLine(" MoveNaviGateToFocusedChild "+ prevNaviGateNode);
            return _node;
        }
        public Node BackNaviGate(int idFocusNaviNode = -1)
        {
            if (null != _prevNaviGateNode)// && _isNaviGate)
            {
                SetNaviGate(false);
                //_prevNaviGateNode._navigate.SetNaviGate(true, this._node, idFocusNaviNode);
                MoveNaviGateTo(_prevNaviGateNode, _node, true, idFocusNaviNode);

                _onBackNaviGate = true;
            }
            return _node;
        }

        // Movement of focus
        public Node ToPrevNaviNode(int type)
        {
            if (_isNaviGate)
            {
                //_toPrev = true;
                _prevFocusNaviNode = _node._childs.At(_idFocusNaviNode)!;
                while (true)
                {
                    --_idFocusNaviNode;

                    if (_idFocusNaviNode < _node._childs.FirstId()) _idFocusNaviNode = _node._childs.LastActiveId();

                    if (null != _node._childs.At(_idFocusNaviNode))
                        if (null != _node._childs.At(_idFocusNaviNode)!._navi)
                            if (_node._childs.At(_idFocusNaviNode)!._navi._isFocusable)
                                if (_node._childs.At(_idFocusNaviNode)!._type == type)
                                    break;
                };
            }
            return _node;
        }
        public Node ToNextNaviNode(int type)
        {
            if (_isNaviGate)
            {
                //_toNext = true;
                _prevFocusNaviNode = _node._childs.At(_idFocusNaviNode)!;
                while (true)
                {
                    ++_idFocusNaviNode;

                    if (_idFocusNaviNode > _node._childs.LastId()) _idFocusNaviNode = _node._childs.FirstActiveId();

                    if (null != _node._childs.At(_idFocusNaviNode))
                        if (null != _node._childs.At(_idFocusNaviNode)!._navi)
                            if (_node._childs.At(_idFocusNaviNode)!._navi._isFocusable) // Test If Node _navi is Focusable
                                if (_node._childs.At(_idFocusNaviNode)!._type == type)
                                    break;
                };
            }
            return _node;
        }
        public Node PrevFocusNaviNode() { return _prevFocusNaviNode; }
        public Position PrevDirection() { return _prevDirection; }
        public Node ToDirection(Position direction)
        {
            if (_isNaviGate)
            {
                // Test if clip have NAVINODE component !
                if (null != _node._childs.At(_idFocusNaviNode)!._naviNodes)
                {
                    if (_node._childs.At(_idFocusNaviNode)!._navi._isFocusable) // Test If Node _navi is Focusable
                        if (_node._childs.At(_idFocusNaviNode)!._naviNodes.ContainsKey(direction))
                        {
                            _prevDirection = direction;
                            _prevFocusNaviNode = _node._childs.At(_idFocusNaviNode)!;

                            _idFocusNaviNode = _node._childs.At(_idFocusNaviNode)!._naviNodes[direction]._index;

                        }
                }
            }
            return _node;
        }
        // Focused Gui
        public Node FocusNaviNode() // return Node focused NaviNode !
        {
            return _node._childs.At(_idFocusNaviNode)!;
        }
        public bool SetSubmitButton(bool button)
        {
            return UpdateSubmitButton(button);
        }
        //public void UpdateSubmitMouse(Node node)
        //{
        //    if (null != node._parent)
        //    {
        //        if (null != node._parent._navigate)
        //            if (node._parent._navigate.IsNaviGate())
        //            {
        //                //if (_navi._isClick && _nodeGui._mouse._onClick)
        //                if (node._navi._onClick)
        //                {
        //                    node._parent._navigate.SetNaviNodeFocusAt(node._index);
        //                    //Console.WriteLine("--- ConfigGamePad clicked ! " + _index);
        //                    node._parent._navigate.SetSubmitButton(true);

        //                    if (FocusNaviNode()._type == UID.Get<NodeGui>())
        //                        FocusNaviNode().This<Gui.Window>().PostMessage(_node, "ON_CLICK");

        //                }
        //            }
        //    }
        //}
        public bool UpdateSubmitButton(bool button)
        {

            if (null != FocusNaviNode())
            {
                if (null != FocusNaviNode()._navi) // check if focused clip have _navi !
                {

                    FocusNaviNode()._navi._onPress = Input.ButtonControl.OnePress("SubmitButton", button);
                    FocusNaviNode()._navi._isPress = button;

                    if (FocusNaviNode()._type == UID.Get<Gui.Base>())
                    {
                        if (FocusNaviNode()._navi._onPress)
                            FocusNaviNode().This<Gui.Base>().PostMessage(FocusNaviNode(), "ON_PRESS");

                        if (FocusNaviNode()._navi._isPress)
                            FocusNaviNode().This<Gui.Base>().PostMessage(FocusNaviNode(), "IS_PRESS");

                    }

                }
            }

            return FocusNaviNode()._navi._onPress;

        }
        public bool OnSubmit()
        {
            if (null != FocusNaviNode())
            {
                if (null != FocusNaviNode()._navi) // check if focused clip have _navi !
                {
                    return FocusNaviNode()._navi._onPress;

                }
            }

            return false;
        }
        //public bool IsSubmit()
        //{
        //    return _submitButton._isPress;
        //}

        public void Update(int type)
        {
            _onBackNaviGate = false;

            if (_isNaviGate)
            {

                // Find focused GUI and set it !
                foreach (Node it in _node.GroupOf(type))
                {
                    if (null != it)
                    {
                        if (null != it._navi)
                        {
                            //it._navi._onPress = false;

                            if (it._index != _idFocusNaviNode)
                                it._navi._isFocus = false;
                            else
                                it._navi._isFocus = true;
                        }
                    }
                }

                if (_idFocusNaviNode != _prevIdFocusNaviNode && _isNaviGate)
                {
                    _onChangeFocus = true;
                    _prevIdFocusNaviNode = _idFocusNaviNode;
                }
                else
                    _onChangeFocus = false;
            }
        }

    }
}
