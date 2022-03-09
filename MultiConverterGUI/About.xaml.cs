using Markdig;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

namespace MultiConverterGUI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        string offlineBackup = @"
# MultiConverter
### Shadowlands to Wotlk

## Features
- Supported formats M2, ADT, WMO
- Can recombine Skel files back into M2

## [Requirements]
## [Usage]

## Support
Use the [Issues] section on Github to report any issues,
this can also be used to ask for features.

Please confirm issues before submitting and contribute to an
already existing issue if possible.

Screenshots in 010 Editor with the correct template or an upload of 
the file will help greatly in solving any issues.


Created by [Adspartan]
Updated by [callumhutchy]

##### [Source Code] 
##### [Discord] 
##### [Model Changing] 

   [Issues]: <https://github.com/callumhutchy/MultiConverter/issues>
   [Adspartan]: <https://github.com/adspartan>
   [callumhutchy]: <https://github.com/callumhutchy>
   [Source Code]: <https://github.com/callumhutchy/MultiConverter>
   [Discord]: <https://discord.gg/pMFZnP47>
   [Model Changing]: <https://model-changing.net>
   [Requirements]: <https://github.com/callumhutchy/MultiConverter/wiki/Requirements>
   [Usage]: <https://github.com/callumhutchy/MultiConverter/wiki/Usage>
";
        private static bool willNavigate;
        public About()
        {

            InitializeComponent();
            try
            {
                using (WebClient client = new WebClient())
                {
                    wb.NavigateToString(Markdown.ToHtml(client.DownloadString("https://raw.githubusercontent.com/callumhutchy/MultiConverter/main/README.md").Trim()));
                }
            }
            catch
            {
                wb.NavigateToString(Markdown.ToHtml(offlineBackup));
            }

        }

        private void wb_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            // first page needs to be loaded in webBrowser control
            if (!willNavigate)
            {
                willNavigate = true;
                return;
            }

            // cancel navigation to the clicked link in the webBrowser control
            e.Cancel = true;

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = true,
                Verb = "open",
                FileName = e.Uri.ToString()
            };

            Process.Start(startInfo);
        }
    }
}
