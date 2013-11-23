using GalaSoft.MvvmLight;
using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeLens.Model
{
    public class FilterItem: ViewModelBase
    {
        public FilterItem()
        {
        }

        private int _id;
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _filterTitle;
        /// <summary>
        /// Filter name
        /// </summary>
        public string FilterTitle
        {
            get { return _filterTitle; }
            set { 
                _filterTitle = value;
                RaisePropertyChanged("FilterTitle");
            }
        }

        private string _filterIdName;
        /// <summary>
        /// 
        /// </summary>
        public string FilterIdName
        {
            get { return _filterIdName; }
            set { 
                _filterIdName = value;
                RaisePropertyChanged("FilterIdName");
            }
        }
        

        private IFilter _currentFilter;
        /// <summary>
        /// Current filter, which should be appliedto image
        /// </summary>
        public IFilter CurrentFilter
        {
            get { return _currentFilter; }
            set { 
                _currentFilter = value;
                RaisePropertyChanged("CurrentFilter");
            }
        }
        

    }
}
