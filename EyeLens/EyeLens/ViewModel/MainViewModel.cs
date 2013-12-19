using EyeLens.Model;
using EyeLens.Resources;
using GalaSoft.MvvmLight;
using Nokia.Graphics.Imaging;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using Windows.Storage.Streams;
using Windows.Phone.Speech.Synthesis;
using EyeLens.Helpers;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Shell;

namespace EyeLens.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private const int MaxZoomLevel = 4;
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            InitFiltersList();
            try
            {
                InterfaceSpeech = (bool)appSettings["InterfaceSpeech"];
            }
            catch {
                InterfaceSpeech = true;
            };
        }

        public void UpdatedFirstTile(string text) {
            try
            {
                ShellTile TileToFind = ShellTile.ActiveTiles.First();
                StandardTileData NewTileData = new StandardTileData
                {
                    BackTitle = AppResources.Filter,
                    //BackBackgroundImage = new Uri(textBoxBackBackgroundImage.Text, UriKind.Relative),
                    BackContent = text
                };
                // Update the Application Tile
                TileToFind.Update(NewTileData);
            }
            catch { };
        }

        /// <summary>
        /// Init filterslist and add filters in it
        /// </summary>
        private void InitFiltersList()
        {
            FiltersList = new ObservableCollection<FilterItem>();
            FiltersList.Add(new FilterItem() { 
                Id = 6, 
                FilterTitle = AppResources.Filter_MagicPen,
                CurrentFilter = new MagicPenFilter()
              });

            FiltersList.Add(new FilterItem()
            {
                Id = 7,
                FilterTitle = AppResources.Filter_Grayscale,
                CurrentFilter = new GrayscaleFilter()
            });
            FiltersList.Add(new FilterItem()
            {
                Id = 3,
                FilterTitle = AppResources.Filter_Antique,
                CurrentFilter = new AntiqueFilter()
            });
            FiltersList.Add(new FilterItem()
            {
                Id = 4,
                FilterTitle = AppResources.Filter_Stamp,
                CurrentFilter = new StampFilter(4, 0.3)
            });

            FiltersList.Add(new FilterItem()
            {
                Id = 5,
                FilterTitle = AppResources.Filter_Cartoon,
                CurrentFilter = new CartoonFilter(false)
            });

            FiltersList.Add(new FilterItem()
            {
                Id = 1,
                FilterTitle = AppResources.Filter_Sharpness,
                CurrentFilter = new SharpnessFilter(7)
            });
            FiltersList.Add(new FilterItem()
            {
                Id = 2,
                FilterTitle = AppResources.Filter_AutoEnhance,
                CurrentFilter = new AutoEnhanceFilter()
            });
            FiltersList.Add(new FilterItem()
            {
                Id = 8,
                FilterTitle = AppResources.Filter_Posterize,
                CurrentFilter = new PosterizeFilter()
            });
            /*FiltersList.Add(new FilterItem()
            {
                Id = 8,
                FilterTitle = AppResources.Filter_Custom,
                CurrentFilter = new CustomEffect(_cameraPreviewImageSource)
            });*/

        }

        public IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;

        private ObservableCollection<FilterItem> _filtersList = new ObservableCollection<FilterItem>();
        /// <summary>
        /// List of current filters in app
        /// </summary>
        public ObservableCollection<FilterItem> FiltersList
        {
            get { return _filtersList; }
            set { 
                _filtersList = value;
                RaisePropertyChanged("FiltersList");
            }
        }

        public SpeechSynthesizer synth = new SpeechSynthesizer();

        private bool _interfaceSpeech = true;
        /// <summary>
        /// Should we use speech in app interface or not
        /// </summary>
        public bool InterfaceSpeech
        {
            get { return _interfaceSpeech; }
            set { 
                _interfaceSpeech = value;
                try
                {
                    appSettings["InterfaceSpeech"] = _interfaceSpeech;
                }
                catch { };
                RaisePropertyChanged("InterfaceSpeech");
            }
        }        

        /// <summary>
        /// Synth speech and clear speech queue
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<bool> SayText(string text)
        {
            try
            {
                try
                {
                    synth.CancelAll();
                }
                catch { };
                if (InterfaceSpeech == true)
                {
                    await synth.SpeakTextAsync(text);
                }
                else
                {
                };
            }
            catch { };
            return true;
        }

        private int _zoomLevel = 1;
        /// <summary>
        /// ViewZoomLevel
        /// </summary>
        public int ZoomLevel
        {
            get { return _zoomLevel; }
            set {
                if ((value <= MaxZoomLevel) && (value> 0))
                {
                    _zoomLevel = value;
                    RaisePropertyChanged("ZoomLevel");
                }
            }
        }

        private bool _getColorUsed = false;
        /// <summary>
        /// If true, on next tap user wants to get the color
        /// </summary>
        public bool GetColorUsed
        {
            get { return _getColorUsed; }
            set { 
                _getColorUsed = value;
                RaisePropertyChanged("GetColorUsed");
            }
        }
        

    }
}