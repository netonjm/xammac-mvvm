using System.ComponentModel;

namespace xammacmvvm.DeclarativeUI
{
    internal interface IOptionViewModel : INotifyPropertyChanged
    {
        string Name { get; }
        string DisplayName { get; }
        string ToolTip { get; set; }
        bool IsVisible { get; }
        object Value { get; }
        bool IsDynamic { get; }
        bool IsCreationOption { get; }
        bool DidValueChange { get; set; }

        string GroupName { get; set; }
        bool DisplayErrorMessage { get; set; }

        bool TryImportValue(IOptionViewModel other);
    }

    internal abstract class OptionViewModel : ViewModel, IOptionViewModel
    {
        public OptionViewModel()
            : base()
        {
            IsDynamic = true;
            IsVisible = true;
            IsCreationOption = true;
        }

        public string Name { get; set; }

        string _toolTip;
        public string ToolTip
        {
            get => _toolTip;
            set => Set(ref _toolTip, value);
        }

        string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set => Set(ref _displayName, value);
        }

        bool _isVisible;
        public bool IsVisible {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        public abstract object Value { get; }
        public bool IsDynamic { get; set; }
        public bool IsCreationOption { get; set; }
        public bool DidValueChange { get; set; }


        public string GroupName { get; set; }
        public bool DisplayErrorMessage { get; set; }

        public abstract bool TryImportValue(IOptionViewModel other);
    }
}

