using System.Collections.Generic;
using System.ComponentModel;

namespace xammacmvvm.DeclarativeUI
{
    internal interface ICheckBoxOption
        : IOptionViewModel
    {
        bool IsChecked { get; set; }
    }

    internal class CheckBoxViewModel : OptionViewModel, ICheckBoxOption
    {
        private bool _isChecked;
        private bool _invertBoolean;

        public event EventHandler Checked;

        public CheckBoxViewModel(bool invertBoolean)
        {
            _invertBoolean = invertBoolean;
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (Set(ref _isChecked, value))
                {
                    DidValueChange = true;
                    Checked?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private string _textValue;
        public string TextValue
        {
            get => _textValue;
            set
            {
                if (Set(ref _textValue, value))
                {
                    DidValueChange = true;
                }
            }
        }

        public override object Value => _invertBoolean ? !_isChecked : _isChecked;

        public static readonly string DefaultDataTemplate = "optionsCheckBox";

        public override bool TryImportValue(IOptionViewModel other)
        {
            if (other is CheckBoxViewModel c)
            {
                IsChecked = c.IsChecked;
                _invertBoolean = c._invertBoolean;
                DidValueChange = c.DidValueChange;
                return true;
            }

            return false;
        }
    }
}

