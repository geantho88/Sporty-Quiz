using GoogleAds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQSinglePlayer.Enviroment
{
    class Monetize
    {
        public static AdView SetAdBanner(AdFormats format, string AdUnitID)
        {
            AdView bannerAd = new AdView
            {
                Format = format,
                AdUnitID = AdUnitID
            };

            return bannerAd;
        }
    }
}
