using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace HotMess
{
    public static class ApplicationExtensions
    {
        public static void BeginInvoke(this Application app, Action action)
        {
            // Get the application dispatcher; check for null to avoid attempts to dispatch while
            // the application is shutting down, etc.
            var dispatcher = app != null ? app.Dispatcher : null;
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(action);
            }
        }
    }
}
