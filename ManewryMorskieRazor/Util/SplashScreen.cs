using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public struct SplashScreen
    {
        public string? Message { get; private set; }
        public bool IsDismisableByUser { get; private set; }

        public SplashScreen(string? message, bool isDismisableByUser) 
            => (Message, IsDismisableByUser) = (message, isDismisableByUser);
    }
}
