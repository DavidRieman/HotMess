using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Toolbox;

namespace HotMess
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private const string ConfigFileName = "Config.xml";
        private List<Key> heldKeys = new List<Key>();

        public MainWindow()
        {
            InitializeComponent();

            GameConfig gameConfig = null;
            if (File.Exists(ConfigFileName))
            {
                gameConfig = XmlUtilities.DeserializeFromXmlFile<GameConfig>(ConfigFileName);
            }
            else
            {
                gameConfig = new GameConfig()
                {
                    Pins = new ObservableCollection<PinConfig>(PinConfig.DefaultConfiguration),
                    Debug = false,
                    LossRecoveryTime = 5,
                };
            }

            string initialMessage = "Press F12 to configure. Press F11 to begin game.";
            this.GameLogic = new GameLogic(gameConfig, initialMessage);
        }

        private GameLogic gameLogic;
        public GameLogic GameLogic
        {
            get { return gameLogic; }
            set
            {
                if (gameLogic != value)
                {
                    gameLogic = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("GameLogic"));
                }
            }
        }

        private void SetGameText(string gameText)
        {
            this.textBlockGameText.Text = gameText;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!heldKeys.Contains(e.Key))
            {
                heldKeys.Add(e.Key);
            }

            this.UpdateKeySummary();
            this.GameLogic.UpdateActivePins(heldKeys);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12)
            {
                HandleConfiguration();
                return;
            }
            else if (e.Key == Key.F11)
            {
                this.GameLogic.NewGame();
                return;
            }

            if (heldKeys.Contains(e.Key))
            {
                heldKeys.Remove(e.Key);
            }

            this.UpdateKeySummary();
            this.GameLogic.UpdateActivePins(heldKeys);
        }

        private void HandleConfiguration()
        {
            var config = ConfigurationWindow.Show(this, this.GameLogic.GameConfig);
            if (config.Accepted)
            {
                this.GameLogic.GameConfig = config.GameConfig;
                XmlUtilities.SerializeToXmlFile(this.GameLogic.GameConfig, ConfigFileName);
            }
        }

        private void UpdateKeySummary()
        {
            textBlockHeldKeys.Text = KeyCompression.Summary(heldKeys); 
        }
    }
}