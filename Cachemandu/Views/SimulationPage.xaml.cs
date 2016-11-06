using Cachemandu.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cachemandu.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimulationPage : Page
    {
        private Cache cache;

        public SimulationPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               AppViewBackButtonVisibility.Visible;

            Tuple<Cache, StorageFile> tuple = (Tuple<Cache, StorageFile>) e.Parameter;
            cache = tuple.Item1;
            StorageFile logFile = tuple.Item2;

            StreamReader reader = null;
            Stream stream = null;

            try
            {
                stream = await logFile.OpenStreamForReadAsync();
                reader = new StreamReader(stream, new ASCIIEncoding());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            LogParser parser = new LogParser(reader, false);
            Logger logger = new Logger(500);

            if (reader != null && stream != null && stream.CanRead)
            {
                for (int i = 0; !parser.CloseIfDone(); i++)
                {
                    MemInst inst = parser.GetNextInst();
                    if (inst != null && inst.type == InstType.LOAD)
                    {
                        logger.add(cache.check(inst.addr));
                    }
                }

                List<float> hist = logger.getHistory();
                foreach (var item in hist)
                {
                    Debug.WriteLine(item);
                }
            }
        }
    }
}