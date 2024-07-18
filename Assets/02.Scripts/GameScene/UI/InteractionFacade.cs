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
using TingleAvoid;
using TingleAvoid.AD;
using UnityEngine.SocialPlatforms.Impl;


namespace GameScene.UI
{
    public class InteractionFacade : BaseFacade, IRegistMonobehavior
    {
        BLOC _bloc;

        AdmobService _admob;

        AudioService _audioService;
        DBService _dbService;
        LoginService _loginService;
        Preferences _pref;
        

        Button _directionBtn;
        IPublisher<DirectionButtonClick> _directionPub;
        IPublisher<SceneLoadEvent> _sceneloadPub;
        IPublisher<GetRewardAndContinueEvent> _getRewardPub;

        Button _pausePanelOpenBtn;

        [SerializeField]GameObject _loadingPanel;

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
        Image _coinImg;
        TextMeshProUGUI _x2Img;

        Button[] _gameOverBtns;
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
            _coinImg = gameObject.GetHierachyPath<Image>(Hierarchy.CoinImage);
            _x2Img = gameObject.GetHierachyPath<TextMeshProUGUI>(Hierarchy.X2CointImage);
        }

        public override void Initialize()
        {
            _audioService = _container.Resolve<AudioService>();

            _dbService = _container.Resolve<DBService>();

            _loginService = _container.Resolve<LoginService>();

            _pref = _container.Resolve<Preferences>();

            _bloc = _container.Resolve<BLOC>();

            _admob = new AdmobService();
            _admob.Init();

            _disposable = new CompositeDisposable();

            _pauseBtns = new Button[Constants.PausePanelButtonCount];

            _gameOverBtns = new Button[Constants.GameOverPanelButtonCount];

            _directionPub = _container.Resolve<IPublisher<DirectionButtonClick>>();

            _sceneloadPub = _container.Resolve<IPublisher<SceneLoadEvent>>();

            _getRewardPub = _container.Resolve<IPublisher<GetRewardAndContinueEvent>>();

            for (int i = 0; i < _pauseBtns.Length; i++)
            {
                _pauseBtns[i] = _pausePanel.GetChild(i + 1).GetComponent<Button>();
            }

            for (int i = 0; i < _gameOverBtns.Length; i++)
            {
                _gameOverBtns[i] = _gameoverPanel.GetChild(Constants.GameOverPanelButtonsKeys[i]).GetComponent<Button>();
            }

            PausePanelButtonsSetting();

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
                _pref.CheckSoundStatus(_pauseBtns[(int)PausePanelBtn.Sound]);
                _pref.CheckVibrationStatus(_pauseBtns[(int)PausePanelBtn.Vibration]);
                Time.timeScale = 0f;
                _pausePanelOpenBtn.interactable = false;
            }).AddTo(_disposable);

            _gameOverBtns[(int)GameOverPanelBtn.Home].OnClickAsObservable().Subscribe(_ =>
            {
                _audioService.Stop(AudioService.SoundType.BGM);
                //Get Lastest UserInfo Here
                _dbService.GetUserData(_loginService.PLAYFABID);

                if (UnityEngine.Random.Range(0f, 1f) <= 0.4f)
                {
                    //Show Ad
                    var adActions = new AdActions
                    {
                        _type = AdType.FullScreen,

                        _onOpen = () =>
                        {
                            Debug.Log("Ad is opened");
                        },
                        _onClose = () =>
                        {
                            Debug.Log("Ad is closed");
                            // 전면 광고가 닫혔을 때 씬 이동
                            _loadingPanel.gameObject.SetActive(false);
                            _sceneloadPub.Publish(new SceneLoadEvent()
                            {
                                Scene = SceneName.LobbyScene
                            });
                        },
                        _onError = (error) =>
                        {
                            Debug.LogError("Ad failed with error: " + error);
                            _loadingPanel.gameObject.SetActive(false);
                            // 광고 로드 실패 시에도 씬 이동을 원하면 여기에 추가
                            _sceneloadPub.Publish(new SceneLoadEvent()
                            {
                                Scene = SceneName.LobbyScene
                            });
                        }
                    };

                    _loadingPanel.SetActive(true);
                    _admob.RequestAd(adActions);
                }
                else
                {
                    //Skip Ad
                    _sceneloadPub.Publish(new SceneLoadEvent()
                    {
                        Scene = SceneName.LobbyScene
                    });
                }                
            }).AddTo(_disposable); 

            _gameOverBtns[(int)GameOverPanelBtn.AdContinue].OnClickAsObservable().Subscribe(_ =>
            {
                _pausePanelOpenBtn.interactable = true;
                //부활 => 게임오버시 코인 2배로 획득
                var adActions = new AdActions
                {
                    _type = AdType.Reward,

                    _onReward = (reward) =>
                    {
                        Debug.Log($"Reward received: {reward.Amount} {reward.Type}");
                        // 리워드 처리 로직을 여기에 추가
                    },

                    _onOpen = () =>
                    {
                        Debug.Log("Ad is opened");
                    },
                    _onClose = () =>
                    {
                        Debug.Log("Ad is closed");
                        // 리워드 광고가 닫혔을 때 게임재시작
                        _loadingPanel.gameObject.SetActive(false);

                        _getRewardPub.Publish(new GetRewardAndContinueEvent()
                        {
                            isRewardGet = true
                        });
                       
                    },
                    _onError = (error) =>
                    {
                        Debug.LogError("Ad failed with error: " + error);
                        _loadingPanel.gameObject.SetActive(false);

                        _getRewardPub.Publish(new GetRewardAndContinueEvent()
                        {
                            isRewardGet = true
                        });
                    }
                };

                _gameoverPanel.gameObject.SetActive(false);
                _loadingPanel.SetActive(true);
                _admob.RequestAd(adActions);
            }).AddTo(_disposable);
        }

        public override void Dispose()
        {
            _disposable?.Dispose();
            _disposable?.Clear();
            _disposable = null;
            _admob.Clear();
        }

        #region Public Methods
        public void ActivateGameOverPanel(bool isRecord, int score, int level, int coin)
        {
            _gameoverPanel.gameObject.SetActive(true);
            _pausePanelOpenBtn.interactable = false;

            ResultTask(isRecord, score, level, coin).Forget();
            //여기서 리워드 받으면 코인 한번더 올려주는 로직을 작성해도 되고
        }

        public void NonactiveAdContinueButton()
        {
            _gameOverBtns[(int)GameOverPanelBtn.AdContinue].interactable = false;
        }

        public void ResetResultUIElements()
        {
            _scoreText.transform.parent.gameObject.SetActive(false);
            _scoreText.gameObject.SetActive(false);

            _levelText.transform.parent.gameObject.SetActive(false);
            _levelText.gameObject.SetActive(false);
            
            _newRecordImg.gameObject.SetActive(false);
            
            _coinText.transform.parent.gameObject.SetActive(false);
            _coinText.gameObject.SetActive(false);
            _coinImg.gameObject.SetActive(false);
            
                       
            foreach (Button btn in _gameOverBtns)
            {
                btn.gameObject.SetActive(false);
            }                                                                               
        }
        #endregion


        #region Private Methods
        private void PausePanelButtonsSetting()
        {
            _pauseBtns[(int)PausePanelBtn.Continue].OnClickAsObservable().Subscribe(_ =>
            {
                _pausePanel.gameObject.SetActive(false);
                Time.timeScale = 1.0f;
                _pausePanelOpenBtn.interactable = true;
            }).AddTo(_disposable);

            _pauseBtns[(int)PausePanelBtn.Home].OnClickAsObservable().Subscribe(_ =>
            {
                _pausePanel.gameObject.SetActive(false);

                Time.timeScale = 1.0f;
                _audioService.Stop(AudioService.SoundType.BGM);

                if (UnityEngine.Random.Range(0f, 1f) <= 0.2f)
                {
                    //Show Ad
                    var adActions = new AdActions
                    {
                        _type = AdType.FullScreen,

                        _onOpen = () =>
                        {
                            Debug.Log("Ad is opened");
                        },
                        _onClose = () =>
                        {
                            Debug.Log("Ad is closed");
                            _loadingPanel.gameObject.SetActive(false);
                            // 전면 광고가 닫혔을 때 씬 이동
                            _sceneloadPub.Publish(new SceneLoadEvent()
                            {
                                Scene = SceneName.LobbyScene
                            });
                        },
                        _onError = (error) =>
                        {
                            Debug.LogError("Ad failed with error: " + error);
                            _loadingPanel.gameObject.SetActive(false);
                            // 광고 로드 실패 시에도 씬 이동을 원하면 여기에 추가
                            _sceneloadPub.Publish(new SceneLoadEvent()
                            {
                                Scene = SceneName.LobbyScene
                            });
                        }
                    };
                    _loadingPanel.SetActive(true);
                    _admob.RequestAd(adActions);
                }
                else
                {
                    //Skip Ad
                    _sceneloadPub.Publish(new SceneLoadEvent()
                    {
                        Scene = SceneName.LobbyScene
                    });
                }
            }).AddTo(_disposable);

            _pauseBtns[(int)PausePanelBtn.Sound].OnClickAsObservable().Subscribe(_ =>
            {
                //Button Image Alpha Control Here
                _pref.SoundControl(_pauseBtns[(int)PausePanelBtn.Sound]);
                
            }).AddTo(_disposable);

            _pauseBtns[(int)PausePanelBtn.Vibration].OnClickAsObservable().Subscribe(_ =>
            {
                _pref.VibrateControl(_pauseBtns[(int)PausePanelBtn.Vibration]);
                
                //Vibration On / Off
            }).AddTo(_disposable);
        }

        private async UniTaskVoid ResultTask(bool isRecord, int score, int level, int coin)
        {
            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _scoreText.transform.parent.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _scoreText.gameObject.SetActive(true);
            await AnimateValue(_scoreText, score);

            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _levelText.transform.parent.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _levelText.gameObject.SetActive(true);
            await AnimateValue(_levelText, level);

            if (isRecord)
            {
                //TODO : make DB and Save BestRecord and Compare it for this
                _newRecordImg.gameObject.SetActive(true);
            }

            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _coinText.transform.parent.gameObject.SetActive(true);
            await UniTask.Delay(TimeSpan.FromMilliseconds(500));
            _coinImg.gameObject.SetActive(true);
            _coinText.gameObject.SetActive(true);

            if (_bloc.GameModel.GetRewardProperty.Value is true)
            {
                await AnimateValue(_coinText, coin/2);
                await UniTask.Delay(TimeSpan.FromMilliseconds(800));
                PresentDoubleCoinEffect(coin).Forget();
            }
            else
            {
                await AnimateValue(_coinText, coin);
            }

            await UniTask.Delay(TimeSpan.FromMilliseconds(500));

            foreach (Button btn in _gameOverBtns)
            {
                btn.gameObject.SetActive(true);
            }
        }
        private async UniTaskVoid PresentDoubleCoinEffect(int coin)
        {
            await AnimateValue(_coinText, coin, coin/2);
            _x2Img.gameObject.SetActive(true);
        }


        private async UniTask AnimateValue(TextMeshProUGUI textElement, int targetValue, int currentValue = 0)
        {
            int increment = 30; // Change this to control the speed of the count-up animation
            float delay = 0.01f; // Change this to control the delay between each increment

            while (currentValue < targetValue)
            {
                currentValue += increment;
                if (currentValue > targetValue)
                {
                    currentValue = targetValue;
                }
                textElement.text = currentValue.ToString();
                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
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
            public static readonly string CoinImage = "GameOver/Tr_Coin/Img_Coin";
            public static readonly string X2CointImage = "GameOver/Tr_Coin/Double";
        }
    }
}
