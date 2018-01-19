using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerWPFUI.Models;

namespace TrackerWPFUI.ViewModels
{
    public class CreatePrizeViewModel : Screen
    {
        protected TournamentsTestContext db = new TournamentsTestContext();
        
        private int _placeNumber;
        private string _placeName;
        private decimal _prizeAmount;
        private double _prizePercentage;

        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }


        public int PlaceNumber
        {
            get { return _placeNumber; }
            set
            {
                _placeNumber = value;
                NotifyOfPropertyChange(() => PlaceNumber);
            }
        }

        public string PlaceName
        {
            get { return _placeName; }
            set
            {
                _placeName = value;
                NotifyOfPropertyChange(() => PlaceName);
            }
        }

        public decimal PrizeAmount
        {
            get { return _prizeAmount; }
            set
            {
                _prizeAmount = value;
                NotifyOfPropertyChange(() => PrizeAmount);
            }
        }

        public double PrizePercentage
        {
            get { return _prizePercentage; }
            set
            {
                _prizePercentage = value;
                NotifyOfPropertyChange(() => PrizePercentage);
            }
        }

        public bool CanCreatePrize(int placeNumber, string placeName, decimal prizeAmount, double prizePercentage)
        {
            return ValidateForm(placeNumber, placeName, prizeAmount, prizePercentage);
        }

        public void CreatePrize(int placeNumber, string placeName, decimal prizeAmount, double prizePercentage)
        {
            /*prizes model = new prizes
            {
                PlaceNumber = placeNumber,
                PlaceName = placeName,
                PrizeAmount = prizeAmount,
                PrizePercentage = prizePercentage
            };

            db.prizes.Add(model);
            UpdateDB();*/

            //PrizeModel model = new PrizeModel
            //{
            //    PlaceNumber = placeNumber,
            //    PlaceName = placeName,
            //    PrizeAmount = prizeAmount,
            //    PrizePercentage = prizePercentage
            //};

            //GlobalConfig.Connection.CreatePrize(model);

            Prize p = new Prize
            {
                PlaceNumber = PlaceNumber,
                PlaceName = PlaceName,
                PrizeAmount = PrizeAmount,
                PrizePercentage = PrizePercentage
            };

            db.Prizes.Add(p);
            UpdateDB();

            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(p);
            this.TryClose();
        }

        private bool ValidateForm(int placeNumber, string placeName, decimal prizeAmount, double prizePercentage)
        {
            bool output = true;

            if (placeNumber < 1)
            {
                output = false;
            }

            if (String.IsNullOrWhiteSpace(placeName))
            {
                output = false;
            }

            if (prizeAmount <= 0 && prizePercentage <= 0)
            {
                output = false;
            }

            if (prizePercentage < 0 || prizePercentage > 100)
            {
                output = false;
            }

            return output;
        }

        public void CancelCreation()
        {
            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(new Prize());
            this.TryClose();
        }

        private async void UpdateDB()
        {
            try
            {
                await db.SaveChangesAsync();
                ErrorMessage = "Database Update Success!";
            }
            catch (Exception e)
            {
                ErrorMessage = $"Database Update Fail. ({ e.Message })";
            }
        }
    }
}
