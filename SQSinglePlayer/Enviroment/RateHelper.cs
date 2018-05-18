using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SQSinglePlayer.Enviroment
{
    class RateHelper
    {
        public void RateAppMessage()
        {
            MessageBoxResult mm =  MessageBox.Show("ΑΜΑ Entertainment. Ευχαριστούμε που παίξατε το Sporty Quiz Single Player. Η γνώμη σας θα μας βοηθήσει να γίνουμε καλύτεροι. Παρακαλούμε αξιολογήστε το Quiz","Sporty Quiz", MessageBoxButton.OKCancel);
            if (mm == MessageBoxResult.OK)
            {
                App.AppSettings.AppRated = true;
                MarketplaceReviewTask rr = new MarketplaceReviewTask();
                rr.Show();
            }
            if (mm == MessageBoxResult.Cancel)
            {

            }
        }
    }
}
