using xammacmvvm.DeclarativeUI;
using ObjCRuntime;

namespace xammacmvvm;

internal class ObservableCheckBox : NSButton
{
    readonly CheckBoxViewModel _model;
    const string SelectorName = "onButtonTextCardItemSelector:";

    public ObservableCheckBox(CheckBoxViewModel model)
    {
        _model = model;
        SetButtonType(NSButtonType.Switch);
        model.PropertyChanged += Model_PropertyChanged;

        Action = new Selector(SelectorName);
        Target = this;

        PropertyChanged(nameof(_model.IsChecked));
        PropertyChanged(nameof(_model.TextValue));
        PropertyChanged(nameof(_model.IsVisible));
    }

    [Export(SelectorName)]
    void OnLocationButtonClicked(NSObject target)
    {
        _model.IsChecked = State == NSCellStateValue.On;
    }

    private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        PropertyChanged(e.PropertyName);
    }

    void PropertyChanged(string propertyName)
    {
        if (propertyName == nameof(_model.IsChecked))
        {
            State = _model.IsChecked ? NSCellStateValue.On : NSCellStateValue.Off;
        }
        else if (propertyName == nameof(_model.TextValue))
        {
            Title = _model.TextValue ?? string.Empty;
        }
        else if (propertyName == nameof(_model.IsVisible))
        {
            Hidden = !_model.IsVisible;
        }
    }

    protected override void Dispose(bool disposing)
    {
        _model.PropertyChanged -= Model_PropertyChanged;
        Target = null;
        base.Dispose(disposing);
    }
}



