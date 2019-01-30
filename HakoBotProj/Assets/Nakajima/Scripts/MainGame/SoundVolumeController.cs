using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundVolumeController : MonoBehaviour
{ 
    // BGMのAudioSource
    AudioSource bgmAudio;

    // 音量変更用Slider
    [SerializeField]
    Slider bgmSlider;
    [SerializeField]
    Slider seSlider;

    // 音量調整用Text
    [SerializeField]
    Text[] volumeType;
    
    // 現在の音量
    float[] volume = new float[2];

    // 変更する音量の要素番号
    int state = 0;

    // Use this for initialization
    void Start () {
        bgmAudio = AudioController.Instance.myAudio[0];

        volume[0] = bgmSlider.value;
        volume[1] = seSlider.value;
    }
	
	// Update is called once per frame
	void Update () {
        VolumeChange();
	}

    /// <summary>
    /// 音量調整
    /// </summary>
    public void VolumeChange()
    {
        // 入力
        for(int num = 0;num < 4; num++) {
            volumeType[0].color = new Color(255, 255, 255);
            volumeType[1].color = new Color(255, 255, 255);

            // どっちの音量変更するか決定
            if (PlayerSystem.Instance.LeftStickAxis(num).y > 0) state--;
            else if (PlayerSystem.Instance.LeftStickAxis(num).y < 0) state++;

            // 要素数をはみ出さないよう調整
            if (state > 1) state = 1;
            else if (state < 0) state = 0;

            // 現在変更中の音量を指定
            volumeType[state].color = new Color(255, 0, 0);

            // 音量変更
            if (PlayerSystem.Instance.Button_RightShoulder(num)) volume[state] += 0.05f;
            else if (PlayerSystem.Instance.Button_LeftShoulder(num)) volume[state] -= 0.05f;
        }

        // Sliderに反映
        bgmSlider.value = volume[0];
        seSlider.value = volume[1];

        // BGMの音量調整
        bgmAudio.volume = bgmSlider.value;

        // SEのAudioSourceをリスト化
        var audioVolume = MainManager.Instance.GetAudioSource().Where(obj => obj != bgmAudio).ToList();

        // SEの音量調整
        foreach(AudioSource _audio in audioVolume) {
            _audio.volume = seSlider.value;
            // 音量が0でないなら消音ではない
            if(_audio.volume != 0.0f && AudioController.Instance.volumeZero == true) AudioController.Instance.volumeZero = false;
            // 音量が0なら消音
            if (_audio.volume == 0.0f && AudioController.Instance.volumeZero == false) AudioController.Instance.volumeZero = true;
        }
    }
}
