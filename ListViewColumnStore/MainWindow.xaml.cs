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

            this.LoadColumnState();

            People p = new People();
            int id = 1;
            p.Add(new Person() { Id = id++, FirstName = "Snoopy", LastName = string.Empty });
            p.Add(new Person() { Id = id++, FirstName = "Woodstock", LastName = string.Empty });
            p.Add(new Person() { Id = id++, FirstName = "Donald", LastName = "Duck" });
            p.Add(new Person() { Id = id++, FirstName = "Micky", LastName = "Maus" });
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
                this.SaveColumnState();
                App.ApplicationExit();
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void LoadColumnState()
        {
            if (File.Exists(SettingsFile))
            {
                bool deserializationSucceeded = true;
                List<String> columnOrder = null;
                try
                {
                    columnOrder = Serializer.FromJson<List<string>>(SettingsFile);
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
                        int oldIndex = 0;
                        for (int i = 0; i < this.PeopleGridView.Columns.Count; i++)
                        {
                            if (this.PeopleGridView.Columns[i].Header.ToString().Equals(colName, StringComparison.Ordinal))
                            {
                                oldIndex = i;
                                break;
                            }
                        }

                        this.PeopleGridView.Columns.Move(oldIndex, newIndex++);
                    }
                }
            }
        }

        private void SaveColumnState()
        {
            if (Directory.Exists(SettingsDirectory) == false)
            {
                Directory.CreateDirectory(SettingsDirectory);
            }

            List<String> columnOrder = new List<string>();
            foreach (var col in this.PeopleGridView.Columns)
            {
                columnOrder.Add(col.Header.ToString());
            }

            Serializer.ToJson(columnOrder, SettingsFile);
        }

        private static string SettingsFile
        {
            get { return _XmlDirectory ?? Path.Combine(SettingsDirectory, "Columnsetings.json"); }
            set { _XmlDirectory = value; }
        }

        private static string _XmlDirectory;

        private static string SettingsDirectory
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ListViewColumnStore"); }
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
        }

        public class People : List<Person>
        {
        }
    }
}