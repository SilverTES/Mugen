
namespace Mugen.Core
{
    public class ObjectPool<T> where T : Node
    {
        private readonly Queue<T> _pool;
        private readonly List<T> _activeObjects; // Liste pour suivre les objets actifs
        private readonly Func<T> _factory;
        private readonly int _initialSize;
        //private readonly Node _parent; // Stocke le node parent

        public ObjectPool(Node parent, Func<T> factory, int initialSize)
        {
            _pool = new Queue<T>();
            _activeObjects = new List<T>();
            _factory = factory;
            _initialSize = initialSize;
            //_parent = parent; // Sauvegarde l'instance de Game

            // Initialisation du pool
            for (int i = 0; i < initialSize; i++)
            {
                T obj = factory();
                obj._isActive = false; // Marquer comme inactif
                obj._parent = null; // Réinitialiser le parent
                _pool.Enqueue(obj);
            }
        }

        // Obtenir un objet du pool
        public T Get()
        {
            T obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                // Utiliser l'instance de Game stockée pour créer un nouvel objet
                obj = _factory();

                //Misc.Log($"New object created: {obj.GetType().Name}");
            }

            obj._isActive = true;
            _activeObjects.Add(obj); // Ajouter à la liste des objets actifs
            return obj;
        }

        // Retourner un objet au pool
        public void Return(T obj, Node parent)
        {
            obj.Init();
            obj._isActive = false;

            if (parent != null)
                parent.RemoveChild(obj); // Retirer de son parent

            obj._parent = null; // Réinitialiser le parent

            _activeObjects.Remove(obj); // Retirer de la liste des objets actifs
            _pool.Enqueue(obj);

            //Misc.Log($"Object returned to pool: {obj.GetType().Name}");
        }

        // Obtenir tous les objets actifs
        public IEnumerable<T> GetActiveObjects()
        {
            return _activeObjects; // Retourner la liste des objets actifs
        }
    }
}
