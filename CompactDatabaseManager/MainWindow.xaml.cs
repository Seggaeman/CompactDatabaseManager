using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Debug = System.Diagnostics.Debug;
using File = System.IO.File;

namespace CompactDatabaseManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel ViewModel
        {
            get { return (MainWindowViewModel)this.DataContext; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void _informationSchemaTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TableTV tableTV = ((TreeView)sender).SelectedItem as TableTV;
            if (tableTV == null)
                ViewModel.TableContent = null;
            else
                ViewModel.UpdateDataGrid(tableTV.Name);
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog_L = new OpenFileDialog();
            openFileDialog_L.DefaultExt = "sdf";
            openFileDialog_L.Filter = "SQL Server CE Files (*.sdf)|*.sdf";
            if (openFileDialog_L.ShowDialog(this) == true)
            {
                bool openDatabaseResult = false;
                bool? dialogResult = true;
                PasswordDialog.password_S = null;
                while((openDatabaseResult = ViewModel.OpenDatabase(openFileDialog_L.FileName, PasswordDialog.password_S)) == false && dialogResult == true)
                {
                    PasswordDialog passwordDialog = new PasswordDialog();
                    dialogResult = passwordDialog.ShowDialog();
                }
            }
        }
    }
}
