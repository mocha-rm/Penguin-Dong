using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;
using VContainer.Unity;

using Cysharp.Threading.Tasks;
using UniRx;
using System;
using MessagePipe;
using GameScene.Message;

namespace GameScene.UI
{
    public class InteractionFacade : BaseFacade, IRegistMonobehavior
    {
        Button _directionBtn;
        IPublisher<DirectionButtonClick> _directionPub;

        Button _pausePanelOpenBtn;

        CompositeDisposable _disposable;


        #region GameOverPanel
        Transform _gameoverPanel;
        //Score
        TextMeshProUGUI _scoreText;
        Image _newRecordImg;
        //Level
        TextMeshProUGUI _levelText;
        //Coin
        TextMeshProUGUI _coinText;

        Button[] _gameOverBtns;
        IPublisher<SceneLoadEvent> _sceneloadPub;
        enum GameOverPanelBtn { Home, AdContinue }
        #endregion

        #region PausePanel
        Transform _pausePanel;
        Button[] _pauseBtns;
        enum PausePanelBtn
        {
            Continue,
            Home,
            Sound,
            Vibration
        }
        #endregion



        public void RegistBehavior(IContainerBuilder builder)
        {
            _directionBtn = gameObject.GetHierachyPath<Button>(Hierarchy.DirectionButton);

            _pausePanelOpenBtn = gameObject.GetHierachyPath<Button>(Hierarchy.PauseButton);
            _pausePanel = gameObject.GetHierachyPath<Transform>(Hierarchy.PausePanel);

            _gameoverPanel = gameObject.GetHierachyPath<Transform>(Hierarchy.GameOverPanel);
            _scoreText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.ScoreText);
            _newRecordImg = gameObject.GetHierachyPath<Image>(Hierarchy.NewRecordImage);
            _levelText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.LevelText);
            _coinText = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.CoinText);
        }

        public override void Initialize()
        {
            _disposable = new CompositeDisposable();

            _pauseBtns = new Button[Constants.PausePanelButtonCount];

            _gameOverBtns = new Button[Constants.GameOverPanelButtonCount];

            _directionPub = _container.Resolve<IPublisher<DirectionButtonClick>>();

            _sceneloadPub = _container.Resolve<IPublisher<SceneLoadEvent>>();

            for (int i = 0; i < _pauseBtns.Length; i++)
            {
                _pauseBtns[i] = _pausePanel.GetChild(i + 1).GetComponent<Button>();
            }

            for (int i = 0; i < _gameOverBtns.Length; i++)
            {
                _gameOverBtns[i] = _gameoverPanel.GetChild(Constants.GameOverPanelButtonsKeys[i]).GetComponent<Button>();
            }

            _directionBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _directionPub.Publish(new DirectionButtonClick()
                {
                    //Publish DirectionButtonClick Event Here ! 
                });
            }).AddTo(_disposable);

            _pausePanelOpenBtn.OnClickAsObservable().Subscribe(_ =>
            {
                _pausePanel.gameObject.SetActive(true);
                Time.timeScale = 0f;
                _pausePanelOpenBtn.interactable = false;
            }).AddTo(_disposable);

            _gameOverBtns[(int)GameOverPanelBtn.Home].OnClickAsObservable().Subscribe(_ =>
            {
                //go to lobby scene
                _sceneloadPub.Publish(new SceneLoadEvent()
                {
                    Scene = SceneName.LobbyScene
                });

            }).AddTo(_disposable);

            _gameOverBtns[(int)GameOverPanelBtn.AdContinue].OnClickAsObservable().Subscribe(_ =>
            {
                //play ad here
                _sceneloadPub.Publish(new SceneLoadEvent()
                {
                    Scene = SceneName.GameScene
                });

            }).AddTo(_disposable);

            PausePanelButtonsSetting();

            GameOverPanelButtonsSetting();
        }

        public override void Dispose()
        {
            _disposable?.Dispose();
            _disposable?.Clear();
            _disposable = null;
        }

        #region Public Methods
        public void ActivateGameOverPanel(bool isRecord)
        {
            _gameoverPanel.gameObject.SetActive(true);
            _pausePanelOpenBtn.interactable = false;

            //Show Elements
            if (isRecord)
            {
                //TODO : make DB and Save BestRecord and Compare it for this 
                _newRecordImg.gameObject.SetActive(true);
            }
        }
        #endregion


        #region Private Methods
        private void PausePanelButtonsSetting()
        {
            _pauseBtns[(int)PausePanelBtn.Continue].OnClickAsObservable().Subscribe(_ =>
            {
                //Game Resume
                _pausePanel.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                _pausePanelOpenBtn.interactable = true;
            }).AddTo(_disposable);

            _pauseBtns[(int)PausePanelBtn.Home].OnClickAsObservable().Subscribe(_ =>
            {
                //Home Scene Loading
            }).AddTo(_disposable);

            _pauseBtns[(int)PausePanelBtn.Sound].OnClickAsObservable().Subscribe(_ =>
            {
                //Sound On / Off
            }).AddTo(_disposable);

            _pauseBtns[(int)PausePanelBtn.Vibration].OnClickAsObservable().Subscribe(_ =>
            {
                //Vibration On / Off
            }).AddTo(_disposable);
        }

        private void GameOverPanelButtonsSetting()
        {
            _gameOverBtns[(int)GameOverPanelBtn.Home].OnClickAsObservable().Subscribe(_ =>
            {
                //Home Scene Loading
            }).AddTo(_disposable);

            _gameOverBtns[(int)GameOverPanelBtn.AdContinue].OnClickAsObservable().Subscribe(_ =>
            {
                //After Ad and Resume Game
            }).AddTo(_disposable);
        }

        private async UniTaskVoid ResultTask()
        {
            await UniTask.Yield();
        }

        #endregion


        public static class Constants
        {
            public static readonly int PausePanelButtonCount = 4;

            public static readonly int GameOverPanelButtonCount = 2;
            public static readonly int[] GameOverPanelButtonsKeys = { 4, 5 };
        }

        public static class Hierarchy
        {
            public static readonly string DirectionButton = "Btn_Direction";

            public static readonly string PauseButton = "Btn_Pause";
            public static readonly string PausePanel = "Pause";

            public static readonly string GameOverPanel = "GameOver";
            public static readonly string ScoreText = "GameOver/Tr_Score/Text_Score";
            public static readonly string NewRecordImage = "GameOver/Tr_Score/Img_NewRecord";
            public static readonly string LevelText = "GameOver/Tr_Level/Text_Level";
            public static readonly string CoinText = "GameOver/Tr_Coin/Text_Coin";
        }
    }
}
