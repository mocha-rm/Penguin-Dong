using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

using GameScene.Player;
using GameScene.Obstacle;
using GameScene.UI;

using GameScene.Rule;


//Add Models Here
//public Rule.IGameModel GameModel {get; private set;}

//public Score.IScoreModel ScoreModel {get; private set;}

//what we have to indicate on UI ? Life, Score, Level

namespace GameScene
{
    public class BLOC : IInitializable
    {
        [Inject] IObjectResolver _container;

        public IGameModel GameModel { get; private set; }

        public IPlayerModel PlayerModel { get; private set; }



        public void Initialize()
        {
            //Initialize Model Here
            //normally bloc pattern is for seperate presentation Layer and businessLogic
            //It `s just for presenting (UI Parts)? 

            GameModel = _container.Resolve<GameRule>().Model;

            PlayerModel = _container.Resolve<PlayerFacade>().Model;
        }
    }
}
