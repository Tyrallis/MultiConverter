using Markdig;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MultiConverterGUI
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        string offlineBackup = @"# MultiConverter
### Shadowlands to Wotlk

## Features
- Supported formats M2, ADT, WMO
- Can recombine Skel files back into M2

## Support
Please use the [Issues] section on Github to report any issues,
this can also be used to ask for features.


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
   [Model Changing]: <https://model-changing.net>";

        public About()
        {

            InitializeComponent();
            try
            {
                using (WebClient client = new WebClient())
                {
                    wb.NavigateToString(Markdown.ToHtml(client.DownloadString("https://raw.githubusercontent.com/callumhutchy/MultiConverter/main/README.md")));
                }
            }
            catch
            {
                wb.NavigateToString(Markdown.ToHtml(offlineBackup));
            }
        }
    }
}
