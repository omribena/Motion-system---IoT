using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SFB;
using TMPro;
using UnityEngine.Video;

public class OpenVideo : MonoBehaviour
{
    public bool videoStarted = false;
    public VideoPlayer videoPlayer;

    void Start()
    {
        //create canvas (this is essential for the buttom gameobject)
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        canvasGO.transform.SetParent(GameObject.Find("OpenVideo").transform);

        //create the button and attach to the canvas and choosing its position
        GameObject buttonGO = new GameObject("Select video");
        buttonGO.transform.SetParent(canvasGO.transform);
        RectTransform buttonRectTransform = buttonGO.AddComponent<RectTransform>();
        buttonRectTransform.anchorMin = new Vector2(1f, 1f);
        buttonRectTransform.anchorMax = new Vector2(1f, 1f);
        buttonRectTransform.anchoredPosition = new Vector2(-150f, -30f);
        buttonRectTransform.sizeDelta = new Vector2(70f, 40f);

        //changing the color of the botton
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = Color.white; // Set the desired background color

        //ading a text to the button (change the size and the text here)
        GameObject textGO = new GameObject("Button Text");
        textGO.transform.SetParent(buttonRectTransform);
        TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Select video";
        buttonText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF");
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.fontWeight = FontWeight.Bold;
        buttonText.fontSize = 18;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.black; 
        RectTransform textRectTransform = textGO.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = buttonRectTransform.sizeDelta;
        textRectTransform.localPosition = Vector2.zero;

        GameObject videoObject = new GameObject("VideoPlayer");
        videoPlayer = videoObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;

        Camera camera = Instantiate(Camera.main);
        camera.transform.SetParent(videoPlayer.transform);
        videoPlayer.targetCamera = camera;
        camera.transform.position = new Vector3(0f,0f,0f);
        camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);      

        //this code attach clicking the buttom to open the panel
        Button selectCSVButton = buttonGO.AddComponent<Button>();
        selectCSVButton.onClick.AddListener(OpenFileDialog);

        //open the panel
        void OpenFileDialog()
        {
            string[] paths = StandaloneFileBrowser.OpenFilePanel("Select video", "", "mp4", false);
            if (paths.Length > 0)
            {
                string videoPath = paths[0];
                videoPlayer.url = videoPath;
                videoStarted=true;
            }
        }

    }

    void Update()
    {
        
    }
}