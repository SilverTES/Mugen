using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Mugen.Core
{
    public abstract class State
    {
        public StateMachine? _stateMachine;
        public Node _node;

        public State(Node node)
        {
            _node = node;
        }
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Update(GameTime gameTime);
    }

    public class StateMachine
    {
        private State _curState;
        public State CurState => _curState;
        public StateMachine(State initialState)
        {
            _curState = initialState;
            _curState._stateMachine = this;
        }
        /// <summary>
        /// Force change currentState to newState
        /// </summary>
        /// <param name="newState"></param>
        public void SetState(State newState)
        {
            _curState.Exit();
            _curState = newState;
            _curState.Enter();
        }
        /// <summary>
        /// Change currentState to newState if newState is different than currentState
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(State newState)
        {
            if (newState != _curState)
                SetState(newState);
        }

        public void Update(GameTime gameTime)
        {
            _curState.Update(gameTime);
        }
    }
}
