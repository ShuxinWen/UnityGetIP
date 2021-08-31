using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 获取的IP类型
/// </summary>
public enum ADDRESSFAM
{
    IPv4, IPv6
}
public class GetIP : MonoBehaviour
{

    private Text getIP;
    private InputField inPutIP;
    private Text result;
    private Button button;

    // Start is called before the first frame update
    void Start()
    {
        getIP = GameObject.Find("获取IP").GetComponent<Text>();
        result = GameObject.Find("结果").GetComponent<Text>();
        inPutIP = GameObject.Find("InputField").GetComponent<InputField>();
        button = GameObject.Find("验证").GetComponent<Button>();
        button.onClick.AddListener(() => 
        {
            bool isright = JudgeIPFormat(inPutIP.text);
            if (isright)
                result.text = "输入的IP：" + inPutIP.text + " 格式正确！";
            else
                result.text = "输入的IP：" + inPutIP.text + " 格式错误！";
        });
        getIP.text = GetLocalIP(ADDRESSFAM.IPv4);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    /// <summary>
    /// 获取本机IP
    /// </summary>
    /// <param name="Addfam">要获取的IP类型</param>
    /// <returns></returns>
    public static string GetLocalIP(ADDRESSFAM Addfam)
    {
        if (Addfam == ADDRESSFAM.IPv6 && !Socket.OSSupportsIPv6)
        {
            return null;
        }

        string output = "";

        foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            NetworkInterfaceType _type1 = NetworkInterfaceType.Wireless80211;
            NetworkInterfaceType _type2 = NetworkInterfaceType.Ethernet;

            if ((item.NetworkInterfaceType == _type1 || item.NetworkInterfaceType == _type2) && item.OperationalStatus == OperationalStatus.Up)
#endif 
            {
                foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                {
                    //IPv4
                    if (Addfam == ADDRESSFAM.IPv4)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                            //Debug.Log("IP:" + output);
                        }
                    }

                    //IPv6
                    else if (Addfam == ADDRESSFAM.IPv6)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
        }
        return output;
    }

    /// <summary>
    /// 判断IP
    /// </summary>
    /// <param name="strJudgeString"></param>
    /// <returns></returns>
    private bool JudgeIPFormat(string strJudgeString)
    {
        bool blnTest = false;
        bool _Result = true;
        //正则表达式
        Regex regex = new Regex("^[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}$");
        blnTest = regex.IsMatch(strJudgeString);
        if (blnTest == true)
        {
            string[] strTemp = strJudgeString.Split(new char[] { '.' }); // textBox1.Text.Split(new char[] { ‘.’ });
            int nDotCount = strTemp.Length - 1; //字符串中.的数量，若.的数量小于3，则是非法的ip地址
            if (3 == nDotCount)//判断字符串中.的数量
            {
                for (int i = 0; i < strTemp.Length; i++)
                {
                    if (Convert.ToInt32(strTemp[i]) > 255)
                    {
                        //大于255则提示，不符合IP格式                     
                        Debug.Log("不符合IP格式");
                        _Result = false;
                    }
                }
            }
            else
            {
                Debug.Log("不符合IP格式");
                _Result = false;
            }
        }
        else
        {
            //输入非数字则提示，不符合IP格式
            _Result = false;
        }
        return _Result;
    }

}
