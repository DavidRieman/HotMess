using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media;
using System.Windows;

namespace HotMess
{
    public class GameLogic : BaseNotifying
    {
        private class Stage
        {
            public PinConfig Pin { get; set; }
            public string Message { get; set; }
        }

        private Thread thread;
        private Random rand = new Random();
        private Queue<Stage> pinSequence = new Queue<Stage>();
        private Stage currentStage;
        private List<PinConfig> mustHoldPins = new List<PinConfig>();
        private DateTime lossThreat;
        private string lossThreatReason;

        public GameLogic(GameConfig gameConfig, string initialMessage)
        {
            this.GameConfig = gameConfig;
            this.SetMessage(initialMessage);
            this.thread = new Thread(GameStateLoop)
            {
                IsBackground = true,
            };
            this.thread.Start();
        }

        private void GameStateLoop()
        {
            while (true)
            {
                if (this.IsPlaying)
                {
                    // If a loss threat has been brewing for long enough, mark the game as lost.
                    var currentLossThreat = this.lossThreat;
                    if (currentLossThreat != DateTime.MinValue)
                    {
                        var lossTime = currentLossThreat.AddSeconds(this.GameConfig.LossRecoveryTime);
                        var timeLeft = lossTime - DateTime.Now;
                        if (timeLeft <= TimeSpan.Zero)
                        {
                            LoseGame();
                        }
                        else
                        {
                            var printTimeLeft = string.Format("{0}\r\n{1:0}...", this.lossThreatReason, timeLeft.TotalSeconds);
                            this.SetMessage(printTimeLeft, Colors.Orange);
                        }
                    }
                    else
                    {
                        // Reinforce the current message, in case there was a loss threat a moment ago.
                        this.SetMessage(this.currentStage.Message);
                    }
                }

                Thread.Sleep(10);
            }
        }

        private GameConfig gameConfig;
        public GameConfig GameConfig
        {
            get { return gameConfig; }
            set { if (gameConfig != value) { gameConfig = value; NotifyPropertyChanged("GameConfig"); } }
        }

        private bool isPlaying;
        public bool IsPlaying
        {
            get { return isPlaying; }
            private set { if (isPlaying != value) { isPlaying = value; NotifyPropertyChanged("IsPlaying"); } }
        }

        private string gameMessage;
        public string GameMessage
        {
            get { return gameMessage; }
        }

        private Brush gameMessageColor = new SolidColorBrush(Colors.Black);
        public Brush GameMessageColor
        {
            get { return gameMessageColor; }
        }

        public void SetMessage(string message, Color? color = null)
        {
            if (color == null)
                color = Colors.Black;
            
            this.gameMessage = message;
            Application.Current.BeginInvoke(() =>
            {
                this.gameMessageColor = new SolidColorBrush(color.Value);
                NotifyPropertyChanged("GameMessage");
                NotifyPropertyChanged("GameMessageColor");
            });
        }

        private bool MeetsLossConditions()
        {
            // If any pins in the 'must hold' list is no longer held, then we are losing.
            var lostConnects = from pin in this.mustHoldPins where !pin.IsActive select pin;
            if (lostConnects.Any())
            {
                // Build a list of all connections which are currently lost, in a format suitable for display.
                var lostConnectionsList = string.Join(" & ", from l in lostConnects select l.Name);
                this.lossThreatReason = string.Format("Lost connection from {0}!", lostConnectionsList);
                return true;
            }

            // If any pins (aside from the next one up) are held, then we are losing due to incorrect hold.
            var incorrectHolds = (from pin in this.pinSequence where pin.Pin.IsActive select pin);
            if (incorrectHolds.Any())
            {
                this.lossThreatReason = "Incorrect connection";
                return true;
            }

            return false;
        }

        public void UpdateActivePins(IEnumerable<Key> heldKeys)
        {
            if (this.IsPlaying)
            {
                UpdateActivePins(this.GameConfig, heldKeys);

                if (MeetsLossConditions())
                {
                    // If this is a new loss threat, mark it. Otherwise leave the time we started this threat alone.
                    if (this.lossThreat == DateTime.MinValue)
                    {
                        this.lossThreat = DateTime.Now;
                    }
                }
                else
                {
                    // No current threat of loss.
                    this.lossThreat = DateTime.MinValue;

                    // Check for Advancement
                    if (this.currentStage.Pin.IsActive)
                    {
                        this.AdvanceSequence();
                    }
                }
            }
        }

        public static void UpdateActivePins(GameConfig gameConfig, IEnumerable<Key> heldKeys)
        {
            // Find the pins which map to any held key, and mark them
            var activeGameKeys = KeyCompression.DecompressKeys(heldKeys);
            foreach (var pin in gameConfig.Pins)
            {
                pin.IsActive = activeGameKeys.Contains(pin.Key);
            }
        }

        public void NewGame()
        {
            this.pinSequence.Clear();
            this.mustHoldPins.Clear();
            this.currentStage = null;
            this.lossThreat = DateTime.MinValue;

            // Prepare the randomized connection sequence; exclude the ground as we'll work with it specially.
            var gamePins = from pin in this.GameConfig.Pins
                           where pin.IsEnabled && pin.Id != "Ground"
                           orderby Guid.NewGuid()
                           select pin;
            if (!gamePins.Any())
            {
                this.IsPlaying = false;
                return;
            }

            // Prepare game messages; the first message is special as it includes connect-to-ground messaging.
            var firstPin = gamePins.First();
            var ground = (from pin in this.GameConfig.Pins where pin.Id == "Ground" select pin).Single();
            var firstMessage = string.Format("Connect {0} to {1}.", firstPin.Name, ground.Name);
            this.pinSequence.Enqueue(new Stage() { Pin = firstPin, Message = firstMessage });
            foreach (var pin in gamePins.Where(p => p != firstPin))
            {
                var message = string.Format("Add {0}", pin.Name);
                this.pinSequence.Enqueue(new Stage(){Pin = pin, Message = message});
            }

            AdvanceSequence();
            
            this.IsPlaying = true;
        }

        private void AdvanceSequence()
        {
            if (this.pinSequence.Any())
            {
                // The key required for the stage we are advancing from, must continue to be held.
                if (this.currentStage != null)
                {
                    this.mustHoldPins.Add(this.currentStage.Pin);
                }

                this.currentStage = this.pinSequence.Dequeue();
                this.SetMessage(this.currentStage.Message);
            }
            else
            {
                // Advancing past the last stage? Game Victory!
                this.WinGame();
            }
        }

        private void LoseGame()
        {
            var message = string.Format("Oh no! You lose! ({0})", this.lossThreatReason);
            this.SetMessage(message, Colors.Red);
            this.IsPlaying = false;
        }

        private void WinGame()
        {
            this.SetMessage("Well done! You win!", Colors.Green);
            this.IsPlaying = false;
        }
    }
}
