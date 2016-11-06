using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Cachemandu.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cachemandu.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile logFile;

        public MainPage()
        {
            logFile = null;
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
               AppViewBackButtonVisibility.Collapsed;
        }

        private async void PickLogFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".txt");

            logFile = await openPicker.PickSingleFileAsync();
            if (logFile != null)
            {
                // Application now has read/write access to the picked file
                txtLogFileName.Text = logFile.Name;
            }
        }

        private void RunSimulation(object sender, RoutedEventArgs e)
        {
            /* int wordSize = int.Parse(((TextBlock) ((ComboBoxItem) lstWordSize.SelectedItem).Content).Text);
            int blockSize = (int)lstBlockSize.SelectedValue;
            int numBlocks = (int)lstNumBlocks.SelectedValue;
            int mappingSize = (int)lstMapSize.SelectedValue;
            */

            // TODO - Parse other options

            Cache c = new Cache(32, 256, 32, 1, new RandomReplacementPolicy(), false);
            Tuple<Cache, StorageFile> t = Tuple.Create<Cache, StorageFile>(c, logFile);
            Frame.Navigate(typeof(Views.SimulationPage), t);
        }
    }
}
