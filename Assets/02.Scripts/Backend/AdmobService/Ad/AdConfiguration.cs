using GoogleMobileAds.Api;
using TingleAvoid.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;



namespace TingleAvoid.AD
{
    public class AdActions
    {
        public AdType _type;
        public UnityAction _onOpen;
        public UnityAction _onClose;
        public UnityAction<Reward> _onReward;
        public UnityAction<AdError> _onError;
    }

    public static class AdConfiguration
    {
        //@DEBUG_TRANSLATE
        ////Test용
        ///
        public static readonly string Banner_Android_Id = "ca-app-pub-3940256099942544/6300978111";

        public static readonly string Interstitial_Android_Id = "ca-app-pub-3940256099942544/1033173712";
        public static readonly string Interstitial_Ios_Id = "ca-app-pub-3940256099942544/4411468910";

        public static readonly string Reward_Android_Id = "ca-app-pub-3940256099942544/5224354917";
        public static readonly string Reward_Ios_Id = "ca-app-pub-3940256099942544/1712485313";

        public static readonly string Native_Android_Id = "ca-app-pub-3940256099942544/2247696110";
        public static readonly string Native_Ios_Id = "ca-app-pub-3940256099942544/3986624511";

        ////Real AdId
        //public static readonly string Interstitial_Android_Id = "ca-app-pub-5018200534083869/8862749067";
        //public static readonly string Interstitial_Ios_Id = "ca-app-pub-5018200534083869/9134211809";

        //public static readonly string Reward_Android_Id = "ca-app-pub-5018200534083869/7549667395";
        //public static readonly string Reward_Ios_Id = "ca-app-pub-5018200534083869/7821130130";

        //public static readonly string Native_Android_Id = "ca-app-pub-5018200534083869/7285356784";
        //public static readonly string Native_Ios_Id = "ca-app-pub-5018200534083869/3813015639";
    }
}
