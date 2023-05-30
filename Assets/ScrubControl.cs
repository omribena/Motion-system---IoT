using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScrubControl : MonoBehaviour
{
    public OpenFile openfile;
    public OpenVideo openvideo;

    public GameObject stickman;
    public VideoPlayer videoPlayer;

    private bool isScrubbing = false;

    Slider slider;

    private void Start()
    {
        openfile = GameObject.Find("OpenFile").GetComponent<OpenFile>();
        openvideo = GameObject.Find("OpenVideo").GetComponent<OpenVideo>();

        stickman = GameObject.Find("Stickman");

        // Create a new slider GameObject
        GameObject sliderGO = new GameObject("ScrubSlider");
        sliderGO.transform.SetParent(transform); // Set the ScrubControl script as the parent of the slider

        // Add the necessary components to the slider GameObject
        slider = sliderGO.AddComponent<Slider>();
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        // Adjust the position of the slider
        sliderRect.anchorMin = new Vector2(0f, 0f);
        sliderRect.anchorMax = new Vector2(1f, 0f);
        sliderRect.pivot = new Vector2(0.5f, 0f);
        sliderRect.anchoredPosition = new Vector2(0f, 0f); // Set the desired position of the slider
        sliderRect.localPosition = new Vector3(0f, 0f, 0f);


        // Add a handle and fill to the slider
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(sliderRect);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        handleRect.anchorMin = new Vector2(0.5f, 0f);
        handleRect.anchorMax = new Vector2(0.5f, 0f);
        handleRect.pivot = new Vector2(0.5f, 0f);
        handleRect.anchoredPosition = new Vector2(0f, 0f); // Set the desired position of the handle


        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(sliderRect);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = Color.blue;
        // Adjust the position of the fill
        fillRect.anchorMin = new Vector2(0f, 0f);
        fillRect.anchorMax = new Vector2(0f, 0f);
        fillRect.pivot = new Vector2(0f, 0f);
        fillRect.anchoredPosition = new Vector2(0f, 0f);
    }

    private void Update()
    {
        if (openvideo.videoStarted && openfile.isAnimationStarted && videoPlayer == null)
        {
            videoPlayer = openvideo.videoPlayer;
            slider.minValue = 0f;
            slider.maxValue = (float)videoPlayer.length; // Set the maximum value to the length of the video
            slider.wholeNumbers = false; // Allow fractional values for precise scrubbing
            slider.onValueChanged.AddListener(Scrub);
        }

        if (videoPlayer != null && !isScrubbing)
        {
            float playheadPosition = GetPlayheadPosition();
            slider.value = playheadPosition;
        }
    }

    public void Scrub(float value)
    {
        float totalTime = GetTotalTime();
        float targetTime = value * totalTime;

        // Update stickman's joint positions based on the target time
        Stickman stickmanScript = stickman.GetComponent<Stickman>();
        //stickmanScript.UpdateJointPositions(targetTime);

        // Scrub video
        videoPlayer.time = targetTime;

        if (!isScrubbing)
        {
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
