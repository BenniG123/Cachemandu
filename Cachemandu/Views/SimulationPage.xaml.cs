using Cachemandu.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        Models.Cache cache;

        public SimulationPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            StorageFile file = (StorageFile) e.Content;

            LogParser parser = new LogParser(file.Path, false);
            Logger logger = new Logger(500);
            bool istrue = false;

            if (parser.IsOpen())
            {
                for (int i = 0; !parser.CloseIfDone(); i++)
                {
                    MemInst inst = parser.GetNextInst();
                    logger.add(i < 500);
                    if (i == 1000)
                    {
                        i = 0;
                    }
                }

                List<float> hist = logger.getHistory();
                foreach (var item in hist)
                {
                    Debug.WriteLine(item);
                }
            }

            Debug.WriteLine("!");

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               AppViewBackButtonVisibility.Visible;
        }
    }
}