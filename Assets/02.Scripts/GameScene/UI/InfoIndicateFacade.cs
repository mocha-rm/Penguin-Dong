using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;
using VContainer.Unity;

using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using MessagePipe;
using GameScene.Message;
using Cysharp.Threading.Tasks.CompilerServices;


namespace GameScene.UI
{
    public class InfoIndicateFacade : BaseFacade, IRegistMonobehavior
    {
        //what`s need ?
        //Info Indicate Part : Score(Text), Life(Image), Level(according to spend time , increase icicle speed / motif from minigame heaven)

        //Register
        TextMeshProUGUI _scoreText;
        TextMeshProUGUI _countText;
        
        Transform _levelTr;

        //Use
        float _totalHP;
        float _curHP;
        Image _hpBar;
        TextMeshProUGUI _hpText;

        Image _levelProceedImage;
        TextMeshProUGUI _currentLevelText;

        int _countNumber = 0;
        int _prevScore = 0;

        IPublisher<CountDownComplete> _countdownPub;

        CancellationTokenSource _cts;

        AudioService _audioService;



        public void RegistBehavior(IContainerBuilder builder)
        {
            _scoreText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.ScoreText);
            _countText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.CountText);

            _hpBar = gameObject.GetHierachyPath<Image>(Hierarchy.HPBar);
            _hpText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.HPText);

            _levelTr = gameObject.GetHierachyPath<Transform>(Hierarchy.LevelTr);
        }


        public override void Initialize()
        {
            _audioService = _container.Resolve<AudioService>();
            

            _scoreText.text = "0";

            _hpBar.fillAmount = 1.0f;
            _curHP = Constants.DefaultHP;
            _hpText.text = $"{_curHP} / {Constants.DefaultHP}";

            _levelProceedImage = _levelTr.GetChild((int)Constants.LevelChild.ProceedImage).GetComponent<Image>();
            _currentLevelText = _levelTr.GetChild((int)Constants.LevelChild.LevelText).GetComponent<TextMeshProUGUI>();

            _countdownPub = _container.Resolve<IPublisher<CountDownComplete>>();

            _cts = new CancellationTokenSource();

            StartCountdown().Forget();
        }


        public override void Dispose()
        {
            _cts.Cancel();
            _cts = null;
        }

        #region Public Methods
        public void IndicateScore(int score)
        {
            RollUpScore(_prevScore, score).Forget();
            _prevScore = score;
        }

        public void IndicateLifeStatus(float damage, float ability)
        {
            _curHP -= damage;

            //hptext action
            _totalHP = Constants.DefaultHP + ability;
            _hpText.text = $"{_curHP} / {_totalHP}";

            //hpbar action
            //_hpBar.fillAmount -= damage * 0.01f;
            _hpBar.fillAmount = _curHP / _totalHP;
        }

        public void IndicateLifeStatus(float heal)
        {
            if (_curHP < _totalHP)
            {
                _curHP += _totalHP * heal;
                _hpText.text = $"{_curHP} / {_totalHP}";

                //_hpBar.fillAmount += heal * 0.01f;
                _hpBar.fillAmount = _curHP / _totalHP;
            }
        }

        public void SetLifeStatus(float setValue)
        {
            _hpText.text = setValue.ToString();
        }

        public void IndicateLevelStatus(float guage, int level)
        {
            LevelGaugeUp(guage, level).Forget();
        }
        #endregion


        #region Private Methods
        private async UniTaskVoid RollUpScore(float current, float next)
        {
            float offset = (next - current) / Constants.RollDuration;

            while (current < next)
            {
                current += offset * Time.deltaTime;

                _scoreText.text = current.ToString("f0");

                await UniTask.Yield();
            }

            current = next;

            _scoreText.text = current.ToString();
        }

        private async UniTask LevelGaugeUp(float guage, int level)
        {
            float dt = 0f;
            float curFill = _levelProceedImage.fillAmount;

            float duration = Mathf.Abs(curFill - guage) / Constants.LevelUpSpeed;

            _currentLevelText.text = level.ToString();

            while (dt < duration)
            {
                dt += Time.deltaTime;

                _levelProceedImage.fillAmount = Mathf.Lerp(curFill, guage, dt / duration);

                await UniTask.Yield(_cts.Token);
            }

            _levelProceedImage.fillAmount = guage;
        }

        private async UniTaskVoid StartCountdown()
        {
            _countNumber = Constants.countdown;

            while (_countNumber > 0)
            {
                _countText.text = _countNumber.ToString();
                //_audioService.Play(AudioService.AudioResources.Count);
                --_countNumber;

                await UniTask.Delay(System.TimeSpan.FromMilliseconds(1000));
            }

            _countdownPub.Publish(new CountDownComplete()
            {

            });

            _countText.text = $"Go!";

            await UniTask.Delay(System.TimeSpan.FromMilliseconds(1000));          

            _countText.gameObject.SetActive(false);
        }

        #endregion




        public static class Constants
        {
            public static readonly float DefaultHP = 100.0f;

            public static readonly int countdown = 3;

            public enum LevelChild
            {
                ProceedImage,
                LevelText
            }

            public static readonly float RollDuration = 0.6f;

            public static readonly float LevelUpSpeed = 0.75f;
        }

        public static class Hierarchy
        {
            public static readonly string ScoreText = "Text_Score";
            public static readonly string CountText = "CountDown";

            public static readonly string HPBar = "Life/HP_Background/HP_RealBar";
            public static readonly string HPText = "Life/HP_Background/HP_Text";
            
            public static readonly string LevelTr = "Level";
        }
    }
}
