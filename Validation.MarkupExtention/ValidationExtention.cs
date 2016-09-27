using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Linq;

using System.Windows.Threading;
using Validation.Contracts;

namespace Validation.MarkupExtention
{
    public class ValidateExtension : MarkupExtension
    {
        const int CACHE_LOCK_TIMEOUT_MS = 1000;

        Binding _binding;
        
        public ValidateExtension(Binding binding)
        {
            _binding = binding;
            //_countBinding.ValidatesOnDataErrors = true;
            if (_binding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
                throw new InvalidOperationException("You can't implement your own way of calling BindingExpression.UpdateSource() method, since it is used in the validation process.");

            if (ValidationCache.Instance.GetMappings(_binding.Path.Path).Count > 0)
                throw new InvalidOperationException(String.Format("You can't bind the same property to more than one framework element, as this will break the focus functionality. PropertyName={0}", _binding.Path.Path));

            if (_binding.UpdateSourceTrigger != UpdateSourceTrigger.PropertyChanged)
                _binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
        }

        static void WireUpDataContextEvents(FrameworkElement targetElement)
        {
            IProcessedStateProvider processedStateProvider = targetElement.DataContext as IProcessedStateProvider;

            foreach (var entry in ValidationCache.Instance.Cache)
            {
                processedStateProvider.AddMonitoredField(entry.BoundPropertyName);
            }

            // Do we really need to lock the cache here?
            ObjectCache.Instance.LockWithTimeout(CACHE_LOCK_TIMEOUT_MS);
            if (ObjectCache.Instance.ObjectExists(targetElement.DataContext))
                return;

            INotifyDataErrorOrder errorOrderProvider = targetElement.DataContext as INotifyDataErrorOrder;
            INotifyDataValidationStarted dataValidationStartedProvider = targetElement.DataContext as INotifyDataValidationStarted;

            if (processedStateProvider != null && errorOrderProvider != null && dataValidationStartedProvider != null)
            {
                ObjectCache.Instance.AddObject(targetElement.DataContext);
            }
            else
            {
                throw new ArgumentException("The provided DataContext do not inherit BaseViewModel");
            }

            WeakEventManager<INotifyDataValidationStarted, DataValidationStartedEventArgs>.AddHandler
                       (dataValidationStartedProvider, "DataValidationStarted", DataValidationStartedProvider_DataValidationStarted);
            WeakEventManager<INotifyDataErrorOrder, DataErrorOrderEventArgs>.AddHandler
                    (errorOrderProvider, "DataErrorOrderChanged", ErrorOrderProvider_DataErrorOrderChanged);

            
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                return null;

            // get the target of the extension from the IServiceProvider interface
            IProvideValueTarget ipvt = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            FrameworkElement targetElement = ipvt.TargetObject as FrameworkElement;
            if (targetElement == null)
                return null;

            if (ipvt.TargetProperty is DependencyProperty)
            {
                DependencyProperty dp = ipvt.TargetProperty as DependencyProperty;

               
                if (targetElement.DataContext != null)
                    WireUpDataContextEvents(targetElement);
                
                DataContextChangedEventManager.AddHandler(targetElement, OnDataContextChanged);
                
                string boundPropertyName = _binding.Path.Path;
                // add cache entries
                ValidationCache.Instance.Add(boundPropertyName, targetElement, dp.Name);
               
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var entries = ValidationCache.Instance.GetMappings(boundPropertyName);
                    foreach (var entry in entries)
                    {
                        WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler
                            (entry.Element, "GotFocus", Element_GotFocus);
                        WeakEventManager<FrameworkElement, RoutedEventArgs>.AddHandler
                            (entry.Element, "LostFocus", Element_LostFocus);

                    }
                }), DispatcherPriority.ContextIdle);
            }

            return _binding.ProvideValue(serviceProvider);
        }

        private static void OnDataContextChanged(object sender, EventArgs e)
        {
            WireUpDataContextEvents(sender as FrameworkElement);
        }
        
        private static void ErrorOrderProvider_DataErrorOrderChanged(object sender, DataErrorOrderEventArgs e)
        {
            // focus only the first element with failed validation
            if (e.ErrorOrder > 0)
                return;

            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var assocElements = ValidationCache.Instance.GetMappings(e.PropertyName);
                if (assocElements.Count > 0)
                {
                    if (assocElements[0].Element.Focusable && !(assocElements[0].Element.IsFocused || assocElements[0].Element.IsKeyboardFocused))
                    {
                        assocElements[0].Element.Focus();
                    }
                }

            }), DispatcherPriority.ContextIdle);
        }
        

        private static void Element_GotFocus(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var element = sender as FrameworkElement;
                var processedStateProvider = element.DataContext as IProcessedStateProvider;
                var mapping = ValidationCache.Instance.GetMapping(element);
                if (mapping != null)
                    processedStateProvider.SetMonitoredFieldVisited(mapping.BoundPropertyName);

            }), DispatcherPriority.ApplicationIdle);
        }

        private static void Element_LostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            var mappings = ValidationCache.Instance.GetMapping(fe);
            var descriptor = DependencyPropertyDescriptor.FromName(
                                                        mappings.DependencyPropertyName,
                                                        fe.GetType(),
                                                        fe.GetType());
            BindingExpression be = fe.GetBindingExpression(descriptor.DependencyProperty);
            be.UpdateSource();
            
        }


        private static void DataValidationStartedProvider_DataValidationStarted(object sender, DataValidationStartedEventArgs e)
        {
            if (e.FieldValidated)
                return;

            var cacheEntry = ValidationCache.Instance.GetMappings(e.PropertyName);
            if (cacheEntry == null || cacheEntry.Count == 0)
                return;
            FrameworkElement fe = cacheEntry[0].Element;
            var descriptor = DependencyPropertyDescriptor.FromName(
                                                        cacheEntry[0].DependencyPropertyName,
                                                        fe.GetType(),
                                                        fe.GetType());
            BindingExpression be = fe.GetBindingExpression(descriptor.DependencyProperty);
            be.UpdateSource();
        }

    }

}
