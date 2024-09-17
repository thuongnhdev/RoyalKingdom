using System.Collections;
using System.Collections.Generic;
using CoreData.UniFlow.Common;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using System;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;

public class CharacterDataSource : MonoSingleton<CharacterDataSource>
{
    private List<VassarData> vassarDatas = new List<VassarData>();
    private List<WorkerData> workerDatas = new List<WorkerData>();

    [SerializeField]
    private string pathFolderVassarAsset = "Assets/Dev/Characteristic/Characteristic/Vassar/SO/";

    [SerializeField]
    private string pathFolderWorkerAsset = "Assets/Dev/Characteristic/Characteristic/Worker/SO/";

    private long timeRelease = 0;
    private string FileAsset = ".asset";
    private string FilePrefab = ".prefab";

    private void Start()
    {
        this.timeRelease = DateTime.Now.Ticks;

        InitDataVassarSource();
    }
    public void InitDataVassarSource()
    {

    }
}
