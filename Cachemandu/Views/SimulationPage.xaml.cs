using Cachemandu.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Syncfusion.SfChart.UWP;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Cachemandu.Views
{

    public class HitRateItem
    {
        public float Rate { get; set; }
        public int PC { get; set; }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimulationPage : Page
    {
        private Cache cache;
        public ObservableCollection<HitRateItem> plotList { get; set; }
        private float averageHitRate;

        public SimulationPage()
        {
            this.InitializeComponent();
            plotList = new ObservableCollection<HitRateItem>();
            DataContext = this;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               AppViewBackButtonVisibility.Visible;

            progressSimulation.IsActive = true;
            stackReport.Visibility = Visibility.Collapsed;

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
            int numEntries = 2000;
            Logger logger = new Logger(numEntries);

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

                reader.Dispose();
                stream.Dispose();

                List<float> hist = logger.getHistory();

                int pc = numEntries;
                averageHitRate = 0f;

                foreach (var item in hist)
                {
                    HitRateItem dataPoint = new HitRateItem();
                    dataPoint.Rate = item;
                    dataPoint.PC = pc;
                    plotList.Add(dataPoint);
                    averageHitRate += item;

                    pc += numEntries;
                }

                averageHitRate /= hist.Count;
                txtAverageHitRate.Text = averageHitRate.ToString();
                txtLogFileName.Text = logFile.Name;

                progressSimulation.IsActive = false;
                chartHitRate.PrimaryAxis.Header = "Program Counter";
                chartHitRate.SecondaryAxis.Header = "Hit Rate";
                stackReport.Visibility = Visibility.Visible;
            }
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (Frame.CanGoBack && e.Handled == false)
            {
                plotList.Clear();
                SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
                e.Handled = true;
                Frame.GoBack();
                GC.Collect();
            }
        }
    }
}