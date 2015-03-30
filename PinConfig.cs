using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml.Serialization;
using System.Windows.Media;

namespace HotMess
{
    public class PinConfig : BaseNotifying
    {
        private static Brush ActiveForegroundColor = new SolidColorBrush(Colors.Black);
        private static Brush InactiveForegroundColor = new SolidColorBrush(Colors.Black);
        private static Brush ActiveBackgroundColor = new SolidColorBrush(Colors.Green);
        private static Brush InactiveBackgroundColor = new SolidColorBrush(Colors.White);

        private string id;
        public string Id
        {
            get { return id; }
            set { if (id != value) { id = value; NotifyPropertyChanged("Id"); } }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { if (name != value) { name = value; NotifyPropertyChanged("Name"); } }
        }

        private bool isEnabled;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { if (isEnabled != value) { isEnabled = value; NotifyPropertyChanged("IsEnabled"); } }
        }

        private Key key;
        public Key Key
        {
            get { return key; }
            set { if (key != value) { key = value; NotifyPropertyChanged("Key"); NotifyPropertyChanged("KeyName"); } }
        }

        public string KeyName
        {
            get { return Key.ToString(); }
        }

        private bool isActive;
        [XmlIgnore]
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    NotifyPropertyChanged("IsActive");
                    NotifyPropertyChanged("DebugForegroundColor");
                    NotifyPropertyChanged("DebugBackgroundColor");
                }
            }
        }

        [XmlIgnore]
        public Brush DebugForegroundColor { get { return IsActive ? ActiveForegroundColor : InactiveForegroundColor; } }

        [XmlIgnore]
        public Brush DebugBackgroundColor { get { return IsActive ? ActiveBackgroundColor : InactiveBackgroundColor; } }

        public static IEnumerable<PinConfig> DefaultConfiguration
        {
            get
            {
                yield return new PinConfig() { Id = "Ground", Key = Key.None, Name = "Ground", IsEnabled = true };
                yield return new PinConfig() { Id = "A0",     Key = Key.F3,   Name = "0",      IsEnabled = true };
                yield return new PinConfig() { Id = "A1",     Key = Key.F2,   Name = "1",      IsEnabled = false };
                yield return new PinConfig() { Id = "A2",     Key = Key.F1,   Name = "2",      IsEnabled = true };
                yield return new PinConfig() { Id = "A3",     Key = Key.D3,   Name = "3",      IsEnabled = false };
                yield return new PinConfig() { Id = "A4",     Key = Key.D2,   Name = "4",      IsEnabled = true };
                yield return new PinConfig() { Id = "A5",     Key = Key.D1,   Name = "5",      IsEnabled = false };
                yield return new PinConfig() { Id = "D0",     Key = Key.X,    Name = "6",      IsEnabled = true };
                yield return new PinConfig() { Id = "D1",     Key = Key.W,    Name = "7",      IsEnabled = false };
                yield return new PinConfig() { Id = "D2",     Key = Key.V,    Name = "8",      IsEnabled = true };
                yield return new PinConfig() { Id = "D3",     Key = Key.Q,    Name = "9",      IsEnabled = false };
                yield return new PinConfig() { Id = "D4",     Key = Key.P,    Name = "10",     IsEnabled = true };
                yield return new PinConfig() { Id = "D5",     Key = Key.O,    Name = "11",     IsEnabled = false };
                yield return new PinConfig() { Id = "Click",  Key = Key.J,    Name = "12",     IsEnabled = true };
                yield return new PinConfig() { Id = "Space",  Key = Key.I,    Name = "13",     IsEnabled = false };
                yield return new PinConfig() { Id = "Left",   Key = Key.H,    Name = "14",     IsEnabled = true };
                yield return new PinConfig() { Id = "Right",  Key = Key.C,    Name = "15",     IsEnabled = false };
                yield return new PinConfig() { Id = "Down",   Key = Key.B,    Name = "17",     IsEnabled = false };
                yield return new PinConfig() { Id = "Up",     Key = Key.A,    Name = "16",     IsEnabled = true };
            }
        }
    }
}