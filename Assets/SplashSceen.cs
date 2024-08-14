using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;




public class SplashSceen : MonoBehaviour
{
    public Image _backGroundImg; // 커스텀 스플래쉬 이미지를 설정하기 위한 필드
    public Image _splahImg;

    public double splashDuration = 2000; // 커스텀 스플래쉬 이미지 표시 시간
    public float fadeDuration = 1.0f;




    void Awake()
    {
        _backGroundImg.gameObject.SetActive(false);
    }

   
    public async UniTask ShowCustomSplash()
    {
        // Unity 스플래쉬 화면이 끝날 때까지 대기
        await UniTask.Delay(TimeSpan.FromMilliseconds(2000));

        // 커스텀 스플래쉬 이미지를 활성화
        _backGroundImg.gameObject.SetActive(true);

        // 지정된 시간 동안 대기
        await UniTask.Delay(TimeSpan.FromMilliseconds(splashDuration));

        FadeOut().Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(fadeDuration)); 
    }

    private async UniTaskVoid FadeOut()
    {
        float startAlpha = 1.0f;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        Color fcolor;

        while (progress < 1.0f)
        {
            fcolor = new Color(1f,1f,1f,Mathf.Lerp(startAlpha, 0, progress));
            progress += rate * Time.deltaTime;

            _splahImg.color = fcolor;

            await UniTask.Yield();
        }

        _splahImg.color = new Color(1f, 1f, 1f, 0f);
    }
}
