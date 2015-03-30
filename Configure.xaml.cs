using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Toolbox;

namespace HotMess
{
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        private List<Key> heldKeys = new List<Key>();
        private GameConfig gameConfig = new GameConfig() { Pins = new ObservableCollection<PinConfig>() };

        public GameConfig GameConfig
        {
            get { return gameConfig; }
            set { gameConfig = value; }
        }

        public bool Accepted { get; set; }

        private ConfigurationWindow()
        {
            InitializeComponent();
        }

        public static ConfigurationWindow Show(Window owner, GameConfig currentConfig)
        {
            var window = new ConfigurationWindow()
            {
                Title = "Configuration",
                ShowInTaskbar = false,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                Width = 400,
                Height = 600,
            };

            // HACK: Copying details here to avoid having to notify property changes; Without this,
            //       if we just window.gameConfig = currentConfig, pins list would be blank, etc.
            window.gameConfig.Pins.Clear();
            foreach (var pin in currentConfig.Pins)
            {
                window.gameConfig.Pins.Add(pin);
            }
            window.gameConfig.Debug = currentConfig.Debug;
            window.gameConfig.LossRecoveryTime = currentConfig.LossRecoveryTime;

            window.ShowDialog();
            return window;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.Accepted = true;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!heldKeys.Contains(e.Key))
            {
                heldKeys.Add(e.Key);
            }
            UpdateKeyPins();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (heldKeys.Contains(e.Key))
            {
                heldKeys.Remove(e.Key);
            }
            UpdateKeyPins();
        }

        private void UpdateKeyPins()
        {
            var focused = FocusManager.GetFocusedElement(this);
            if (!(focused is TextBox))
            {
                GameLogic.UpdateActivePins(gameConfig, heldKeys);
            }
        }
    }
}