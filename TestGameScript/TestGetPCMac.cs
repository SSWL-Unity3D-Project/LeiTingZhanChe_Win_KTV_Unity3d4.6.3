using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestGetPCMac : MonoBehaviour {
    ///<summary>
    /// 通过WMI读取系统信息里的网卡MAC
    ///</summary>
    ///<returns></returns>
    public List<string> GetMacByWMI()
    {
        List<string> macs = new List<string>();
        //try
        //{
        //    string mac = "";
        //    ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
        //    ManagementObjectCollection moc = mc.GetInstances();
        //    foreach (ManagementObject mo in moc)
        //    {
        //        if ((bool)mo["IPEnabled"])
        //        {
        //            mac = mo["MacAddress"].ToString();
        //            macs.Add(mac);
        //        }
        //    }
        //    moc = null;
        //    mc = null;
        //}
        //catch
        //{
        //}

        return macs;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
