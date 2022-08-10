using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ToDoListWpf.Data.Classes;
using System.IO;
using Newtonsoft.Json;


namespace ToDoListWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<UserTask> Tasks { get; set; }

        public static string separator = "@||@";
        public static string currentPath = "";
        public static string currentFileName = "";

        public MainWindow()
        {
            InitializeComponent();

            Tasks = new List<UserTask>();
            
            //BackEnd


            //test

            /*Tasks.Add(new UserTask()
            {
                Id = 1,
                Title = "First",
                Description = "SomeText",

                NeedTime = DateTime.Now,
                IsActual = true,
                Created = DateTime.Now,

                Updated = DateTime.Now,

            });*/


            //timer
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
            //IsVisibleChanged

            DataContext = this; 
            
            Display.ItemsSource = Tasks.ToList();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Title = $"ToDo App {DateTime.Now.ToString("HH:mm:ss")}";    
        }

        private void BtnAccept_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Description Of Message", "Title Of Message", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            if (result == MessageBoxResult.OK)
            {
                //Save new data

                List<UserTask> list = Display.Items.OfType<UserTask>().ToList();
                Tasks.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];

                    if (item.Title != "" && item.Description != null)
                    {
                        continue;
                    }

                    if (item.Id != i+1) item.Id = i + 1;

                    if (item.Created.ToString() == "01.01.0001 0:00:00")
                        item.Created = DateTime.Now;

                    if (item.NeedTime.ToString() == "01.01.0001 0:00:00")
                    {
                        item.NeedTime = DateTime.Now;
                        item.NeedTime = item.NeedTime.AddDays(1);
                    }

                    item.Updated = DateTime.Now; //треба добавить умову зміни item.Updated

                    if (DateTime.Now< item.NeedTime)
                        item.IsActual = true;

                    Tasks.Add(item);
                }
            }
            else
            {
                //Cancel changing

                Tasks.Clear();
                Tasks = LoadFromFile(currentPath + currentFileName);
            }
            Display.ItemsSource = Tasks.ToList();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void CloseWindow_click(object sender, RoutedEventArgs e) => this.Close();

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            PropertyDescriptor propertyDescriptor = (PropertyDescriptor)e.PropertyDescriptor;
            e.Column.Header = propertyDescriptor.DisplayName;
            e.Column.Width = DataGridLength.Auto;
            if (Display.Columns.Count%2==0)
            {
                Style newStyle = new Style(typeof(DataGridCell), e.Column.CellStyle);
                newStyle.Setters.Add(new Setter(DataGridCell.BackgroundProperty, new SolidColorBrush(Colors.LightBlue)));
                e.Column.CellStyle = newStyle;
            }

            if (propertyDescriptor.DisplayName == "Id")
            {
                e.Column.IsReadOnly = true;
            }
            else if (propertyDescriptor.DisplayName == "Title"){}
            else if (propertyDescriptor.DisplayName == "Description"){}
            else if (propertyDescriptor.DisplayName == "IsActual")
            {
                e.Column.Header = "Is Actual";
                e.Column.IsReadOnly = true;
            }
            else if (propertyDescriptor.DisplayName == "NeedTime")
            {
                e.Column.Header = "Need Time";
            }
            else if (propertyDescriptor.DisplayName == "Created"){}
            else if (propertyDescriptor.DisplayName == "Updated")
            {
                e.Column.Header = "Last Update";
            }

            if(IsTypeOrNullableOfType(e.PropertyType, typeof(DateTime)))
            {
                DataGridTemplateColumn col = new DataGridTemplateColumn();
                col.Header = e.Column.Header;
                FrameworkElementFactory datePickerFactoryElem = new FrameworkElementFactory(typeof(Xceed.Wpf.Toolkit.DateTimePicker));
                Binding dateBind = new Binding(e.PropertyName);
                dateBind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                dateBind.Mode = BindingMode.TwoWay;

                datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.ValueProperty, dateBind);
                datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.FormatProperty, Xceed.Wpf.Toolkit.DateTimeFormat.Custom);
                datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.FormatStringProperty, "MM/dd/yyyy HH:mm");


                if (propertyDescriptor.DisplayName == "Created" || propertyDescriptor.DisplayName == "Updated")
                {
                    datePickerFactoryElem.SetValue(Xceed.Wpf.Toolkit.DateTimePicker.IsReadOnlyProperty, true);
                }

                DataTemplate cellTemplate = new DataTemplate();
                cellTemplate.VisualTree = datePickerFactoryElem;
                col.CellTemplate = cellTemplate;
                

                e.Column = col;
            }
       
        }
        private static bool IsTypeOrNullableOfType(Type propertyType, Type desiredType)
        {
            return (propertyType == desiredType || Nullable.GetUnderlyingType(propertyType) == desiredType);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            //Wrote json
            string full = "";
            foreach (var task in Tasks)
                full += JsonConvert.SerializeObject(task) + separator;

            if (full.Length == 0) return;


            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                InitialDirectory = currentPath,
                Title = "Select a Json File",
                Filter = "Json|*.json",
                FileName = currentFileName,
            };

            if (dialog.ShowDialog() == true)
            {
                currentPath = dialog.FileName;

                var tmp = currentPath.Split("\\", StringSplitOptions.RemoveEmptyEntries);
                currentFileName = tmp[tmp.Length - 1];

                if (File.Exists(currentPath))
                    File.Delete(currentPath);

                currentPath = currentPath.Replace(currentFileName, "");

                File.WriteAllText(currentPath + currentFileName, full);
            }
            
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            //Read json
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog()
                {
                    InitialDirectory = currentPath,
                    Title = "Select a Json File",
                    Filter = "Json|*.json",
                    FileName = currentFileName,
                };

                if (dialog.ShowDialog() == true)
                {
                    string FullPath = dialog.FileName;

                    var tmp = FullPath.Split("\\", StringSplitOptions.RemoveEmptyEntries);

                    currentFileName = tmp[tmp.Length - 1];
                    currentPath = FullPath.Replace(currentFileName, "");

                    Tasks.Clear();
                    Tasks = LoadFromFile(FullPath);
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            Display.ItemsSource = Tasks.ToList();
        }

        private List<UserTask> LoadFromFile(string FullPath)
        {
            List<UserTask> tmp = new List<UserTask>();
            if (!File.Exists(FullPath))
                return tmp;

            var text = "";
            using (StreamReader streamReader = File.OpenText(currentPath + currentFileName))
            {
                text = streamReader.ReadToEnd();
            }

            var tmp2 = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in tmp2)
            {
                UserTask obj;
                try
                {
                    obj = JsonConvert.DeserializeObject<UserTask>(item);
                    tmp.Add(obj);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return tmp;
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            this.Show();
            ShowInTaskbar = true;
        }

        private void minBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ShowInTaskbar = false;
        }
    }
}
