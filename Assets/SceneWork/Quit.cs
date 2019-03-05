using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class Quit : MonoBehaviour
{

    public bool windowSwitch = false;
    private Rect windowRect;
    private Vector2 scrollPosition;
    // Use this for initialization
    void Start()
    {
        windowRect = new Rect(0, 200, 200, 200);
    }
    void OnGUI()
    {

        GUILayout.Space(150);
        if (GUILayout.Button("Exit"))
        {
            Application.Quit();
        }
        if (GUILayout.Button("Return"))
        {
            SceneManager.LoadScene("conf");
        }
        if (windowSwitch)
        {
            windowRect = GUILayout.Window(0, windowRect, WindowContain, "Configuration");
        }

    }   // Update is called once per frame
    void Update()
    {

    }
    void WindowContain(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(250), GUILayout.Height(200));
        int i = PlayerPrefs.GetInt("OffSet", 0);
        GUILayout.BeginHorizontal();
        GUILayout.Label("开始时间 " + PlayerPrefs.GetFloat("StartTime" + i));
        GUILayout.Label("        结束时间 " + PlayerPrefs.GetFloat("StopTime" + i));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("NPC数量 " + PlayerPrefs.GetInt("AllNPC" + i));
        GUILayout.Label("    逃脱NPC数量 " + PlayerPrefs.GetInt("EscapedNPC" + i));
        GUILayout.EndHorizontal();



        GUILayout.BeginHorizontal();
        GUILayout.Label("死亡NPC数量 " + PlayerPrefs.GetInt("DiedNPC" + i));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("接入客户端数量 " + PlayerPrefs.GetInt("AllPerson" + i));
        GUILayout.Label("逃脱客户端数量 " + PlayerPrefs.GetInt("EscapedPerson" + i));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("死亡客户端数量 " + PlayerPrefs.GetInt("DiedPerson" + i));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("确定"))
        {
            windowSwitch = false;
        }

        GUILayout.EndScrollView();
    }
}
