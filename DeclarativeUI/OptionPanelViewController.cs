using System;

namespace xammacmvvm.DeclarativeUI
{
    interface IOptionPanelDelegate
    {
        object GetViewForModel(IOptionViewModel model);
    }

    class OptionPanelViewController : NSViewController
    {
        internal const float FirsColumnWidth = 220;

        public IOptionPanelDelegate RowDelegate {
            get => rowDelegate;
            set
            {
                if (rowDelegate == value)
                    return;
                rowDelegate = value;
                RegenerateViews();
            }
        }

        public IEnumerable<IOptionViewModel> ItemsSource
        {
            get => itemsSource;
            set
            {
                if (itemsSource == value)
                    return;

                itemsSource = value;
                RegenerateViews();
            }
        }

        IOptionPanelDelegate rowDelegate;
        IOptionsPanelViewModel _viewModel;
        NSStackView view;
        IEnumerable<IOptionViewModel> itemsSource;

        NSView flexibleSpace;

        public OptionPanelViewController(IOptionsPanelViewModel viewModel)
        {
            view = new NSStackView() {
                Orientation = NSUserInterfaceLayoutOrientation.Vertical,
                Alignment = NSLayoutAttribute.Leading,
                Distribution = NSStackViewDistribution.Fill,
                //TranslatesAutoresizingMaskIntoConstraints = false
            };
            View = view;

            flexibleSpace = new NSView() { TranslatesAutoresizingMaskIntoConstraints = false };
            view.AddArrangedSubview(flexibleSpace);

            _viewModel = viewModel;
            //HACK: Our IOptionsPanelViewModel.Options could be set before view is shown so we ensure we don't lose events 
            _viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            //ensure we don't suscribe twice the PropertyChanged
            _viewModel.PropertyChanged -= viewModel_PropertyChanged;
            _viewModel.PropertyChanged += viewModel_PropertyChanged;
        }

        public override void ViewWillDisappear()
        {
            _viewModel.PropertyChanged -= viewModel_PropertyChanged;
            base.ViewWillDisappear();
        }

        private void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IOptionsPanelViewModel.Options))
            {
                ItemsSource = _viewModel.Options;
            }
        }

        void RegenerateViews()
        {
            if (itemsSource == null || rowDelegate == null)
                return;

            var children = view.ArrangedSubviews;
            foreach (var item in children)
            {
                if (item != flexibleSpace)
                    item.RemoveFromSuperview();
            }

            //we check for all the viewmodels from that group
            foreach (var templateOption in itemsSource)
            {
                var localView = (NSView)rowDelegate.GetViewForModel(templateOption);
                if (localView != null)
                {
                    var stackView = new NSStackView()
                    {
                        TranslatesAutoresizingMaskIntoConstraints = false,
                        Orientation = NSUserInterfaceLayoutOrientation.Horizontal,
                        Distribution = NSStackViewDistribution.Fill,
                        Alignment = NSLayoutAttribute.Leading,
                        Spacing = 10
                    };

                    stackView.Hidden = !templateOption.IsVisible;

                        //stackView.WantsLayer = true;
                        //stackView.Layer.BackgroundColor = NSColor.Green.CGColor;

                    templateOption.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(IOptionViewModel.DisplayName))
                            stackView.Hidden = !templateOption.IsVisible;
                    };

                    stackView.AddArrangedSubview(localView);

                    //only add the row in case there is a view
                    view.InsertArrangedSubview(stackView, view.ArrangedSubviews.Length - 1);
                    view.LeadingAnchor.ConstraintEqualTo(stackView.LeadingAnchor).Active = true;
                    view.TrailingAnchor.ConstraintEqualTo(stackView.TrailingAnchor).Active = true;
                }
            }
        }
    }
}

