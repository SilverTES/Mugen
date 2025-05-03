using Microsoft.Xna.Framework;
using Mugen.Core;

namespace Mugen.Event
{
    namespace Message
    {
        /// <summary>
        /// Interface de base pour tous les messages.
        /// </summary>
        public interface IMessage
        {
            // Peut être étendu avec des propriétés comme un ID ou un horodatage si nécessaire
        }

        public class MessageBus
        {
            // Singleton
            private static MessageBus? _instance;
            public static MessageBus Instance => _instance ??= new MessageBus();

            // Abonnés par type de message
            private readonly Dictionary<Type, List<Action<IMessage>>> _subscribers;
            // File d'attente pour les messages immédiats
            private readonly Queue<(IMessage message, float delay)> _messageQueue;
            // Messages différés
            private readonly List<(IMessage message, float delay)> _delayedMessages;

            public MessageBus()
            {
                _subscribers = new Dictionary<Type, List<Action<IMessage>>>();
                _messageQueue = new Queue<(IMessage message, float delay)>();
                _delayedMessages = new List<(IMessage message, float delay)>();
            }

            /// <summary>
            /// S'abonner à un type de message spécifique.
            /// </summary>
            public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage
            {
                Type messageType = typeof(TMessage);
                if (!_subscribers.ContainsKey(messageType))
                {
                    _subscribers[messageType] = new List<Action<IMessage>>();
                }
                _subscribers[messageType].Add(message => callback((TMessage)message));
            }

            /// <summary>
            /// Se désabonner d'un type de message.
            /// </summary>
            public void Unsubscribe<TMessage>(Action<TMessage> callback) where TMessage : IMessage
            {
                Type messageType = typeof(TMessage);
                if (_subscribers.ContainsKey(messageType))
                {
                    _subscribers[messageType].RemoveAll(action =>
                    {
                        // Comparer les délégués en extrayant la méthode cible
                        return action.Target == callback.Target && action.Method == callback.Method;
                    });
                }
            }

            /// <summary>
            /// Envoyer un message (immédiat ou différé).
            /// </summary>
            public void SendMessage<TMessage>(TMessage message, float delay = 0f) where TMessage : IMessage
            {
                _messageQueue.Enqueue((message, delay));
            }

            /// <summary>
            /// Traiter les messages (appelé dans Update).
            /// </summary>
            public void ProcessMessages(GameTime gameTime)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Mettre à jour les messages différés
                for (int i = _delayedMessages.Count - 1; i >= 0; i--)
                {
                    var (message, delay) = _delayedMessages[i];
                    delay -= deltaTime;
                    if (delay <= 0f)
                    {
                        DispatchMessage(message);
                        _delayedMessages.RemoveAt(i);
                    }
                    else
                    {
                        _delayedMessages[i] = (message, delay);
                    }
                }

                // Traiter la file d'attente
                while (_messageQueue.Count > 0)
                {
                    var (message, delay) = _messageQueue.Dequeue();
                    if (delay > 0f)
                    {
                        _delayedMessages.Add((message, delay));
                    }
                    else
                    {
                        DispatchMessage(message);
                    }
                }
            }

            private void DispatchMessage(IMessage message)
            {
                Type messageType = message.GetType();
                if (_subscribers.ContainsKey(messageType))
                {
                    var subscribers = new List<Action<IMessage>>(_subscribers[messageType]);
                    foreach (var subscriber in subscribers)
                    {
                        subscriber?.Invoke(message);
                    }
                }
            }

            /// <summary>
            /// Réinitialiser le bus.
            /// </summary>
            public void Clear()
            {
                _subscribers.Clear();
                _messageQueue.Clear();
                _delayedMessages.Clear();
            }
        }

        
    }
}
