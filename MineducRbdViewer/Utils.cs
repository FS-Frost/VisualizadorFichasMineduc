using System;
using System.Windows;

namespace MineducRbdViewer {
    
    public static class Utils {        
        public static void ShowWarningMessage(string text) {
            MessageBox.Show(text.ToString(), "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        
        public static void ShowWarningMessage(string format, params object[] args) {
            var text = string.Format(format, args);
            MessageBox.Show(text, "Advertencia", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        
        public static void ShowErrorMessage(string text) {
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        public static void ShowErrorMessage(string format, params object[] args) {
            var text = string.Format(format, args);
            ShowErrorMessage(text);
        }
        
        public static void ShowInfoMessage(string text) {
            MessageBox.Show(text, "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        
        public static void ShowInfoMessage(string format, params object[] args) {
            var text = string.Format(format, args);
            ShowInfoMessage(text);
        }
        
        public static string QuoteString(string s) {
            if (s != null) {
                return string.Format("\"{0}\"", s);
            }

            return null;
        }

        public static void DoOnBackground(Action accion) {
            Application.Current.Dispatcher.BeginInvoke(accion);
        }
        
        public static void SetWindowMinSize(Window window) {
            window.MinWidth = window.Width;
            window.MinHeight = window.Height;
        }
    }
}
