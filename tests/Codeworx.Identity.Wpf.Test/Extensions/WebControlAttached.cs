using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Codeworx.Identity.Wpf.Test.ViewModel;

namespace Codeworx.Identity.Wpf.Test.Extensions
{
    public class WebControlAttached
    {
        public static readonly DependencyProperty BindableSourceProperty = DependencyProperty.RegisterAttached("BindableSource", typeof(Uri), typeof(WebControlAttached), new PropertyMetadata(null, OnBindableSourceChanged));

        public static readonly DependencyProperty NavigatingCommandProperty = DependencyProperty.RegisterAttached("NavigatingCommand", typeof(ICommand), typeof(WebControlAttached), new PropertyMetadata(null, OnNavigatingCommandChanged));

        public static Uri GetBindableSource(WebBrowser obj)
        {
            return (Uri)obj.GetValue(BindableSourceProperty);
        }

        public static ICommand GetNavigatingCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(NavigatingCommandProperty);
        }

        public static void SetBindableSource(WebBrowser obj, Uri value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void SetNavigatingCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(NavigatingCommandProperty, value);
        }

        private static void Browser_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            var args = new NavigatingArgs { Uri = e.Uri };

            var command = GetNavigatingCommand((WebBrowser)sender);

            if (command != null)
            {
                if (command.CanExecute(args))
                {
                    command.Execute(args);
                }
            }
        }

        private static void OnBindableSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser browser)
            {
                browser.Source = (Uri)e.NewValue;
            }
        }

        private static void OnNavigatingCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WebBrowser browser)
            {
                if (e.NewValue == null)
                {
                    browser.Navigating -= Browser_Navigating;
                }
                else
                {
                    browser.Navigating += Browser_Navigating;
                }
            }
        }
    }
}
