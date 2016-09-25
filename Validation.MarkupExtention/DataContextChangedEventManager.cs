using System;
using System.ComponentModel;
using System.Windows;

namespace Validation.MarkupExtention
{
    public class DataContextChangedEventManager : WeakEventManager
    {
        public static void AddHandler(FrameworkElement source, EventHandler handler)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (handler == null)
                throw new ArgumentNullException("handler");

            CurrentManager.ProtectedAddHandler(source, handler);
        }

        public static void RemoveHandler(FrameworkElement source, EventHandler handler)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (handler == null)
                throw new ArgumentNullException("handler");

            CurrentManager.ProtectedRemoveHandler(source, handler);
        }

        private static DataContextChangedEventManager CurrentManager
        {
            get
            {
                Type managerType = typeof(DataContextChangedEventManager);
                DataContextChangedEventManager manager =
                        (DataContextChangedEventManager)GetCurrentManager(managerType);

                if (manager == null)
                {
                    manager = new DataContextChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }

                return manager;
            }
        }

        protected override void StartListening(object source)
        {
            FrameworkElement typedSource = (FrameworkElement)source;
            typedSource.DataContextChanged += TypedSource_DataContextChanged;
        }


        protected override void StopListening(object source)
        {
            FrameworkElement typedSource = (FrameworkElement)source;
            typedSource.DataContextChanged -= TypedSource_DataContextChanged;
        }

        private void TypedSource_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            base.DeliverEvent(sender, new DataContextChangedEventArgs(e.OldValue, e.NewValue));
        }

    }
}


