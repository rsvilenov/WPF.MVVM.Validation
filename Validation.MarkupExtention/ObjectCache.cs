using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validation.MarkupExtention
{
    public class ObjectCache
    {
        List<WeakReference<object>> Cache;

        ObjectCache()
        {
            Cache = new List<WeakReference<object>>();
        }

        static ObjectCache _instance;

        public static ObjectCache Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ObjectCache();
                return _instance;
            }
        }

        void Purge()
        {
            var collected = Cache.Where(x =>
            {
                object dc;
                x.TryGetTarget(out dc);
                return dc == null;
            }).ToList();
            
            foreach (var entry in collected)
                Cache.Remove(entry);
        }

        public void AddObject(object dataContext)
        {
            Purge();
            Cache.Add(new WeakReference<object>(dataContext));
        }

        public bool ObjectExists(object dataContext)
        {
            bool exists = false;
            foreach (var entry in Cache)
            {
                object dc;
                entry.TryGetTarget(out dc);
                if (dc == dataContext)
                {
                    exists = true;
                    break;
                }
            }
            return exists;
        }

    }
}
