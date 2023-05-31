using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using SFB;
using TMPro;
using UnityEngine.Video;


public class Stickman : MonoBehaviour
{
    public OpenFile openfile;
    public OpenVideo openvideo;

    public Camera camera;

    // a dictionary mapping joint names to game objects
    Dictionary<string, GameObject> joints = new Dictionary<string, GameObject>();
    Dictionary<string, GameObject> balls = new Dictionary<string, GameObject>();

    // the line renderer component used to draw the stickman's limbs
    private LineRenderer lineRenderer;

    bool isAnimationStarted;
    string[] lines;
    string[] first_line;

    VideoPlayer videoPlayer;
    bool videoStarted;

    bool play_clicked=false;

    private void Start()
    {
        openfile = GameObject.Find("OpenFile").GetComponent<OpenFile>();
        openvideo = GameObject.Find("OpenVideo").GetComponent<OpenVideo>();

        GameObject stickman = GameObject.Find("Stickman");
        GameObject hips = new GameObject("Hips");
        GameObject torso = new GameObject("Torso");
        GameObject leftHip = new GameObject("Left Hip");
        GameObject leftKnee = new GameObject("Left Knee");
        GameObject leftAnkle = new GameObject("Left Ankle");
        GameObject leftToe = new GameObject("Left Toe");
        GameObject rightHip = new GameObject("Right Hip");
        GameObject rightKnee = new GameObject("Right Knee");
        GameObject rightAnkle = new GameObject("Right Ankle");
        GameObject rightToe = new GameObject("Right Toe");
        GameObject rightFoot = new GameObject("Right Foot");
        GameObject leftFoot = new GameObject("Left Foot");

        camera = Instantiate(Camera.main);
        AudioListener audioListener=stickman.GetComponent<AudioListener>();
        if (audioListener != null)
        {
        Destroy(audioListener);
        }; 
        camera.transform.SetParent(stickman.transform, false);
        camera.transform.localPosition = new Vector3(0f, -0.5f, -2f);
        camera.rect = new Rect(0f, 0f, 0.5f, 1f);

        // populate the joints dictionary
        joints.Add("P_mtp_toes_r", rightToe);
        joints.Add("P_subt_calc_r", rightFoot);
        joints.Add("P_Ankle_tal_r", rightAnkle);
        joints.Add("P_KneeJ_r", rightKnee);
        joints.Add("P_HipJ_r", rightHip);
        joints.Add("P_Pelvis_C", hips);
        joints.Add("Pc_total", stickman);
        joints.Add("P_Torso", torso);
        joints.Add("P_HipJ_l", leftHip);
        joints.Add("P_KneeJ_l", leftKnee);
        joints.Add("P_Ankle_tal_l", leftAnkle);
        joints.Add("P_subt_calc_l", leftFoot);
        joints.Add("P_mtp_toes_l", leftToe);

        foreach (GameObject joint in joints.Values)
        {
        joint.transform.parent = stickman.transform;
        }
        
        // create the line renderer component and set its parameters
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        // Set the width of the line renderer points
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        // Set the material used by the line renderer
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.blue;
        lineRenderer.positionCount = 13;

        GameObject all_balls = new GameObject("all_balls");
        foreach(string key in joints.Keys)
        {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = key + "_Sphere";
        ball.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
        balls.Add(key, ball);
        }
        foreach (GameObject ball in balls.Values)
        {
        ball.transform.parent = all_balls.transform;
        }
        
        Application.targetFrameRate = Mathf.RoundToInt(1.0f / 0.0167f);

        //create canvas (this is essential for the buttom gameobject)
        GameObject canvasGO = new GameObject("Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        //create the button and attach to the canvas and choosing its position
        GameObject buttonGO = new GameObject("Play buttom");
        buttonGO.transform.SetParent(canvasGO.transform);
        RectTransform buttonRectTransform = buttonGO.AddComponent<RectTransform>();
        buttonRectTransform.anchorMin = new Vector2(0.5f, 0f);   // Set the anchor point to the bottom center
        buttonRectTransform.anchorMax = new Vector2(0.5f, 0f);   // Set the anchor point to the bottom center
        buttonRectTransform.anchoredPosition = new Vector2(-50f, 30f);   // Move the button up by 30 units from the bottom
        buttonRectTransform.sizeDelta = new Vector2(70f, 40f);   // Set the size of the button       

        //changing the color of the botton
        Image buttonImage = buttonGO.AddComponent<Image>();
        buttonImage.color = Color.white; // Set the desired background color

        //ading a text to the button (change the size and the text here)
        GameObject textGO = new GameObject("Play");
        textGO.transform.SetParent(buttonRectTransform);
        TextMeshProUGUI buttonText = textGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Play";
        buttonText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF");
        buttonText.fontStyle = FontStyles.Bold;
        buttonText.fontWeight = FontWeight.Bold;
        buttonText.fontSize = 20;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.black; 
        RectTransform textRectTransform = textGO.GetComponent<RectTransform>();
        textRectTransform.sizeDelta = buttonRectTransform.sizeDelta;
        textRectTransform.localPosition = Vector2.zero;

        //this code attach clicking the buttom to open the panel
        Button selectCSVButton = buttonGO.AddComponent<Button>();
        selectCSVButton.onClick.AddListener(playFunction);
        void playFunction()
        {
            play_clicked=true;
        }

        //create the button and attach to the canvas and choosing its position
        GameObject buttonGO1 = new GameObject("Pause button");
        buttonGO1.transform.SetParent(canvasGO.transform);
        RectTransform buttonRectTransform1 = buttonGO1.AddComponent<RectTransform>();
        buttonRectTransform1.anchorMin = new Vector2(0.5f, 0f);   // Set the anchor point to the bottom center
        buttonRectTransform1.anchorMax = new Vector2(0.5f, 0f);   // Set the anchor point to the bottom center
        buttonRectTransform1.anchoredPosition = new Vector2(50f, 30f);   // Move the button up by 30 units from the bottom
        buttonRectTransform1.sizeDelta = new Vector2(70f, 40f);   // Set the size of the button  

        //changing the color of the botton
        Image buttonImage1 = buttonGO1.AddComponent<Image>();
        buttonImage1.color = Color.white; // Set the desired background color

        //ading a text to the button (change the size and the text here)
        GameObject textGO1 = new GameObject("Pause");
        textGO1.transform.SetParent(buttonRectTransform1);
        TextMeshProUGUI buttonText1 = textGO1.AddComponent<TextMeshProUGUI>();
        buttonText1.text = "Pause";
        buttonText1.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF");
        buttonText1.fontStyle = FontStyles.Bold;
        buttonText1.fontWeight = FontWeight.Bold;
        buttonText1.fontSize = 20;
        buttonText1.alignment = TextAlignmentOptions.Center;
        buttonText1.color = Color.black; 
        RectTransform textRectTransform1 = textGO1.GetComponent<RectTransform>();
        textRectTransform1.sizeDelta = buttonRectTransform1.sizeDelta;
        textRectTransform1.localPosition = Vector2.zero;

        //this code attach clicking the buttom to open the panel
        Button selectCSVButton1 = buttonGO1.AddComponent<Button>();
        selectCSVButton1.onClick.AddListener(PauseFunction);
        void PauseFunction()
        {
            play_clicked=false;
        }

        //create the button and attach to the canvas and choosing its position
        GameObject buttonGO2 = new GameObject("quit button");
        buttonGO2.transform.SetParent(canvasGO.transform);
        RectTransform buttonRectTransform2 = buttonGO2.AddComponent<RectTransform>();
        buttonRectTransform2.anchorMin = new Vector2(1f, 1f);
        buttonRectTransform2.anchorMax = new Vector2(1f, 1f);
        buttonRectTransform2.anchoredPosition = new Vector2(-30f, -20f);
        buttonRectTransform2.sizeDelta = new Vector2(50f, 30f); 

        //changing the color of the botton
        Image buttonImage2 = buttonGO2.AddComponent<Image>();
        buttonImage2.color = Color.white; // Set the desired background color

        //ading a text to the button (change the size and the text here)
        GameObject textGO2 = new GameObject("Quit");
        textGO2.transform.SetParent(buttonRectTransform2);
        TextMeshProUGUI buttonText2 = textGO2.AddComponent<TextMeshProUGUI>();
        buttonText2.text = "Quit";
        buttonText2.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/Arial SDF");
        buttonText2.fontStyle = FontStyles.Bold;
        buttonText2.fontWeight = FontWeight.Bold;
        buttonText2.fontSize = 20;
        buttonText2.alignment = TextAlignmentOptions.Center;
        buttonText2.color = Color.black; 
        RectTransform textRectTransform2 = textGO2.GetComponent<RectTransform>();
        textRectTransform2.sizeDelta = buttonRectTransform2.sizeDelta;
        textRectTransform2.localPosition = Vector2.zero;

        //this code attach clicking the buttom to open the panel
        Button selectCSVButton2 = buttonGO2.AddComponent<Button>();
        selectCSVButton2.onClick.AddListener(QuitFunction);
        void QuitFunction()
        {
           Application.Quit();
        }

    }

    private float timeElapsed= 0.0f; // track the time elapsed since the animation started
    private int currentIndex = 1; // keep track of the current row in the CSV file

    private void Update()
    {
        //this because the values change in the OpenFile script
        isAnimationStarted = openfile.isAnimationStarted;
        lines = openfile.lines;
        first_line = openfile.first_line;

        videoPlayer=openvideo.videoPlayer;
        videoStarted=openvideo.videoStarted;

        if (isAnimationStarted==true&& videoStarted==true && play_clicked==true)
        {
            // update the time elapsed
            timeElapsed += Time.deltaTime;

            // check if there are still lines left in the CSV file
            if (currentIndex < lines.Length-1)
            {
                // get the timestamp of the current line and convert it to seconds
                float timestamp = float.Parse(lines[currentIndex].Split(',')[0]);
                timestamp /= 1000.0f; // assuming the timestamp is in milliseconds

                // if the current time elapsed is greater than or equal to the timestamp of the current line
                if (timeElapsed >= timestamp)
                {
                // darw stickman from the first line data
                string[] values = lines[currentIndex].Split(',');
                for (int i = 1; i <=37; i=i+3)
                {
                    float x = float.Parse(values[i]);
                    float y = float.Parse(values[i+1]);
                    float z = float.Parse(values[i+2]);
                    string jointName = first_line[i];

                    // update the position of the current joint
                    joints[jointName].transform.position = new Vector3(x, y, z);
                    balls[jointName].transform.position =new Vector3(x, y, z);
                    
                }
                if (!videoPlayer.isPlaying)
                        videoPlayer.Play();
                lineRenderer.SetPosition(0,joints["P_mtp_toes_r"].transform.position);
                lineRenderer.SetPosition(1, joints["P_subt_calc_r"].transform.position);
                lineRenderer.SetPosition(2, joints["P_Ankle_tal_r"].transform.position);
                lineRenderer.SetPosition(3, joints["P_KneeJ_r"].transform.position);
                lineRenderer.SetPosition(4, joints["P_HipJ_r"].transform.position);
                lineRenderer.SetPosition(5, joints["P_Pelvis_C"].transform.position);
                lineRenderer.SetPosition(6, joints["Pc_total"].transform.position);
                lineRenderer.SetPosition(7, joints["P_Torso"].transform.position);
                lineRenderer.SetPosition(8, joints["P_HipJ_l"].transform.position);
                lineRenderer.SetPosition(9, joints["P_KneeJ_l"].transform.position);
                lineRenderer.SetPosition(10, joints["P_Ankle_tal_l"].transform.position);
                lineRenderer.SetPosition(11, joints["P_subt_calc_l"].transform.position);
                lineRenderer.SetPosition(12, joints["P_mtp_toes_l"].transform.position);

                // increment the current index to move to the next line in the CSV file
                currentIndex++;

                }
            }
        }
        if(isAnimationStarted==true&& videoStarted==true && play_clicked==false)
        {
             videoPlayer.Pause();
        }

    }

}