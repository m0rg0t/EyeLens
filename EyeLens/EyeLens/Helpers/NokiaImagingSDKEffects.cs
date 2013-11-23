/*
 * Copyright © 2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using Nokia.Graphics.Imaging;
using EyeLens.Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Phone.Media.Capture;
using Windows.Storage.Streams;
using EyeLens.ViewModel;
using System.Linq;
using EyeLens.Model;

namespace EyeLens
{
    public class NokiaImagingSDKEffects : ICameraEffect
    {
        private PhotoCaptureDevice _photoCaptureDevice = null;
        private CameraPreviewImageSource _cameraPreviewImageSource = null;
        private FilterEffect _filterEffect = null;
        private CustomEffectBase _customEffect = null;
        private int _effectIndex = 0;
        private int _effectCount = 9;
        private Semaphore _semaphore = new Semaphore(1, 1);

        public String EffectName { get; private set; }

        private string _shortFilterName;
        /// <summary>
        /// short filter name
        /// </summary>
        public string ShortFilterName
        {
            get { return _shortFilterName; }
            set { 
                _shortFilterName = value;

            }
        }
        

        public PhotoCaptureDevice PhotoCaptureDevice
        {
            set
            {
                if (_photoCaptureDevice != value)
                {
                    while (!_semaphore.WaitOne(100));

                    _photoCaptureDevice = value;

                    Initialize();

                    _semaphore.Release();
                }
            }
        }

        ~NokiaImagingSDKEffects()
        {
            while (!_semaphore.WaitOne(100));

            Uninitialize();

            _semaphore.Release();
        }

        public async Task GetNewFrameAndApplyEffect(IBuffer frameBuffer, Size frameSize)
        {
            if (_semaphore.WaitOne(500))
            {
                var scanlineByteSize = (uint)frameSize.Width * 4; // 4 bytes per pixel in BGRA888 mode
                var bitmap = new Bitmap(frameSize, ColorMode.Bgra8888, scanlineByteSize, frameBuffer);

                if (_filterEffect != null)
                {
                    var renderer = new BitmapRenderer(_filterEffect, bitmap);
                    await renderer.RenderAsync();
                }
                else if (_customEffect != null)
                {
                    var renderer = new BitmapRenderer(_customEffect, bitmap);
                    await renderer.RenderAsync();
                }
                else
                {
                    var renderer = new BitmapRenderer(_cameraPreviewImageSource, bitmap);
                    await renderer.RenderAsync();
                }

                _semaphore.Release();
            }
        }

        public void NextEffect()
        {
            if (_semaphore.WaitOne(500))
            {
                Uninitialize();

                _effectIndex++;

                if (_effectIndex >= ViewModelLocator.MainStatic.FiltersList.Count)
                {
                    _effectIndex = 0;
                }

                Initialize();

                _semaphore.Release();
            }
        }

        public void PreviousEffect()
        {
            if (_semaphore.WaitOne(500))
            {
                Uninitialize();
                
                _effectIndex--;

                if (_effectIndex < 0)
                {
                    _effectIndex = ViewModelLocator.MainStatic.FiltersList.Count;
                }

                Initialize();

                _semaphore.Release();
            }
        }

        private void Uninitialize()
        {
            if (_cameraPreviewImageSource != null)
            {
                _cameraPreviewImageSource.Dispose();
                _cameraPreviewImageSource = null;
            }

            if (_filterEffect != null)
            {
                _filterEffect.Dispose();
                _filterEffect = null;
            }

            if (_customEffect != null)
            {
                _customEffect.Dispose();
                _customEffect = null;
            }
        }

        private void Initialize()
        {
            var filters = new List<IFilter>();
            var nameFormat = "{0}/" + ViewModelLocator.MainStatic.FiltersList.Count + " - {1}";

            _cameraPreviewImageSource = new CameraPreviewImageSource(_photoCaptureDevice);

            FilterItem filterItem = ViewModelLocator.MainStatic.FiltersList.FirstOrDefault();
            if (ViewModelLocator.MainStatic.FiltersList.FirstOrDefault(c => c.Id == (_effectIndex + 1)) == null)
            {
                filterItem = ViewModelLocator.MainStatic.FiltersList.FirstOrDefault();
            }
            else
            {
                filterItem = ViewModelLocator.MainStatic.FiltersList.FirstOrDefault(c => c.Id == (_effectIndex + 1));
            };
            ShortFilterName = filterItem.FilterTitle;
            EffectName = String.Format(nameFormat, filterItem.Id, filterItem.FilterTitle);
            filters.Add(filterItem.CurrentFilter);
            /*switch (_effectIndex)
            {
                case 0:
                    {
                        ShortFilterName = AppResources.Filter_MagicPen;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_MagicPen);
                        filters.Add(new MagicPenFilter());
                    }
                    break;

                case 1:
                    {
                        ShortFilterName = AppResources.Filter_Grayscale;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Grayscale);
                        filters.Add(new GrayscaleFilter());
                    }
                    break;

                case 2:
                    {
                        ShortFilterName = AppResources.Filter_Antique;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Antique);
                        filters.Add(new AntiqueFilter());
                    }
                    break;

                case 3:
                    {
                        ShortFilterName = AppResources.Filter_Stamp;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Stamp);
                        filters.Add(new StampFilter(4, 0.3));
                    }
                    break;

                case 4:
                    {
                        ShortFilterName = AppResources.Filter_Grayscale;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Cartoon);
                        filters.Add(new CartoonFilter(false));
                    }
                    break;
                case 5:
                    {
                        ShortFilterName = AppResources.Filter_Sharpness;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Sharpness);
                        filters.Add(new SharpnessFilter(7));
                    }
                    break;

                case 6:
                    {
                        ShortFilterName = AppResources.Filter_AutoEnhance;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_AutoEnhance);
                        filters.Add(new AutoEnhanceFilter());
                    }
                    break;
                case 7:
                    {
                        ShortFilterName = AppResources.Filter_None;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_None);
                    }
                    break;
                case 8:
                    {
                        ShortFilterName = AppResources.Filter_Custom;
                        EffectName = String.Format(nameFormat, _effectIndex + 1, AppResources.Filter_Custom);
                        _customEffect = new CustomEffect(_cameraPreviewImageSource);
                    }
                    break;
            }*/

            if (filters.Count > 0)
            {
                _filterEffect = new FilterEffect(_cameraPreviewImageSource)
                {
                    Filters = filters
                };
            }
        }
    }
}