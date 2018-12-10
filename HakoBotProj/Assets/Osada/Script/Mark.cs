using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class Mark : MonoBehaviour
{
    private Image img;
    private float a = 1.0f;

    private void Start()
    {
        img = GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, a);
        StartCoroutine("FadeOut");

        Observable.Timer(System.TimeSpan.FromSeconds(3.0f))
            .Subscribe(_ =>
            {
                Destroy(gameObject);
            }).AddTo(this);
    }

    /// <summary>
    /// Imageを徐々に不透明にしていく
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn()
    {
        while (true)
        {
            a += 5 * Time.deltaTime;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);

            if (a >= 1)
                break;
            yield return null;
        }
        StartCoroutine("FadeOut");
    }
    /// <summary>
    /// Imageを徐々に透明にしていく
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut()
    {
        while (true)
        {
            a -= 5 * Time.deltaTime;
            img.color = new Color(img.color.r, img.color.g, img.color.b, a);

            if (a <= 0)
                break;
            yield return null;
        }
        StartCoroutine("FadeIn");
    }
}
