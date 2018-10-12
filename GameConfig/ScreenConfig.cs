using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml;

public class ScreenConfig : MonoBehaviour
{
    [DllImport("user32")]
    static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
    [DllImport("user32")]
    static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
    [DllImport("user32")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32")]
    static extern IntPtr GetActiveWindow();
    [DllImport("user32")]
    static extern IntPtr SetActiveWindow(IntPtr hWnd);
    [DllImport("user32")]
    static extern bool GetWindowRect(IntPtr hWnd, ref Rect rect);
    [DllImport("user32")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32")]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);
    [DllImport("user32")]
    static extern int GetSystemMetrics(int nIndex);
    [DllImport("user32")]
    static extern int SetForegroundWindow(IntPtr hwnd);
    [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
    static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
    [DllImport("user32")]
    static extern bool SetMenu(IntPtr hWnd, IntPtr hMenu);

    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;
    const int WS_POPUP = 0x800000;
    const int WS_SYSMENU = 0x80000;
    
    const int SWP_NOSIZE = 0x0001;
    const int SWP_NOMOVE = 0x0002;
    const uint SW_SHOWNORMAL = 1;
    const int HWND_NOTOPMOST = 0xffffffe;

    const int WS_CAPTION = (int)0x00C00000;
    const int WS_CHILD = (int)0x40000000;

    const uint SWP_SHOWWINDOW = 0x0040;
    const uint SWP_DRAWFRAME = 0x0020;
    const uint SWP_DEFERERASE = 0x2000;
    const uint SWP_FRAMECHANGED = 0x0020;
    const uint SWP_NOZORDER = 0x0004;
    const int HWND_TOP = 0;
    const int HWND_TOPMOST = -1;

    static bool IsChangePos = false;
    void ChangeWindowPos()
    {
        if (IsChangePos)
        {
            return;
        }
        IsChangePos = true;
        
        Screen.SetResolution(1280, 720, true);
        StartCoroutine(SetGameWindowInfo());
    }

    /// <summary>
    /// 设置窗口风格.
    /// </summary>
    IEnumerator SetGameWindowInfo()
    {
        yield return new WaitForSeconds(0.5f);
        Screen.SetResolution(1280, 720, false);
        StartCoroutine(fixWindowPos(0f)); //move the game to child screen
        StartCoroutine(fixWindowPos(1f)); //make the game full screen
        StartCoroutine(fixWindowPos(1.2f)); //make the game full screen
    }
    
    void FullScreenViewCtrl()
    {
        IntPtr m_hWnd = GetForegroundWindow();
        int dwWindowStyleSave = GetWindowLong(m_hWnd, GWL_STYLE); //保存窗口风格
        SetWindowLong(m_hWnd, GWL_STYLE,
                      dwWindowStyleSave & (~WS_CHILD) & (~WS_CAPTION) & (~WS_BORDER));//使窗口不具有CAPTION风格
        
        int x = m_ScreenData.px;
        int y = m_ScreenData.py;
        int cx = m_ScreenData.cx;
        int cy = m_ScreenData.cy;
        uint SWP = SWP_SHOWWINDOW;
        SetWindowPos(m_hWnd, HWND_TOPMOST, x, y, cx, cy, SWP); //修改窗口置全屏
    }

    IEnumerator fixWindowPos(float time)
    {
        yield return new WaitForSeconds(time);
        FullScreenViewCtrl();
    }

    // Use this for initialization
    void Awake()
    {
        InitInfo();
#if !UNITY_EDITOR
        ChangeWindowPos();
#endif
    }

    public class ScreenData
    {
        internal int px = 0;
        internal int py = 0;
        internal int cx = 0;
        internal int cy = 0;
    }
    internal ScreenData m_ScreenData = new ScreenData();

    void InitInfo()
    {
#if UNITY_STANDALONE_WIN
        string filePath = Application.dataPath + "/../config";
        if (!Directory.Exists(filePath))
        {
            Directory.CreateDirectory(filePath);
        }
#endif

        string fileName = "../config/ScreenConfig.ini";
        string readInfo = ReadFromFileXml(fileName, "PX");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "0";
            WriteToFileXml(fileName, "PX", readInfo);
        }
        m_ScreenData.px = Convert.ToInt32(readInfo);

        readInfo = ReadFromFileXml(fileName, "PY");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "0";
            WriteToFileXml(fileName, "PY", readInfo);
        }
        m_ScreenData.py = Convert.ToInt32(readInfo);

        readInfo = ReadFromFileXml(fileName, "CX");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "1280";
            WriteToFileXml(fileName, "CX", readInfo);
        }
        m_ScreenData.cx = Convert.ToInt32(readInfo);

        readInfo = ReadFromFileXml(fileName, "CY");
        if (readInfo == null || readInfo == "")
        {
            readInfo = "720";
            WriteToFileXml(fileName, "CY", readInfo);
        }
        m_ScreenData.cy = Convert.ToInt32(readInfo);
    }

    public void WriteToFileXml(string fileName, string attribute, string valueStr)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		filepath = Application.persistentDataPath + "//" + fileName;
#endif

        //create file
        if (!File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Config");
            XmlElement elmNew = xmlDoc.CreateElement("attribute");

            root.AppendChild(elmNew);
            xmlDoc.AppendChild(root);
            xmlDoc.Save(filepath);
            File.SetAttributes(filepath, FileAttributes.Normal);
        }

        //update value
        if (File.Exists(filepath))
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filepath);
            XmlNodeList nodeList = xmlDoc.SelectSingleNode("Config").ChildNodes;

            foreach (XmlElement xe in nodeList)
            {
                xe.SetAttribute(attribute, valueStr);
            }
            File.SetAttributes(filepath, FileAttributes.Normal);
            xmlDoc.Save(filepath);
        }
    }
    
    public string ReadFromFileXml(string fileName, string attribute)
    {
        string filepath = Application.dataPath + "/" + fileName;
#if UNITY_ANDROID
		//filepath = Application.persistentDataPath + "//" + fileName;
#endif
        string valueStr = null;

        if (File.Exists(filepath))
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filepath);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("Config").ChildNodes;
                foreach (XmlElement xe in nodeList)
                {
                    valueStr = xe.GetAttribute(attribute);
                }
                File.SetAttributes(filepath, FileAttributes.Normal);
                xmlDoc.Save(filepath);
            }
            catch (Exception exception)
            {
                File.SetAttributes(filepath, FileAttributes.Normal);
                File.Delete(filepath);
                Debug.LogError("Unity:" + "error: xml was wrong! " + exception);
            }
        }
        return valueStr;
    }
}
