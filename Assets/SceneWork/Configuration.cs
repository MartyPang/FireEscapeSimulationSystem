using UnityEngine;
using System.Collections;
using Assets.Utils;
using UnityEngine.SceneManagement;
public class Configuration : MonoBehaviour
{

    public bool windowSwitch = false;
    private Rect windowRect = new Rect(Screen.width / 2 - 120, Screen.height / 2 - 150, 240, 300);
    Vector2 scrollPosition;
    //参数配置
    string Fire_Spread_Speed = "1.0";//火势蔓延速度
    string NumberOfPeople = "100";//人数
    string VIX_Index = "1.0";//初始恐慌指数
    string NumberOfExit = "1";//出口数

    public GUIStyle bb = new GUIStyle();
    public Texture2D img;
    // Use this for initialization
    void Start()
    {
        //scrollPosition = new Vector2(200, 50); 
    }
    void OnGUI()
    {
        GUIStyle bks = new GUIStyle();
        bks.fontSize = 30;
        string a = "";
        bks.normal.background = img;
        bb.fontSize = 33;
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), a, bks);
        GUI.Label(new Rect(Screen.width / 2 - 150, 100, 140, 40), "应急仿真疏散系统", bb);
        GUIStyle buttons = new GUIStyle();
        buttons.fontSize = 17;
        if (Config.SCENE == 0)
        {
            if (GUI.Button(new Rect(Screen.width / 2 - 230, Screen.height / 2, 100, 30), "更换为场景2"))
            {
                Config.SCENE = 1;
            }
        }
        else {
            if (GUI.Button(new Rect(Screen.width / 2 - 230, Screen.height / 2, 100, 30), "更换为场景1"))
            {
                Config.SCENE = 0;
            }
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 70, Screen.height / 2, 100, 30), "选择成为服务器"))
        {
            windowSwitch = true;
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 90, Screen.height / 2, 100, 30), "选择成为客户端"))
        {
            // Application.LoadLevel("4");
            if (Config.SCENE == 0)
                SceneManager.LoadScene("4");
            else if (Config.SCENE == 1)
                SceneManager.LoadScene("6");
            else
                Debug.Log("SceneLoad Error");
        }
        if (windowSwitch)
        {
            windowRect = GUI.Window(0, windowRect, WindowContain, "Configuration");
        }

    }
    public void WindowContain(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(200), GUILayout.Height(300));

        GUILayout.BeginHorizontal();
        GUILayout.Label("火焰蔓延速度: ");
        Fire_Spread_Speed = GUILayout.TextArea(Fire_Spread_Speed);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("人数: ");
        NumberOfPeople = GUILayout.TextArea(NumberOfPeople);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("初始恐慌指数: ");
        VIX_Index = GUILayout.TextArea(VIX_Index);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("出口数: ");
        NumberOfExit = GUILayout.TextArea(NumberOfExit);

        GUILayout.EndHorizontal();

        if (GUILayout.Button("保存配置"))
        {
            Config.FIRE_SPREAD_SPEED = float.Parse(Fire_Spread_Speed);
            Config.PERSON_NUM = int.Parse(NumberOfPeople);
            //对于恐慌因子，考虑本身输入因素70%的影响比例，火势因子30%的影响比例
            Config.SCARE_SPEED = float.Parse(VIX_Index) * 0.7f + float.Parse(Fire_Spread_Speed) * 0.3f;
            Config.NUM_OF_EXITS = int.Parse(NumberOfExit);
            windowSwitch = false;
            //Application.LoadLevel("3");
            if (Config.SCENE == 0)
                SceneManager.LoadScene("3");
            else if (Config.SCENE == 1)
                SceneManager.LoadScene("5");
            else
                Debug.Log("SceneLoad Error");
        }
        GUILayout.EndScrollView();

    }
    // Update is called once per frame
    void Update()
    {

    }
}
