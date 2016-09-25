using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Validation.MarkupExtention
{
    public class ValidationCache
    {
        private static ValidationCache _instance;
        public static ValidationCache Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ValidationCache();
                return _instance;
            }
        }

        public SynchronizedCollection<PropertyMapping> Cache
        {
            get;
            private set;
        }

        public ValidationCache()
        {
            Cache = new SynchronizedCollection<PropertyMapping>();
        }

        public void Add(string boundPropName, FrameworkElement element, string depPropName)
        {
            Purge();

            bool exists = Cache.Any(x => x.Element == element);

            if (!exists)
                Cache.Add(new PropertyMapping(boundPropName, depPropName, element));

        }

        public PropertyMapping GetMapping(FrameworkElement element)
        {
            PropertyMapping propMapping = null;

            foreach (var entry in Cache)
            {
                if (entry.Element == element)
                    propMapping = entry;
            }

            return propMapping;
        }

        public List<PropertyMapping> GetMappings(string propertyName)
        {
            var elements = new List<PropertyMapping>();

            foreach (var entry in Cache)
            {
                if (entry.Element != null && entry.BoundPropertyName == propertyName)
                {
                    elements.Add(entry);
                }
            }

            return elements;
        }

        void Purge()
        {
            var collectedElements =
                Cache.Where(x => x.Element == null);

            foreach (var el in collectedElements)
            {
                Cache.Remove(el);
            }
        }    
        
        public class PropertyMapping
        {
            public PropertyMapping(string boundPropertyName, string dependencyPropertyName, FrameworkElement element)
            {
                BoundPropertyName = boundPropertyName;
                DependencyPropertyName = dependencyPropertyName;
                ElementRef = new WeakReference<FrameworkElement>(element);
            }

            WeakReference<FrameworkElement> ElementRef { get; }

            public string BoundPropertyName { get; }
            public string DependencyPropertyName { get; }
            
            public FrameworkElement Element
            {
                get
                {
                    FrameworkElement el;
                    ElementRef.TryGetTarget(out el);
                    return el;
                }
            }
        }
    }
}
