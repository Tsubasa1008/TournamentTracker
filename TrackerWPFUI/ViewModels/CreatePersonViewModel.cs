using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerWPFUI.Models;
using TrackerLibrary.Models;

namespace TrackerWPFUI.ViewModels
{
    public class CreatePersonViewModel : Screen
    {
        protected TournamentsTestContext db = new TournamentsTestContext();

        private string _firstName = "";
        private string _lastName = "";
        private string _email = "";
        private string _cellphone = "";
        private string _errorMessage = "";

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                NotifyOfPropertyChange(() => Email);
            }
        }

        public string Cellphone
        {
            get { return _cellphone; }
            set
            {
                _cellphone = value;
                NotifyOfPropertyChange(() => Cellphone);
            }
        }

        public void CancelCreation()
        {
            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(new PersonModel());
            this.TryClose();
        }

        public bool CanCreatePerson(string firstName, string lastName, string email, string cellphone)
        {
            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(cellphone))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreatePerson(string firstName, string lastName, string email, string cellphone)
        {
            /*PersonModel p = new PersonModel();

            p.FirstName = firstName;
            p.LastName = lastName;
            p.EmailAddress = email;
            p.CellphoneNumber = cellphone;*/


            People p = new People
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                CellphoneNumber = cellphone
            };

            db.People.Add(p);
            UpdateDB();

            //GlobalConfig.Connection.CreatePerson(p);
            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(p);
            TryClose();
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
                ErrorMessage = $"There was a problem, update fail!({ e.Message })";
            }
        }
    }
}
