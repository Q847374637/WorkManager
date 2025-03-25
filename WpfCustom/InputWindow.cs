using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;

namespace WpfCustom
{
    public partial class InputWindow : Window
    {
        public string InputValue { get; private set; }

        public InputWindow(string title, string prompt)
        {
            InitializeComponent();

            Title = title;
            promptLabel.Content = prompt;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            InputValue = inputTextBox.Text;
            DialogResult = true;
        }
    }
}
