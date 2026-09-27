using UnityEngine;
using UnityEngine.UI;

namespace Spark.TicTacToe.UI
{
    public class ScrollingBackground : MonoBehaviour
    {
        [SerializeField] private RawImage background;
        [SerializeField] private Vector2 speed = new(0.005f, 0.002f);

        private const float Aspect = 16f / 9f;

        private void Update()
        {
            var screenAspect = (float)Screen.width / Screen.height;

            var width = screenAspect < Aspect ? screenAspect / Aspect : 1f;
            var height = screenAspect > Aspect ? Aspect / screenAspect : 1f;

            var x = (1f - width) * 0.5f + Time.time * speed.x;
            var y = (1f - height) * 0.5f + Time.time * speed.y;

            background.uvRect = new Rect(x, y, width, height);
        }
    }
}