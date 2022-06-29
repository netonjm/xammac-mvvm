using System.Collections.ObjectModel;
using System.ComponentModel;

namespace xammacmvvm.DeclarativeUI
{
    internal interface IOptionsPanelViewModel : INotifyPropertyChanged,
INotifyPropertyChanging
    {
        ObservableCollection<IOptionViewModel> Options { get; }
        int ParametersChanged { get; }
        int TotalParameters { get; }
        bool HasVisibleOptions { get; }

        void SaveOptions();
    }

    internal abstract class OptionsPanelViewModel : ViewModel, IOptionsPanelViewModel
    {
        private readonly IList<IOptionViewModel> _options;

        private bool _hasVisibleOptions;
        public bool HasVisibleOptions
        {
            get { return _hasVisibleOptions; }
            set
            {
                if (_hasVisibleOptions == value)
                    return;
                _hasVisibleOptions = value;
                OnPropertyChanged();
            }
        }

        public int ParametersChanged
        {
            get
            {
                int parametersChanged = 0;
                foreach (IOptionViewModel opt in _options)
                {
                    if (opt.DidValueChange)
                    {
                        ++parametersChanged;
                    }
                }
                return parametersChanged;
            }
        }

        public int TotalParameters => _options.Count;

        public ObservableCollection<IOptionViewModel> Options
        {
            get { return _options as ObservableCollection<IOptionViewModel>; }
        }

        public OptionsPanelViewModel()
        {
            _options = new ObservableCollection<IOptionViewModel>();
        }

        public bool IsOptionSet(string name)
        {
            bool isSet = false;
            var option = _options.Where(o => string.Equals(o.Name, name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (option?.Value is bool boolValue)
            {
                isSet = boolValue;
            }

            return isSet;
        }

        protected void Refresh()
        {
            Options.Clear();

            OnRefreshRequested();

            OnPropertyChanged(nameof(Options));

            HasVisibleOptions = Options.Where(option => option.IsVisible).Any();
            SaveOptions();
            ClearModifiedFlagOnAllOptions();
        }

        public void SaveOptions()
        {
            OnSaveOptionsRequested();
        }

        protected abstract void OnSaveOptionsRequested();

        protected abstract void OnRefreshRequested();

        private void ClearModifiedFlagOnAllOptions()
        {
            foreach (IOptionViewModel option in _options)
            {
                option.DidValueChange = false;
            }
        }
    }
}

