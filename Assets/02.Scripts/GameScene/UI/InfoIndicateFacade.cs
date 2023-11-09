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
        Transform _lifeTr;
        Transform _levelTr;

        //Use
        Image[] _lifeImages;
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
            _lifeTr = gameObject.GetHierachyPath<Transform>(Hierarchy.LifeTr);
            _levelTr = gameObject.GetHierachyPath<Transform>(Hierarchy.LevelTr);
        }


        public override void Initialize()
        {
            _audioService = _container.Resolve<AudioService>();

            _scoreText.text = "0";

            _lifeImages = new Image[Constants.lifeCount];

            for (int i = 0; i < _lifeTr.childCount; i++)
            {
                _lifeImages[i] = _lifeTr.GetChild(i).GetComponent<Image>();
            }

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

        public void IndicateLifeStatus(int order)
        {
            if (order >= 0)
            {
                //maybe can use Animation
                _lifeImages[order].gameObject.SetActive(false);
            }
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
                --_countNumber;
                _countText.text = _countNumber.ToString();
                _audioService.Play(AudioService.AudioResources.Count, AudioService.SoundType.SFX);
                await UniTask.Delay(System.TimeSpan.FromMilliseconds(1000));
            }

            _countdownPub.Publish(new CountDownComplete()
            {

            });

            Debug.Log($"CountDown Ended");

            _audioService.Play(AudioService.AudioResources.GameScene_1, AudioService.SoundType.BGM);

            _countText.gameObject.SetActive(false);
        }

        #endregion




        public static class Constants
        {
            public static readonly int countdown = 3;

            public static readonly int lifeCount = 3;

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
            public static readonly string LifeTr = "Life";
            public static readonly string LevelTr = "Level";
        }
    }
}
