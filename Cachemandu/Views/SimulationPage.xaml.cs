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
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

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
        private LineSeries ls;
        private ObservableCollection<HitRateItem> plotList;

        public SimulationPage()
        {
            this.InitializeComponent();
            plotList = new ObservableCollection<HitRateItem>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               AppViewBackButtonVisibility.Visible;

            progressSimulation.IsActive = true;
            chartHitRate.Visibility = Visibility.Collapsed;

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

                reader.Dispose();
                stream.Dispose();

                List<float> hist = logger.getHistory();
                await ThreadPool.RunAsync((s) =>
                {
                    plotList.Clear();
                });

                int pc = 500;

                foreach (var item in hist)
                {
                    HitRateItem dataPoint = new HitRateItem();
                    dataPoint.Rate = item;
                    dataPoint.PC = pc;
                    plotList.Add(dataPoint);

                    pc += 500;
                }

                progressSimulation.IsActive = false;
                chartHitRate.Visibility = Visibility.Visible;
                ls = ((LineSeries) chartHitRate.Series[0]);
                ls.ItemsSource = plotList;
            }
        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e)
        {
            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (Frame.CanGoBack && e.Handled == false)
            {
                // plotList.Clear();
                // ls.ItemsSource = plotList;
                SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
                e.Handled = true;
                Frame.GoBack();
            }
        }
    }
}