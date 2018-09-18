using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

public class UIAnimDisplay : MonoBehaviour
{
    public Text animationText;
    public int currentAnimClipInt;
    public Animator[] animatorsToMonitor;
    public string animatorIntName;
    private AnimationClip[] clips;
    // Use this for initialization
    void Start()
    {
        if (animationText == null || animatorsToMonitor == null)
        {
            Destroy(this);
        }
        else
        {
            currentAnimClipInt = animatorsToMonitor[0].GetInteger(animatorIntName);
        }
        clips = AnimationUtility.GetAnimationClips(animatorsToMonitor[0].gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!animatorsToMonitor[0].IsInTransition(0))
        {
            animationText.text = clips[currentAnimClipInt].name;
        }
    }

    public void OnClickIncreaseInt()
    {
        if (currentAnimClipInt + 1 < clips.Length)
        {
            currentAnimClipInt += 1;
            for (int i = 0; i < animatorsToMonitor.Length; ++i)
            {
                animatorsToMonitor[i].SetInteger(animatorIntName, currentAnimClipInt);
            }
        }
    }

    public void OnClickDecreaseInt()
    {
        if (currentAnimClipInt - 1 >= 0)
        {
            currentAnimClipInt -= 1;
            for (int i = 0; i < animatorsToMonitor.Length; ++i)
            {
                animatorsToMonitor[i].SetInteger(animatorIntName, currentAnimClipInt);
            }
        }
    }
}

