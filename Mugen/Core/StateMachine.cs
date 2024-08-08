using Microsoft.Xna.Framework;

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
        public StateMachine(State initialState)
        {
            _curState = initialState;
            _curState._stateMachine = this;
        }

        public void ChangeState(State newState)
        {
            _curState.Exit();
            _curState = newState;
            _curState.Enter();
        }

        public void Update(GameTime gameTime)
        {
            _curState.Update(gameTime);
        }
    }
}
