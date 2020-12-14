using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DogGo.Models.ViewModels
{
    public class WalkerProfileViewModel
    {
        public Walker Walker { get; set; }
        public List<Walks> Walk { get; set; }
        public List<Owner> Owner { get; set; }
        public string totalTimeString
        {
            get
            {
                int i = 0;
                foreach(Walks walk in Walk)
                {
                    i += walk.Duration;
                }
                if (i >= 3600)
                {
                    double total = Convert.ToDouble(i);
                    double seconds = total % 60.00;
                    double minutes = Math.Round((total % 3600.00) / 60.00);
                    double hours = Math.Round(total / 3600.00);
                    return $"Total Walk Time {hours}hr {minutes}min {seconds}sec";
                }
                if (i >= 60)
                {
                    double total = Convert.ToDouble(i);
                    double seconds = total % 60.00;
                    double minutes = Math.Round((total % 3600.00) / 60.00);
                    return $"Total Walk Time {minutes}min {seconds}sec";
                }
                else
                {
                    double second = Convert.ToDouble(i);
                    return $"Total walk Time {second}sec";
                }
            }
        }
    }
}
