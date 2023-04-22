using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace DigiSeatShared
{
    //Maybe make this a static class??????
    public class Dialogues
    {
        public async Task ShowErrorDialog(string message)
        {
            var dialog = new ContentDialog
            {
                Content = $"We ran into a problem. Relaunch app or notify support if problem persists.\n\n{message}",
                IsSecondaryButtonEnabled = false,
                PrimaryButtonText = "Okay",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();
        }

        public async Task<int> GetUserInput(string message)
        {
            TextBox inputbox = new TextBox();
            inputbox.AcceptsReturn = false;
            inputbox.Height = 32;
            var dialog = new ContentDialog
            {
                Title = message,
                Content = inputbox,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Continue",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary
            };
            int result = -1;
            var clicked = await dialog.ShowAsync();
            if (clicked == ContentDialogResult.Primary)
            {
                Int32.TryParse(inputbox.Text, out result);
            }
            return result;
            

        }
    }
}
