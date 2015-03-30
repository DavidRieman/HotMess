using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace HotMess
{
    public class GameConfig : BaseNotifying
    {
        public ObservableCollection<PinConfig> Pins { get; set; }

        private bool debug;
        public bool Debug
        {
            get { return debug; }
            set
            {
                if (debug != value) { debug = value; NotifyPropertyChanged("Debug"); }
            }
        }

        private float lossRecoveryTime;
        public float LossRecoveryTime
        {
            get { return lossRecoveryTime; }
            set
            {
                if (lossRecoveryTime != value)
                {
                    lossRecoveryTime = value;
                    NotifyPropertyChanged("LossRecoveryTime");
                    NotifyPropertyChanged("LossRecoveryTimeString");
                }
            }
        }

        [XmlIgnore]
        public string LossRecoveryTimeString
        {
            get { return LossRecoveryTime.ToString(); }
            set
            {
                float f;
                if (float.TryParse(value, out f))
                {
                    LossRecoveryTime = f;
                }
                else
                {
                    LossRecoveryTime = 0;
                }
            }
        }
    }
}