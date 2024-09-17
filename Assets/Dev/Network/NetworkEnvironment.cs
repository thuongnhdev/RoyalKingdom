using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NetworkEnvironment", menuName = "Uniflow/Network/NetworkEnvironment")]
public class NetworkEnvironment : ScriptableObject
{
    [SerializeField]
    private Environment _currentEvironment;
    private enum Environment
    {
        DEV = 0,
        PROD_RELEASE = 1,
        KOREA_RELEASE = 2
    }

    [Header("Dev")]
    [SerializeField]
    private string _devApiHost;
    [SerializeField]
    private string _devApiPort;
    [SerializeField]
    private string _devSocketPort;

    [Header("Prod_Release")]
    [SerializeField]
    private string _proReleaseApiHost;
    [SerializeField]
    private string _proReleaseApiPort;
    [SerializeField]
    private string _proReleaseSocketPort;

    [Header("Korea_Release")]
    [SerializeField]
    private string _koreaReleaseApiHost;
    [SerializeField]
    private string _koreaReleaseApiPort;
    [SerializeField]
    private string _koreaReleaseSocketPort;

    private const string HTTP = "http://";
    private const string WEB_SOCKET = "ws://";

    public string GetApiHostAndPort()
    {
        switch (_currentEvironment)
        {
            case Environment.PROD_RELEASE:
                {
                    return HTTP + _proReleaseApiHost + _proReleaseApiPort;
                }
            case Environment.KOREA_RELEASE:
                {
                    return HTTP + _koreaReleaseApiHost + _koreaReleaseApiPort;
                }
            default:
                {
                    return HTTP + _devApiHost + _devApiPort;
                }
        }
    }

    public string GetSocketHostAndPort()
    {
        switch (_currentEvironment)
        {
            case Environment.PROD_RELEASE:
                {
                    return WEB_SOCKET + _proReleaseApiHost + _proReleaseSocketPort;
                }
            case Environment.KOREA_RELEASE:
                {
                    return WEB_SOCKET + _koreaReleaseApiHost + _koreaReleaseSocketPort;
                }
            default:
                {
                    return WEB_SOCKET + _devApiHost + _devSocketPort;
                }
        }
    }
}
