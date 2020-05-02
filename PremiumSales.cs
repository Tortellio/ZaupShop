using System;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.API;

namespace ZaupShop
{
    public class PremiumSales
    {
        public bool PremiumsalesStart = false;
        private DateTime PremiumlastSale;
        private DateTime PremiumnextSale;
        private DateTime PremiumSaleEndTime;
        private DateTime PremiumSaleTime;
        private DateTime PremiumlastMsg = DateTime.Now;
        byte p = 3;
        public void StartPremiumSale()
        {
            ZaupShop sale = ZaupShop.Instance;
            PremiumSaleTime = DateTime.Now;
            PremiumsalesStart = true;
            PremiumSaleEndTime = PremiumSaleTime.AddMinutes(sale.Configuration.Instance.PremiumSaleTime);
        }
        public void ResetPremiumSale()
        {
            PremiumlastSale = DateTime.Now;
            double Random = UnityEngine.Random.Range(ZaupShop.Instance.Configuration.Instance.MinNextPremiumSaleTime, (ZaupShop.Instance.Configuration.Instance.MaxNextPremiumSaleTime + 1));
            PremiumnextSale = PremiumlastSale.AddMinutes(Random);
            PremiumlastMsg = DateTime.Now;
            PremiumsalesStart = false;
            Logger.Log("The next premium sale will start at " + PremiumnextSale, ConsoleColor.Green);
        }

        public void MsgPremiumSale(IRocketPlayer call)
        {
            TimeSpan time = PremiumnextSale - DateTime.Now;
            double timeD = Math.Round(time.TotalMinutes, 1);
            if (time.TotalMinutes >= 1.0 && !PremiumsalesStart)
                UnturnedChat.Say(call, "The next premium sale will start in " + timeD + " Minutes", "https://i.imgur.com/3gOMlxE.png");
            else if (time.TotalMinutes < 1 && !PremiumsalesStart)
                UnturnedChat.Say(call, "The next premium sale will start in " + (Math.Round(time.TotalSeconds)) + " Seconds", "https://i.imgur.com/3gOMlxE.png");
            else if (time.TotalSeconds <= 0 && PremiumsalesStart)
                UnturnedChat.Say("Premium Sale have already started", "https://i.imgur.com/3gOMlxE.png");
            return;
        }
        public void CheckPremiumSale()
        {
            ZaupShop sale = ZaupShop.Instance;
            if (!PremiumsalesStart)
            {
                if ((PremiumnextSale - DateTime.Now).Seconds <= 60 && (DateTime.Now - PremiumlastMsg).Seconds >= 60)
                {
                    UnturnedChat.Say("Premium Sale is starting in 1 minute", "https://i.imgur.com/3gOMlxE.png");
                    PremiumlastMsg = DateTime.Now;
                }
                if ((PremiumnextSale - DateTime.Now).TotalSeconds <= p && (DateTime.Now - PremiumlastMsg).TotalSeconds >= 1)
                {
                    if (p != 0)
                    {
                        UnturnedChat.Say(ZaupShop.Instance.Translate("psale_start", p, "https://i.imgur.com/3gOMlxE.png"));
                        PremiumlastMsg = DateTime.Now;
                        p -= (byte)1;
                    }
                    else if (!PremiumsalesStart)
                    {
                        UnturnedChat.Say(ZaupShop.Instance.Translate("psale_started", sale.Configuration.Instance.PremiumSaleTime), "https://i.imgur.com/3gOMlxE.png");
                        PremiumlastMsg = DateTime.Now;
                        StartPremiumSale();
                        Logger.LogWarning("Premium Sales have started");
                    }
                }
            }
            if (PremiumsalesStart)
            {
                p = 3;
                if ((PremiumSaleEndTime - DateTime.Now).TotalMinutes <= 1 && (PremiumSaleEndTime - DateTime.Now).TotalSeconds > 59 && (DateTime.Now - PremiumlastMsg).Seconds >= 60)
                {
                    UnturnedChat.Say(ZaupShop.Instance.Translate("psale_ending", 1, "minute"), "https://i.imgur.com/3gOMlxE.png");
                    PremiumlastMsg = DateTime.Now;
                }
                if ((PremiumSaleEndTime - DateTime.Now).TotalSeconds <= 10 && (PremiumSaleEndTime - DateTime.Now).TotalSeconds > 9 && (DateTime.Now - PremiumlastMsg).Seconds >= 10)
                {
                    UnturnedChat.Say(ZaupShop.Instance.Translate("psale_ending", 10, "seconds"), "https://i.imgur.com/3gOMlxE.png");
                    PremiumlastMsg = DateTime.Now;
                }
                if ((PremiumSaleEndTime - DateTime.Now).TotalSeconds <= 5 && (PremiumSaleEndTime - DateTime.Now).TotalSeconds > 4 && (DateTime.Now - PremiumlastMsg).Seconds >= 5)
                {
                    UnturnedChat.Say(ZaupShop.Instance.Translate("psale_ending", 5, "seconds"), "https://i.imgur.com/3gOMlxE.png");
                    PremiumlastMsg = DateTime.Now;
                }
                if (DateTime.Now >= PremiumSaleEndTime)
                {
                    UnturnedChat.Say(ZaupShop.Instance.Translate("psale_end"), "https://i.imgur.com/3gOMlxE.png");
                    Logger.LogWarning("Premium Sales have ended");
                    PremiumlastMsg = DateTime.Now;
                    ResetPremiumSale();
                }
            }
        }
    }

}
