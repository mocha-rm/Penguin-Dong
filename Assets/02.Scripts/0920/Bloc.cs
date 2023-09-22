using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class Bloc : IInitializable
{
    //Add Models Here
    //public Rule.IGameModel GameModel {get; private set;}

    //public Score.IScoreModel ScoreModel {get; private set;}

    //what we have to indicate on UI ? Life, Score, Level


    public void Initialize()
    {
        //Initialize Model Here
        //normally bloc pattern is for seperate presentation Layer and businessLogic
        //It `s just for presenting (UI Parts)? 
    }
}
