using Mugen.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.GUI
{
    public class NaviState
    {
        Stack<Node> _stackNode = new Stack<Node>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstNodeState"> Need the root Node State </param>
        public Node CreateNaviState(Node firstNodeState)
        {
            PushState(firstNodeState);
            return firstNodeState;
        }
        public Stack<Node> GetStackNode()
        {
            return _stackNode;
        }
        public void ClearState()
        {
            _stackNode.Clear();
        }
        public Node PushState(Node node)
        {
            _stackNode.Push(node);
            return node;
        }
        public Node PopState()
        {
            if (_stackNode.Count > 1) // StackNode always contains one Node !
            {
                _stackNode.Pop();
            }

            return Current();
        }
        public Node Current()
        {
            return _stackNode.Peek();
        }
        public bool StateEmpty()
        {
            return _stackNode.Count == 0;
        }
        public int StateSize()
        {
            return _stackNode.Count;
        }
    }
}
