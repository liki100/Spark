using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Spark.Loading.Core
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image _loadingImage;
        
        public async UniTask Run(Func<UniTask> action)
        {
            gameObject.SetActive(true);
            var loadingTween = Tween.Rotation(_loadingImage.rectTransform, new Vector3(0f, 0f, 360f), duration: 1f, cycles: -1);
            
            if (action != null)
                await action();

            loadingTween.Stop();
            gameObject.SetActive(false);
        }
    }
}