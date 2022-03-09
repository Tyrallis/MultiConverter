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
using System.Threading;

namespace MultiConverterGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int progress = 0;
        public static int PROGRESS = 5;
        private int threadRemaining = 0;
        private int converted = 0;
        private int toConverted = 0;
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
            // multi threading only if there's enough models to fix
            int pc = Environment.ProcessorCount * 2 - 1;
            pc = lb.Items.Count < pc ? 1 : pc;
            int count = lb.Items.Count, div = count / pc, r = count % pc;
            threadRemaining = pc;

            if (File.Exists("error.log"))
            {
                System.IO.File.WriteAllText("error.log", string.Empty);
            }

            List<object> items = new List<object>();
            object[] lbitems = new object[lb.Items.Count];
            lb.Items.CopyTo(lbitems, 0);
            items.AddRange(lbitems);

            converted = 0;
            toConverted = items.Count;

            List<string> files = new List<string>();
            foreach (object o in items)
                files.Add((string)o);

            for (int i = 0; i < pc; i++)
            {
                List<string> list = new List<string>();
                int n = div + ((r-- > 0) ? 1 : 0);

                foreach (object o in items.Take(n))
                    list.Add(o.ToString());
                items.RemoveRange(0, ((n > items.Count) ? items.Count : n));
                FixList(list);
            }
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

            pb.Maximum = list.Count;
            pb.Value = 0;
            bool fixHelmet = cbFixHelm.IsChecked != null ? ((bool)cbFixHelm.IsChecked) : false;
            bool fixLiquids = cbLiquids.IsChecked != null ? ((bool)cbLiquids.IsChecked) : false;
            bool fixModels = cbModels.IsChecked != null ? ((bool)cbModels.IsChecked) : false;

            int doneItems = 0;
            bool done = false;
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
                        converter = new M2Converter(s, fixHelmet);
                    }
                    else if (s.EndsWith("adt"))
                    {
                        converter = new AdtConverter(s, fixLiquids, fixModels);
                    }
                    else if (s.EndsWith("wdt"))
                    {
                        converter = new WDTConverter(s);
                    }
                    else if (Regex.IsMatch(s, @".*_[0-9]{3}(_(lod[0-9]))?\.(wmo)"))
                    {
                        converter = new WMOGroupConverter(s, wod);
                    }
                    else if (s.EndsWith("wmo"))
                    {
                        if (wod)
                        {
                            continue; // nothing to do
                        }
                        converter = new WMORootConverter(s);
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
                    doneItems++;
                }
                done = true;
                e.Result = errors;
            });

            bw.RunWorkerAsync(list);
            while (!done)
            {
                pb.Value = doneItems;
                Thread.Sleep(10);
            }
            pb.Value = doneItems;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            lb.Items.Clear();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

    }
}
