using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using FileCleanInfo;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Input;
using System;

namespace WorkManager
{
    public partial class Cleaner : Window
    {
        List<string> placeholdersAddToListTextBox = new List<string> { "Введите x. Тип очистки: <x></x>, </x>.", "Введите x. Тип очистки: x=\"\", x=[], x={}.", "Введите x. Тип очистки: x().", "Введите x. Тип очистки: на основе регулярного выражения." };
        string placeholderAddToListTextBox;
        public Cleaner()
        {
            InitializeComponent();
            fileDirectoryInputTextBox.Text = Directories.FileDirectoryInput.ToString();
            fileDirectoryOutputTextBox.Text = Directories.FileDirectoryOutput.ToString();
            CustomsDataGrid.ItemsSource = FileClean.Customs.ConvertAll(r => r.ToString());
            TagsDataGrid_GotFocus(null, null);
        }

        private void fileDirectoryInputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fileDirectoryInputTextBox = (TextBox)sender;
        }

        private void fileDirectoryOutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            fileDirectoryOutputTextBox = (TextBox)sender;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Directories.FileDirectoryInput = new DirectoryInfo(fileDirectoryInputTextBox.Text);
            Directories.FileDirectoryOutput = new DirectoryInfo(fileDirectoryOutputTextBox.Text);
            MessageBox.Show("Сохранено!");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TagsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(TagsDataGrid.ItemsSource).Refresh();           
        }

        private void AttributesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(AttributesDataGrid.ItemsSource).Refresh();
        }

        private void FunctionsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(FunctionsDataGrid.ItemsSource).Refresh();
        }

        private void CustomsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(CustomsDataGrid.ItemsSource).Refresh();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void TagsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedTextbox = e.EditingElement as TextBox;
            FileClean.Tags[e.Row.GetIndex()] = editedTextbox.Text;
        }
        private void AttributesDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedTextbox = e.EditingElement as TextBox;
            FileClean.Attributes[e.Row.GetIndex()] = editedTextbox.Text;
        }
        private void FunctionsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedTextbox = e.EditingElement as TextBox;
            FileClean.Functions[e.Row.GetIndex()] = editedTextbox.Text;
        }
        private void CustomsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedTextbox = e.EditingElement as TextBox;
            FileClean.Customs[e.Row.GetIndex()] = new Regex (editedTextbox.Text);
        }
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            switch(elementsControl.SelectedIndex)
            {
                case 0:
                    FileClean.Tags.Add(AddToListTextBox.Text);
                    CollectionViewSource.GetDefaultView(TagsDataGrid.ItemsSource).Refresh();
                    break;
                case 1:
                    FileClean.Attributes.Add(AddToListTextBox.Text);
                    CollectionViewSource.GetDefaultView(AttributesDataGrid.ItemsSource).Refresh();
                    break;
                case 2:
                    FileClean.Functions.Add(AddToListTextBox.Text);
                    CollectionViewSource.GetDefaultView(FunctionsDataGrid.ItemsSource).Refresh();
                    break;
                case 3:
                    FileClean.Customs.Add(new Regex (AddToListTextBox.Text));
                    CustomsDataGrid.ItemsSource = FileClean.Customs.ConvertAll(r => r.ToString());
                    break;
            }
            
            AddToListTextBox.Text = string.Empty;
            AddToListTextBox_LostFocus(null, null);

        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileCleanInfo.FileClean.GetAllFiles();
                MessageBox.Show("Обработка завершена.");
            }
            catch (Exception exc)
            {
                MessageBox.Show($"Ошибка: {exc}\nВыполнение остановлено.");
                throw;
            }

        }

        private void elementsControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(elementsControl.SelectedIndex)
            {
                case 0:
                    placeholderAddToListTextBox = placeholdersAddToListTextBox[0];
                    break;
                case 1:
                    placeholderAddToListTextBox = placeholdersAddToListTextBox[1];
                    break;
                case 2:
                    placeholderAddToListTextBox = placeholdersAddToListTextBox[2];
                    break;
                case 3:
                    placeholderAddToListTextBox = placeholdersAddToListTextBox[3];
                    break;
            }
            AddToListTextBox_LostFocus(null, null);
        }

        private void AddToListTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (AddToListTextBox.Text == placeholderAddToListTextBox)
            {
                AddToListTextBox.Text = "";
                AddToListTextBox.FontStyle = FontStyles.Normal;
                AddToListTextBox.Foreground = SystemColors.ControlTextBrush;
            }
        }

        private void AddToListTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
           if (string.IsNullOrWhiteSpace(AddToListTextBox.Text) || placeholdersAddToListTextBox.Contains(AddToListTextBox.Text))
            {
                AddToListTextBox.Text = placeholderAddToListTextBox;
                AddToListTextBox.FontStyle = FontStyles.Italic;
                AddToListTextBox.Foreground = SystemColors.GrayTextBrush;
            }

        }

        private void TagsDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void AttributesDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void FunctionsDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void CustomsDataGrid_GotFocus(object sender, RoutedEventArgs e)
        {


        }

        private void elementsControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
