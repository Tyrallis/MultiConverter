using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using MultiConverterLib;
using MultiConverter.Lib;

namespace MultiConverterGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int progress = 0;
        public MainWindow()
        {
            if (!Listfile.IsInitialized)
                MultiConverter.Lib.Listfile.Initialize();
            InitializeComponent();
            
        }

        private void lb_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                progress = 0;
                string[] list = (string[])e.Data.GetData(DataFormats.FileDrop);
                LoadFiles(list);
            }
        }

        private void LoadFiles(string[] list)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = false;

            bw.DoWork += new DoWorkEventHandler((sender, e) =>
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
                string[] files = e.Argument as string[];
                HashSet<string> lf = new HashSet<string>();

                foreach (var s in files)
                {
                    if (Directory.Exists(s))
                    {
                        foreach (string file in Directory.EnumerateFiles(s, "*.*", SearchOption.AllDirectories))
                            if (Utils.IsCorrectFile(file) && !lf.Contains(file))
                                lf.Add(file.ToLower());
                    }
                    else if (Utils.IsCorrectFile(s) && !lf.Contains(s))
                        lf.Add(s.ToLower());
                }

                e.Result = lf;
            });

            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler((sender, e) =>
            {
                HashSet<string> files = (HashSet<string>)e.Result;
                foreach (string file in files)
                    lb.Items.Add(file);
            });

            bw.RunWorkerAsync(list);
        }

        private void lb_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void btnFix_Click(object sender, RoutedEventArgs e)
        {
            List<object> items = new List<object>();
            object[] lbitems = new object[lb.Items.Count];
            lb.Items.CopyTo(lbitems, 0);
            items.AddRange(lbitems);
            List<string> files = new List<string>();
            foreach (object o in items)
                files.Add((string)o);
            FixList(files);
        }

        struct ConvertionErrorInfo
        {
            public Exception exception;
            public string filename;

            public ConvertionErrorInfo(Exception e, string file)
            {
                exception = e;
                filename = file;
            }
        }

        private void FixList(List<string> list)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true;

            bw.DoWork += new DoWorkEventHandler((sender, e) =>
            {
                List<string> items = e.Argument as List<string>;
                List<ConvertionErrorInfo> errors = new List<ConvertionErrorInfo>();
                int progress = 0;

                bool wod = (items[items.Count - 1] == "wod");
                if (wod)
                    items.RemoveAt(items.Count - 1);

                BackgroundWorker worker = sender as BackgroundWorker;

                foreach (string s in items)
                {
                    IConverter converter = null;

                    if (s.EndsWith("m2"))
                    {
                        converter = new M2Converter(s, true);
                    }
                    else if (s.EndsWith("anim"))
                    {
                        converter = new AnimConverter(s);
                    }

                    // ? -> in case a file with a wrong extension/pattern was in the list
                    try
                    {
                        if (converter?.Fix() ?? false)
                        {
                            converter.Save();
                        }
                    }
                    catch (Exception exception)
                    {
                        errors.Add(new ConvertionErrorInfo(exception, s));
                    }

                }

                e.Result = errors;
            });

            bw.RunWorkerAsync(list);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lb.Items.Clear();
        }
    }
}
