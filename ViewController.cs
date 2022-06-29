using ObjCRuntime;
using xammacmvvm.DeclarativeUI;

namespace xammacmvvm;

public partial class ViewController : NSViewController
{
    CheckBoxViewModel _checkBoxViewModel;
    TextBoxViewModel _textBoxViewModel;

    OptionPanelViewController optionPanelViewController;

    protected ViewController(NativeHandle handle) : base(handle)
    {
        
    }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        //dynamic option panel needs a specific view model to store collection and save data
        var optionsViewModel = new TestingOptionPanelViewModel();
       
        //we create a control to convert ViewModels to native views and stack them all
        optionPanelViewController = new OptionPanelViewController(optionsViewModel) {
            RowDelegate = new OptionPanelDelegate()
        };
        View = optionPanelViewController.View;

        //create our collection of elements
        _checkBoxViewModel = new CheckBoxViewModel(false) { TextValue = "hello", IsChecked = true };
        _textBoxViewModel = new TextBoxViewModel() { TextValue = "Information about click..." };

        _checkBoxViewModel.Checked += _checkBoxViewModel_Checked;

        //time to refresh the model
        optionsViewModel.Refresh(new() { _checkBoxViewModel, _textBoxViewModel });

        // Do any additional setup after loading the view.
        //LoopUpdateLabel();
    }


    int itemWasChecked = 0;
    private void _checkBoxViewModel_Checked(object? sender, EventArgs e)
    {
        itemWasChecked++;
        _textBoxViewModel.TextValue = $"You clicked: {itemWasChecked}";
    }

    async Task LoopUpdateLabel()
    {
        for (int i = 0; i < 10; ++i)
        {
            await Task.Delay(1000);

            _checkBoxViewModel.TextValue = $"{i}";
            _checkBoxViewModel.IsChecked = !_checkBoxViewModel.IsChecked;
        }
    }

    public override void ViewDidAppear()
    {
        base.ViewDidAppear();

        //_label.Bind(new NSString("value"), _vm, nameof(ViewModel.Label), null);
    }

    public override void ViewDidDisappear()
    {
        //_label.Unbind(new NSString("value"));

        base.ViewDidDisappear();
    }

    public override NSObject RepresentedObject
    {
        get => base.RepresentedObject;
        set
        {
            base.RepresentedObject = value;

            // Update the view, if already loaded.
        }
    }
}

#region Option Panel

class OptionPanelDelegate : IOptionPanelDelegate
{
    public object GetViewForModel(IOptionViewModel model)
    {
        if (model is TextBoxViewModel textBoxViewModel)
            return GetTextBoxRowView(textBoxViewModel);
        if (model is CheckBoxViewModel checkBoxViewModel)
            return GetCheckBoxRowView(checkBoxViewModel);
        return null;
    }

    NSView GetTextBoxRowView(TextBoxViewModel model)
    {
        var stackView = new NSStackView()
        {
            Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
            Alignment = NSLayoutAttribute.CenterY,
            Distribution = NSStackViewDistribution.Fill
        };
        var textField = new ObservableTextField(model);
        stackView.AddArrangedSubview(textField);
        textField.WidthAnchor.ConstraintEqualTo(240).Active = true;
        return textField;
    }

    NSView GetCheckBoxRowView(CheckBoxViewModel model)
    {
        var stackView = new NSStackView()
        {
            Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
            Alignment = NSLayoutAttribute.CenterY,
            Distribution = NSStackViewDistribution.Fill
        };
        var checkBox = new ObservableCheckBox(model);
        stackView.AddArrangedSubview(checkBox);
        return stackView;
    }
}

class TestingOptionPanelViewModel : OptionsPanelViewModel
{
    public event EventHandler Saved;

    public List<OptionViewModel> Settings { get; private set; }

    public void Refresh(List<OptionViewModel> settings)
    {
        Settings = settings;
        Refresh();
    }

    protected override void OnRefreshRequested()
    {
        foreach (var item in Settings)
            Options.Add(item);
    }

    protected override void OnSaveOptionsRequested()
    {
        Saved?.Invoke(this, EventArgs.Empty);
    }
}

#endregion
