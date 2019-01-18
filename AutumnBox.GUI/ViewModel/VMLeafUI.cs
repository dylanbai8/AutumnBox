﻿using AutumnBox.GUI.MVVM;
using AutumnBox.GUI.Util.Debugging;
using AutumnBox.GUI.Util.UI;
using AutumnBox.GUI.View.LeafContent;
using AutumnBox.GUI.View.Windows;
using AutumnBox.OpenFramework.Extension.LeafExtension;
using MaterialDesignThemes.Wpf;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutumnBox.GUI.ViewModel
{
    class VMLeafUI : ViewModelBase, ILeafUI
    {
        private enum State
        {
            Initing = -1,
            Ready = 0,
            Running = 1,
            Finished = 2,
            Shutdown = 3,
            Unfinished = 4,
        }
        private State CurrentState { get; set; } = State.Initing;

        public FlexiableCommand Copy
        {
            get => _copy; set
            {
                _copy = value;
                RaisePropertyChanged();
            }
        }
        private FlexiableCommand _copy;

        public bool IsIndeterminate
        {
            get => _isIndeterminate; set
            {
                _isIndeterminate = value;
                RaisePropertyChanged();
            }
        }
        private bool _isIndeterminate;

        public LeafWindow View
        {
            get => _view; set
            {
                _view = value;
                RaisePropertyChanged();
                InitView();
            }
        }
        private LeafWindow _view;

        public string Content
        {
            get => _contentBuilder?.ToString();
            set { }
        }
        private readonly StringBuilder _contentBuilder;

        public double Progress
        {
            get => _progress; set
            {
                ThrowIfNotRunning();
                if (value == -1)
                {
                    IsIndeterminate = true;
                    _progress = 0;
                    RaisePropertyChanged();
                    return;
                }
                IsIndeterminate = false;
                _progress = value;
                RaisePropertyChanged();
            }
        }
        private double _progress;

        public string Tip
        {
            get => _tip; set
            {
                ThrowIfNotRunning();
                _tip = value;
                RaisePropertyChanged();
            }
        }
        private string _tip;

        public System.Drawing.Size Size
        {
            get
            {
                System.Drawing.Size size = new System.Drawing.Size();
                View.Dispatcher.Invoke(() =>
                {
                    size.Height = (int)View.Height;
                    size.Width = (int)View.Width;
                });
                return size;
            }
            set
            {
                ThrowIfNotRunning();
                View.Dispatcher.Invoke(() =>
                {
                    View.Height = value.Height;
                    View.Width = value.Width;
                });
            }
        }

        public byte[] Icon
        {
            get => _icon; set
            {
                ThrowIfNotRunning();
                _icon = value;
                RaisePropertyChanged();
            }
        }
        private byte[] _icon;

        public string Title
        {
            get => _title; set
            {
                ThrowIfNotRunning();
                _title = value;
                RaisePropertyChanged();
            }
        }
        private string _title;

        public event EventHandler<LeafCloseBtnClickedEventArgs> CloseButtonClicked;

        private Panel InnerPanel { get; set; }

        private void InitView()
        {
            if (CurrentState != State.Ready) return;
            View.Closing += View_Closing;
            InnerPanel = (View.Content as Panel);
        }

        public VMLeafUI()
        {
            _contentBuilder = new StringBuilder();
            RaisePropertyChangedOnDispatcher = true;
            Title = "LeafUI Window";
            Progress = -1;
            Tip = App.Current.Resources["RunningWindowStateRunning"] as string;
            Icon = null;
            Copy = new FlexiableCommand(() =>
            {
                try
                {
                    Clipboard.SetText(Content);
                }
                catch { }
            });
            CurrentState = State.Ready;
        }

        private void View_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (CurrentState == State.Shutdown || CurrentState == State.Finished || CurrentState == State.Unfinished)
            {
                e.Cancel = false;
            }
            else
            {
                LeafCloseBtnClickedEventArgs args = new LeafCloseBtnClickedEventArgs
                {
                    CanBeClosed = false
                };
                CloseButtonClicked?.Invoke(this, args);
                e.Cancel = !args.CanBeClosed;
                if (args.CanBeClosed) Finish();
                else WriteLine(App.Current.Resources["RunningWindowCantStop"]);
            }
        }

        public void EnableHelpBtn(Action callback)
        {
            View.Dispatcher.Invoke(() =>
            {
                HelpButtonHelper.EnableHelpButton(View, callback);
            });
        }

        public void Finish(int exitCode = 0)
        {
            ThrowIfNotRunning();
            App.Current.Dispatcher.Invoke(() =>
            {
                Finish(App.Current.Resources["LeafUITipCode" + exitCode] as string
                    ?? App.Current.Resources["LeafUITipCodeUnknown"] as string);
            });
        }

        public void Finish(string tip)
        {
            ThrowIfNotRunning();
            Tip = tip;
            Progress = 100;
            CurrentState = State.Finished;
        }

        public void Show()
        {
            if (CurrentState != State.Ready)
            {
                throw new InvalidOperationException("Leaf UI is not ready!");
            }
            View.Dispatcher.Invoke(() =>
            {
                View.Show();
            });
            CurrentState = State.Running;
        }

        public void Shutdown()
        {
            CurrentState = State.Shutdown;
        }

        public void WriteLine(object content)
        {
            ThrowIfNotRunning();
            _contentBuilder.AppendLine(content?.ToString());
            RaisePropertyChanged(nameof(Content));
        }

        public void Dispose()
        {
            //Trace.WriteLine("LeafUI dispose");
            if (CurrentState == State.Finished)
            {
                return;
            }
            else if (CurrentState == State.Running)
            {
                CurrentState = State.Unfinished;
            }
            View.Dispatcher.Invoke(() =>
            {
                View.Close();
            });
            View = null;
        }

        private void ThrowIfNotRunning()
        {
            if (CurrentState == State.Shutdown || CurrentState == State.Finished)
            {
                throw new InvalidOperationException("Leaf UI is locked!");
            }
        }

        public void ShowMessage(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Task<object> task = null;
            App.Current.Dispatcher.Invoke(() =>
            {
                var view = new MessageView(message);
                task = View.fuck.ShowDialog(view);
            });
            task.Wait();
        }

        public bool DoYN(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Task<object> task = null;
            App.Current.Dispatcher.Invoke(() =>
            {
                var view = new YNView(message,null,null);
                task = View.fuck.ShowDialog(view);
            });
            task.Wait();
            return (bool)task.Result;
        }

        public bool? DoChoice(string message, string btnYes = null, string btnNo = null, string btnCancel = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Task<object> task = null;
            App.Current.Dispatcher.Invoke(() =>
            {
                var view = new ChoiceView(message,btnYes,btnNo,btnCancel);
                task = View.fuck.ShowDialog(view);
            });
            task.Wait();
            return (task.Result as bool?);
        }

        public object SelectFrom(object[] options,string hint=null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            Task<object> task = null;
            App.Current.Dispatcher.Invoke(() =>
            {
                var view = new SingleSelectView(hint,options);
                task = View.fuck.ShowDialog(view);
            });
            task.Wait();
            return task.Result;
        }

        public object[] Select(object[] option, int maxSelect = 1)
        {
            throw new NotImplementedException();
        }
    }
}
