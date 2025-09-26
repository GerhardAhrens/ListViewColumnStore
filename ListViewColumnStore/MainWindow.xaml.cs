//-----------------------------------------------------------------------
// <copyright file="MainWindow.cs" company="Lifeprojects.de">
//     Class: MainWindow
//     Copyright © Lifeprojects.de 2025
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>24.09.2025 19:07:56</date>
//
// <summary>
// MainWindow mit Minimalfunktionen
// </summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;

namespace ListViewColumnStore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            this.InitializeComponent();
            WeakEventManager<Window, RoutedEventArgs>.AddHandler(this, "Loaded", this.OnLoaded);
            WeakEventManager<Window, CancelEventArgs>.AddHandler(this, "Closing", this.OnWindowClosing);

            this.WindowTitel = "Minimal WPF Template";
            this.DataContext = this;
        }

        private string _WindowTitel;

        public string WindowTitel
        {
            get { return _WindowTitel; }
            set
            {
                if (this._WindowTitel != value)
                {
                    this._WindowTitel = value;
                    this.OnPropertyChanged();
                }
            }
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            WeakEventManager<Button, RoutedEventArgs>.AddHandler(this.BtnCloseApplication, "Click", this.OnCloseApplication);

            this.LoadGridLaylout(this.PeopleListView.Name);

            People p = new People();
            int id = 1;
            p.Add(new Person() { Id = id++, FirstName = "Snoopy", LastName = string.Empty, Aktiv = true });
            p.Add(new Person() { Id = id++, FirstName = "Woodstock", LastName = string.Empty, Aktiv = true });
            p.Add(new Person() { Id = id++, FirstName = "Donald", LastName = "Duck", Aktiv = true });
            p.Add(new Person() { Id = id++, FirstName = "Micky", LastName = "Maus" , Aktiv = true });
            this.PeopleListView.ItemsSource = p;

        }

        private void OnCloseApplication(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;

            MessageBoxResult msgYN = MessageBox.Show("Wollen Sie die Anwendung beenden?", "Beenden", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (msgYN == MessageBoxResult.Yes)
            {
                this.SaveGridLayout(this.PeopleListView.Name);
                App.ApplicationExit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void LoadGridLaylout(string gridLayoutName)
        {
            string filename = Serializer.SettingsDirectory(gridLayoutName);

            if (File.Exists(filename))
            {
                bool deserializationSucceeded = true;
                List<String> columnOrder = null;
                try
                {
                    columnOrder = Serializer.FromJson<List<string>>(filename);
                }
                catch (Exception)
                {
                    deserializationSucceeded = false;
                }

                if (deserializationSucceeded && columnOrder != null && columnOrder.Count > 0)
                {
                    int newIndex = 0;
                    foreach (var colName in columnOrder)
                    {
                        string colHeader = string.Empty;
                        double colWidth = 0;

                        if (colName.Split('~').Length == 1)
                        {
                            colHeader = colName.Split('~')[0];
                        }
                        else if (colName.Split('~').Length == 2)
                        {
                            colHeader = colName.Split('~')[0];
                            colWidth = Convert.ToDouble(colName.Split('~')[1], CultureInfo.CurrentCulture);
                        }

                        int oldIndex = 0;
                        for (int i = 0; i < this.PeopleGridView.Columns.Count; i++)
                        {
                            if (this.PeopleGridView.Columns[i].Header.ToString().Equals(colHeader, StringComparison.Ordinal))
                            {
                                if (colWidth > 0)
                                {
                                    this.PeopleGridView.Columns[i].Width = colWidth;
                                }

                                oldIndex = i;
                                break;
                            }
                        }

                        this.PeopleGridView.Columns.Move(oldIndex, newIndex++);
                    }
                }
            }
        }

        private void SaveGridLayout(string gridLayoutName)
        {
            string filename = Serializer.SettingsDirectory(gridLayoutName);

            if (Directory.Exists(Path.GetDirectoryName(filename)) == false)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
            }

            List<String> columnOrder = new List<string>();
            foreach (GridViewColumn col in this.PeopleGridView.Columns)
            {
                string column = $"{col.Header}~{col.Width}";
                columnOrder.Add(column);
            }

            Serializer.ToJson(columnOrder, filename);
        }

        #region INotifyPropertyChanged implementierung
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            var e = new PropertyChangedEventArgs(propertyName);
            handler(this, e);
        }
        #endregion INotifyPropertyChanged implementierung

        public class Person
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public bool Aktiv { get; set; }
        }

        public class People : List<Person>
        {
        }
    }
}