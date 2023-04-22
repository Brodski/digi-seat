using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiSeatShared
{
    public static class Settings
    {
        public static string ApiBaseUrl
        {
            get
            {
                //return "http://localhost:50323";
                return "http://localhost:50324";
                //return "https://digiseatapi.azurewebsites.net";
            }
        }

        public static string LightType
        {
            get
            {
                return "playbulb";
                //return "philipshue";
            }
        }

        public static bool UseLightsAsCandle
        {
            get
            {
                return true;
            }
        }

        public static int LightColorDurationMinutes
        {
            get
            {
                return 1;
            }
        }

        public static string ApiKey
        {
            get
            {
                return "a480edc7-811b-4cfa-a216-47fc31697ebe";
            }
        }
    }
}
