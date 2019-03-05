using UnityEngine;
using Assets.Utils;
using System.Collections;
using System;
public class draw : MonoBehaviour {

    private Color c1 = Color.red;
    private Color c2 = Color.red;
    private LineRenderer lineRenderer;

    private Vector3 []v = new Vector3[10];
    private int MultipleX;
    private int MultipleY;
    public Camera camera;
    private int coordOffset=10;
    void Start()
    {
        lineRenderer = lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetColors(c1, c2);
        lineRenderer.SetWidth(0.2f, 0.2f);
        lineRenderer.SetVertexCount(10);
        float x, y,t1,t2,p;
        int s;
        for(int i = 0; i < 10; i++)
        {
           // x = PlayerPrefs.GetFloat("Strange" + i, 0);
            x = PlayerPrefs.GetInt("AllNPC" + i, 0);
            t1 = PlayerPrefs.GetFloat("StartTime" + i, 0);
            t2 = PlayerPrefs.GetFloat("StopTime" + i, 0);
            p = PlayerPrefs.GetInt("AllNPC" + i, 0) - PlayerPrefs.GetInt("EscapedNPC" + i, 0) - PlayerPrefs.GetInt("DiedNPC" + i, 0);
            s = PlayerPrefs.GetInt("Scene" + i, 0);
            y = t2 - t1;
            if (p == 0 && s == Config.SCENE)
                v[i] = new Vector3(x, y, 0);
            else
                v[i] = new Vector3(0, 0, 0);
            Debug.Log(x);
        }
        
/*        for(int i = 0; i < 10; i++)
        {

        }
*/                  //在此循环中加入v[i]的数值
        Qsort(v, 0, 9);
        float Xtmp = v[9].x;
        float Ytmp = v[0].y;
      
        for (int a = 0; a < 10; a++)
        {
            if (v[a].y > Ytmp)
                Ytmp = v[a].y;
        }
        MultipleX = 1;MultipleY = 1;
        while (Xtmp>MultipleX*10)
        {
            MultipleX += 1;
        }
        while (Ytmp>MultipleY * 8)
        {
            MultipleY += 1;
        }
        //Debug.Log(MultipleX + " " + MultipleY);
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(v[i].x + " " + v[i].y);
            v[i].x = v[i].x / MultipleX * 2;
            v[i].y = v[i].y / MultipleY * 2;
            Debug.Log(v[i].x + " " + v[i].y);

        }
        //    Debug.Log(MultipleX + " " + MultipleY);
    
    }

    void Update()
    {
        for (int i=0;i<10;i++)
            lineRenderer.SetPosition(i, v[i]);
    }
    // isY代表是不是Y轴 
    void drawCoordinate(Vector3 point, string name, bool isY)
    {
        // 将世界坐标转换为屏幕坐标 
           Vector2 position = camera.WorldToScreenPoint(point);
           position = new Vector2(position.x, position.y);

        // 设置刻度的大小和颜色 
        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(name));
        GUI.color = Color.yellow;
        // 根据X,Y轴的不同加上适当偏移量画出刻度 
        if (isY)
        {
            GUI.Label(new Rect(position.x - nameSize.x - coordOffset, Screen.height - position.y, nameSize.x, nameSize.y), name);
        }
        else {
            GUI.Label(new Rect(position.x - nameSize.x, Screen.height - position.y, nameSize.x, nameSize.y), name);
        }
    }
    void OnGUI()
    {
        int tmp;
        for(int i = 0; i <= 8; i++)
        {
            tmp = i * MultipleY;
            drawCoordinate(new Vector3(0, i*2, 0), tmp.ToString(), true);
        }
        for (int i = 0; i <= 10; i++)
        {
            tmp = i * MultipleX;
            drawCoordinate(new Vector3(i*2, 0, 0), tmp.ToString(), true);
        }
        drawCoordinate(new Vector3(0, 17, 0), "时间/s", true);
        drawCoordinate(new Vector3(23, 0, 0), "人数", false);
    }
    void Qsort(Vector3 []a, int low, int high)
    {
        if (low >= high)
        {
            return;
        }
        int first = low;
        int last = high;
        Vector3 key = a[first];/*用字表的第一个记录作为枢轴*/

        while (first < last)
        {
            while (first < last && a[last].x >= key.x)
            {
                --last;
            }

            a[first] = a[last];/*将比第一个小的移到低端*/

            while (first < last && a[first].x <= key.x)
            {
                ++first;
            }

            a[last] = a[first];
            /*将比第一个大的移到高端*/
        }
        a[first] = key;/*枢轴记录到位*/
        Qsort(a, low, first - 1);
        Qsort(a, first + 1, high);
    }
}
