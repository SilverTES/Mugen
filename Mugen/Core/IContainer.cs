using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mugen.Core
{
    public class ZIndex
    {
        public bool _isAlive = true; // Help Garbage Collector management ! if not null then check if _isActive or not !!!

        public float _z = 0;
        public int _index = -1;
    }
    /// <summary>
    /// Base IContainer [object, object, object, ...] : Add, Delete, 
    /// </summary>
    /// <typeparam name="OBJECT"></typeparam>
    public class IContainer<OBJECT> where OBJECT : ZIndex
    {
        public List<OBJECT?> _objects = new List<OBJECT?>();
        public Stack<int> _freeObjects = new Stack<int>();

        public int Count()
        {
            return _objects.Count;
        }
        public int NbActive()
        {
            return _objects.Count - _freeObjects.Count;
        }
        public bool IsEmpty()
        {
            return _objects.Count == 0;
        }
        public void SetAt(int index, OBJECT obj)
        {
            if (index >= 0 && index < _objects.Count)
                _objects[index] = obj;
        }
        public OBJECT? At(int index)
        {
            if (index >= 0 && index < _objects.Count)
                return _objects[index];
            else
                return null;

        }

        void AddObject(OBJECT obj)
        {
            int index = _objects.Count;
            obj._index = index;
            _objects.Add(obj);
        }
        public OBJECT? Add(OBJECT obj)
        {
            if (null != obj)
            {
                if (_freeObjects.Count > 0)
                {
                    int freeChildIndex = _freeObjects.Pop();
                    //int freeChildIndex = _freeObjects.Peek();

                    obj._index = freeChildIndex;


                    if (null != _objects[freeChildIndex]) // If Garbage Collector haven't kill this one then Add new Element in list !
                    {
                        if (!_objects[freeChildIndex]!._isAlive)
                            AddObject(obj);
                    }
                    else
                    {
                        _objects[freeChildIndex] = obj;
                    }

                    //_freeObjects.Pop();
                }
                else
                {
                    AddObject(obj);
                }
            }

            return obj;
        }
        private void Kill(int index)
        {
            if (null != _objects[index]) _objects[index]!._isAlive = false;
            _objects[index] = null;
            _freeObjects.Push(index);
        }
        public void Delete(int index)
        {
            //Misc.log("Begin delete Object :" + index + " \n");
            if (_objects.Count > 0)
                if (index >= 0 && index < _objects.Count)
                {
                    if (null != _objects[index])
                    {
                        //if (null != _objects[index]) _objects[index]!._isAlive = false;
                        //_objects[index] = null;
                        //_freeObjects.Push(index);
                        Kill(index);

                    }
                }

        }
        public void Delete(OBJECT obj)
        {
            if (null != obj)
            {
                int index = obj._index;

                //if (null != _objects[index]) _objects[index]!._isAlive = false;
                //_objects[index] = null;
                //_freeObjects.Push(index); // Add index as free !
                Kill(index);

            }
        }
        public OBJECT? First()
        {
            return _objects.First();
        }
        public OBJECT? Last()
        {
            return _objects.Last();
        }
        public int FirstId()
        {
            return 0;
        }
        public int LastId()
        {
            return _objects.Count - 1;
        }
        public int FirstActiveId()
        {
            int id = FirstId();
            while (null == _objects[id])
            {
                ++id;
            }
            return id;
        }
        public int LastActiveId()
        {
            int id = LastId();
            while (null == _objects[id])
            {
                --id;
            }
            return id;
        }

    }
}
