using ManewryMorskie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class DialogService
    {
        public event EventHandler<int>? OptionChoosed;
        public event Func<string, string[], Task>? NewOptionsSet;
        public event Func<string, MessageType, Task>? NewMessage;
        public event Func<SplashScreen?, Task>? SplashScreenDisplayed;

        public async Task DisplayMessage(string message, MessageType msgType = MessageType.Standard)
        {
            if (NewMessage != null)
                await NewMessage.Invoke(message, msgType);
        }

        public async Task DisplayOptions(string title, params string[] options)
        {
            if(NewOptionsSet != null)
                await NewOptionsSet.Invoke(title, options);
        }

        public void ChooseOption(int id)
        {
            OptionChoosed?.Invoke(this, id);
        }

        public async Task DisplaySplashScreen(SplashScreen? splashScreen)
        {
            if(SplashScreenDisplayed != null)
                await SplashScreenDisplayed.Invoke(splashScreen);
        }
    }

}
