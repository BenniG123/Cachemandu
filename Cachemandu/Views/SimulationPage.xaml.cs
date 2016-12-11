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
        private ulong fileSize;
        private ulong fileSizeRead;

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

            Tuple<Cache, StorageFile, int> tuple = (Tuple<Cache, StorageFile, int>) e.Parameter;
            cache = tuple.Item1;
            StorageFile logFile = tuple.Item2;
            int numLayers = tuple.Item3;

            StreamReader reader = null;
            Stream stream = null;

            try
            {
                fileSize = (await logFile.GetBasicPropertiesAsync()).Size;
                fileSizeRead = 0;
                stream = await logFile.OpenStreamForReadAsync();
                reader = new StreamReader(stream, new ASCIIEncoding());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            LogParser parser = new LogParser(reader, false);
            int numToAvg = 2000;
            List<Logger> logger = new List<Logger>();
            for (int i = 0; i < numLayers; i++)
            {
                logger.Add(new Logger(numToAvg));
            }

            if (reader != null && stream != null && stream.CanRead)
            {
                for (int i = 0; !parser.CloseIfDone(); i++)
                {
                    MemInst inst = parser.GetNextInst();
                    if (inst != null)
                    {
                        fileSizeRead += (ulong)inst.lineSize;
                        float percentage = fileSizeRead * 1.0f / fileSize;
                        if (inst.type == InstType.LOAD)
                        {
                            int where = cache.check(inst.addr, 1);

                            // If there wasn't a hit in any layer, log all the misses
                            if (where == 0)
                            {
                                for (int j = 0; j < numLayers; j++)
                                {
                                    logger[j].add(false);
                                }
                            }
                            // Otherwise, mark hits and misses accordingly
                            else
                            {
                                for (int j = 0; j < where; j++)
                                {
                                    logger[j].add(where - 1 == j);
                                }
                            }
                        }
                    }
                }

                reader.Dispose();
                stream.Dispose();

                List<List<float>> hist = new List<List<float>>();
                for (int i = 0; i < numLayers; i++)
                {
                    hist.Add(logger[i].getHistory());
                }

                int pc = numToAvg;
                averageHitRate = 0f;

                foreach (var item in hist[0])
                {
                    HitRateItem dataPoint = new HitRateItem();
                    dataPoint.Rate = item;
                    dataPoint.PC = pc;
                    plotList.Add(dataPoint);
                    averageHitRate += item;

                    pc += numToAvg;
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