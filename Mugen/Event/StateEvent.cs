
namespace Mugen.Event
{
    /// <summary>
    /// Event System : ON ,OFF ,IS
    /// </summary>
    /// 
    public class StateEvent
    {
        bool[] _prevEvents;
        bool[] _events;
        
        /// <summary>
        /// Create new Event System
        /// </summary>
        /// <param name="nbEvents"> nb events to create </param>
        public StateEvent(int nbEvents)
        {
            _prevEvents = new bool[nbEvents];
            _events = new bool[nbEvents];
        }
        /// <summary>
        /// Clear all the events previous status
        /// </summary>
        public void BeginSetEvents()
        {
            for (int i = 0; i < _events.Length; ++i)
                _prevEvents[i] = _events[i];
        }
        /// <summary>
        /// Set the event boolean
        /// </summary>
        /// <param name="idEvent"> id of the event </param>
        /// <param name="isEvent"> boolean when the event is activate </param>
        public void SetEvent(int idEvent, bool isEvent)
        {
            _events[idEvent] = isEvent;
        }
        /// <summary>
        /// Trigger when event is down 
        /// </summary>
        /// <param name="idEvent"> id of the event </param>
        /// <returns></returns>
        public bool OnEvent(int idEvent)
        {
            return !_prevEvents[idEvent] && _events[idEvent];
        }
        /// <summary>
        /// Trigger when event is up
        /// </summary>
        /// <param name="idEvent"> id of the event </param>
        /// <returns></returns>
        public bool OffEvent(int idEvent)
        {
            return _prevEvents[idEvent] && !_events[idEvent];
        }
        /// <summary>
        /// Status when event is down
        /// </summary>
        /// <param name="idEvent"> is of the event</param>
        /// <returns></returns>
        public bool IsEvent(int idEvent)
        {
            return _events[idEvent];
        }
    }
}
