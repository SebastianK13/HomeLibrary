using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomeLibrary.Models.ViewModels
{
    public class PendingOffers
    {
        private List<PendingOffers> tempPendingOffer;
        public PendingOffers()
        {
        }

        public PendingOffers(List<Offer> offers)
        {
            tempPendingOffer = new List<PendingOffers>();
            string action = "";
            foreach(var o in offers)
            {
                if (o.Swap.Away)
                {
                    action = "Away";
                }
                else if(o.Swap.Sell)
                {
                    action = "Sell";
                }

                tempPendingOffer.Add(new PendingOffers(){
                    OfferID = o.OfferID,
                    Action = action,
                    SourceImg = o.Swap.Book.ImageSource,
                    ProposeDesc = o.ProposeDesc,
                    SwapBookTitle = o.Swap.Book.Title,
                    ProposerNick = o.Profile.Nickname,
                    MyAcceptation = o.Swap.Closed,
                    ProposerAcc = o.Closed
                });

            }
            PendingOffersList = tempPendingOffer;
        }

        public PendingOffers(List<SwapPropose> proposes)
        {
            tempPendingOffer = new List<PendingOffers>();
            string action = "Swap";
            foreach (var p in proposes)
            {

                tempPendingOffer.Add(new PendingOffers()
                {
                    OfferID = p.ProposeID,
                    Action = action,
                    SourceImg = p.Book.ImageSource,
                    OfferedBookTitle = p.Book.Title,
                    SwapBookTitle = p.Swap.Book.Title,
                    ProposerNick = p.Book.Profile.Nickname,
                    MyAcceptation = p.Swap.Closed,
                    ProposerAcc = p.Closed
                });

            }
            PendingOffersList = tempPendingOffer;
        }

        public List<PendingOffers> PendingOffersList { get; set; }
        public int OfferID { get; set; }
        public string Action { get; set; }
        public string SourceImg { get; set; }
        public string OfferedBookTitle { get; set; }
        public string ProposeDesc { get; set; }
        public string SwapBookTitle { get; set; }
        public string ProposerNick { get; set; }
        public bool MyAcceptation { get; set; }
        public bool ProposerAcc { get; set; }
    }
    public class ClosedSwaps
    {

    }
}