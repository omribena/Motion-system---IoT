using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScrubControl : MonoBehaviour
{
    public GameObject stickman;
    public VideoPlayer videoPlayer;
    private bool isScrubbing = false;

    private void Start()
    {
        stickman = GameObject.Find("Stickman");
        videoPlayer = GameObject.Find("videoPlayer").GetComponent<VideoPlayer>();      
        Slider slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(Scrub);
    }

    private void Update()
    {
        if (!isScrubbing)
        {
            float playheadPosition = GetPlayheadPosition();
            Slider slider = GetComponent<Slider>();
            slider.value = playheadPosition;
        }
    }

    private void Scrub(float value)
    {
        float totalTime = GetTotalTime();
        float targetTime = value * totalTime;

        // Scrub animation
        Animation animation = stickman.GetComponent<Animation>();
        foreach (AnimationState state in animation)
        {
            state.time = targetTime;
        }
        animation.Sample();

        // Scrub video
        videoPlayer.time = targetTime;

        if (!isScrubbing)
        {
            animation.Play();
            videoPlayer.Play();
            isScrubbing = true;
        }
    }

    private float GetPlayheadPosition()
    {
        float totalTime = GetTotalTime();
        float currentTime = GetCurrentTime();
        return currentTime / totalTime;
    }

    private float GetCurrentTime()
    {
        return (float)videoPlayer.time;
    }

    private float GetTotalTime()
    {
        return (float)videoPlayer.length;
    }

    public void StartScrubbing()
    {
        isScrubbing = true;
    }

    public void StopScrubbing()
    {
        isScrubbing = false;
    }
}
