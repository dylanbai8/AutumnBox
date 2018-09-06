﻿using AutumnBox.Basic.FlowFramework;
using System;
using System.Windows;
using System.Windows.Input;

namespace AutumnBox.GUI.Windows
{
    /// <summary>
    /// PullingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PullingWindow : Window
    {
        private readonly IFunctionFlowBase flow;
        public PullingWindow(IFunctionFlowBase _flow)
        {
            InitializeComponent();

            flow = _flow;
            flow.OutputReceived += (s, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    TBOutput.AppendText(e.Text);
                    TBOutput.ScrollToEnd();
                });
            };
            flow.NoArgFinished += (s, e) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Close();
                });
            };
            if (flow.IsClosed) throw new Exception("do not run flow!");
            flow.MustTiggerAnyFinishedEvent = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            flow.RunAsync();
            new TBLoadingEffect(TBLoading).Start();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch{ }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            flow.ForceStop();
        }
    }
}
