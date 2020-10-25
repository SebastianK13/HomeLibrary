using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models.ViewModels
{
    public class SwapViewModel
    {
        public SwapViewModel()
        {
        }
        public SwapViewModel(List<Swap> swaps)
        {
            SwapView = new List<SwapViewModel>();
            foreach (var s in swaps)
            {
                SwapView.Add(new SwapViewModel()
                {
                    Title = s.Book.Title,
                    SwapID = s.SwapID,
                    Source = s.Book.ImageSource,
                    ActionName = SetActionName(s),
                    DescriptionOffer = SetDescription(s),
                    Currency = s.Currency
                });
            }
        }
        public List<SwapViewModel> SwapView;
        public string ActionName { get; set; }
        public string DescriptionOffer { get; set; }
        public string Title { get; set; }
        public string Source { get; set; }
        public string Currency { get; set; }
        public int SwapID { get; set; }
        private string temp;

        public string SetActionName(Swap s)
        {
            string actionName = "Give it away";
            if(s.Away == true)
            {
                actionName = "Give it away";
            }
            else if (s.Sell == true)
            {
                actionName = "Sell";
            }
            else if(s.SwapB == true)
            {
                actionName = "Swap book";
            }

            temp = actionName;

            return actionName;
        }
        public string SetDescription(Swap s)
        {
            string Description = "";

            switch (temp)
            {
                case "Give it away":
                    break;
                case "Sell":
                    string precisionPrice = s.Price.ToString();
                    precisionPrice = precisionPrice.Remove(precisionPrice.Length - 2);
                    Description = "Amount claimed: " + precisionPrice +" "+ s.Currency;
                    break;
                case "Swap book":
                    Description = s.DescSwap;
                    break;
            }

            return Description;
        }
    }
}