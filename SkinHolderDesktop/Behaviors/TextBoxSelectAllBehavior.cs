using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SkinHolderDesktop.Behaviors;

public static class TextBoxSelectAllBehavior
{
    public static readonly DependencyProperty SelectAllOnFocusProperty =
        DependencyProperty.RegisterAttached(
            "SelectAllOnFocus",
            typeof(bool),
            typeof(TextBoxSelectAllBehavior),
            new PropertyMetadata(false, OnSelectAllOnFocusChanged));

    public static bool GetSelectAllOnFocus(DependencyObject obj)
    {
        return (bool)obj.GetValue(SelectAllOnFocusProperty);
    }

    public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
    {
        obj.SetValue(SelectAllOnFocusProperty, value);
    }

    private static void OnSelectAllOnFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        bool newValue = (bool)e.NewValue;

        if (d is TextBox textBox)
        {
            if (newValue)
            {
                textBox.GotFocus += TextBox_GotFocus;
                textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
            }
            else
            {
                textBox.GotFocus -= TextBox_GotFocus;
                textBox.PreviewMouseLeftButtonDown -= TextBox_PreviewMouseLeftButtonDown;
            }
        }
        
        if (d is PasswordBox passwordBox)
        {
            if (newValue)
            {
                passwordBox.GotFocus += PasswordBox_GotFocus;
                passwordBox.PreviewMouseLeftButtonDown += PasswordBox_PreviewMouseLeftButtonDown;
            }
            else
            {
                passwordBox.GotFocus -= PasswordBox_GotFocus;
                passwordBox.PreviewMouseLeftButtonDown -= PasswordBox_PreviewMouseLeftButtonDown;
            }
        }
    }

    private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox) textBox.SelectAll();
    }

    private static void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox textBox && !textBox.IsKeyboardFocusWithin)
        {
            textBox.Focus();
            e.Handled = true;
        }
    }

    private static void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox) passwordBox.SelectAll();
    }

    private static void PasswordBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is PasswordBox passwordBox && !passwordBox.IsKeyboardFocusWithin)
        {
            passwordBox.Focus();
            e.Handled = true;
        }
    }
}