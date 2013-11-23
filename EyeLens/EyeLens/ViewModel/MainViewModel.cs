using EyeLens.Model;
using EyeLens.Resources;
using GalaSoft.MvvmLight;
using Nokia.Graphics.Imaging;
using System.Collections.ObjectModel;

using Nokia.Graphics.Imaging;
using EyeLens.Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using Windows.Storage.Streams;
using Windows.Phone.Speech.Synthesis;
using EyeLens.Helpers;

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
            var str = ColorNameDictionary.GetColorName(1, 1, 1);
        }

        /// <summary>
        /// Init filterslist and add filters in it
        /// </summary>
        private void InitFiltersList()
        {
            FiltersList = new ObservableCollection<FilterItem>();
            FiltersList.Add(new FilterItem() { 
                Id = 1, 
                FilterTitle = AppResources.Filter_MagicPen,
                CurrentFilter = new MagicPenFilter()
              });

            FiltersList.Add(new FilterItem()
            {
                Id = 2,
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
                Id = 6,
                FilterTitle = AppResources.Filter_Sharpness,
                CurrentFilter = new SharpnessFilter(7)
            });
            FiltersList.Add(new FilterItem()
            {
                Id = 7,
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

        private ObservableCollection<FilterItem> _filtersList = new ObservableCollection<FilterItem>();
        /// <summary>
        /// 
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

        public async Task<bool> SayText(string text)
        {
            try
            {
                try
                {
                    synth.CancelAll();
                }
                catch { };
                await synth.SpeakTextAsync(text);
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
    }
}