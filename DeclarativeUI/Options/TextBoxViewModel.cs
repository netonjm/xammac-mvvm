namespace xammacmvvm.DeclarativeUI
{
    internal interface ITextBoxOption : IOptionViewModel
    {
        string TextValue { get; set; }
    }

    internal class TextBoxViewModel : OptionViewModel, ITextBoxOption
    {
        private string _value;
        public TextBoxViewModel()
        {
        }

        public string TextValue
        {
            get => _value;
            set
            {
                if (Set(ref _value, value))
                {
                    DidValueChange = true;
                }
            }
        }

        public override object Value => _value;

        public static readonly string DefaultDataTemplate = "optionsTextBox";

        public override bool TryImportValue(IOptionViewModel other)
        {
            if (other is TextBoxViewModel t)
            {
                TextValue = t.TextValue;
                DidValueChange = t.DidValueChange;
                return true;
            }

            return false;
        }
    }
}

