using System;
using System.Reflection.Emit;
using xammacmvvm.DeclarativeUI;
using AppKit;

namespace xammacmvvm;

class ObservableTextField : NSTextField
{
    readonly TextBoxViewModel _model;

    public ObservableTextField(TextBoxViewModel model)
    {
        _model = model;
        model.PropertyChanged += Model_PropertyChanged;

        PropertyChanged(nameof(_model.DisplayName));
        PropertyChanged(nameof(_model.TextValue));
        PropertyChanged(nameof(_model.IsVisible));
    }

    private void Model_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        => PropertyChanged(e.PropertyName);

    void PropertyChanged(string propertyName)
    {
        if (propertyName == nameof(_model.DisplayName))
        {
            ToolTip = _model.DisplayName;
        }
        else if (propertyName == nameof(_model.TextValue))
        {
            StringValue = _model.TextValue ?? string.Empty;
        }
        else if (propertyName == nameof(_model.IsVisible))
        {
            Hidden = !_model.IsVisible;
        }
    }

    protected override void Dispose(bool disposing)
    {
        _model.PropertyChanged -= Model_PropertyChanged;
        base.Dispose(disposing);
    }
}



