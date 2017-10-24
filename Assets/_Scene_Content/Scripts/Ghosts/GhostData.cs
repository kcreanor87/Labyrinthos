using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class GhostData : MonoBehaviour
{
    public static GhostData inst;
    public List<GhostList> _ghostLevels = new List<GhostList>();

    private void Awake()
    {
        inst = this;
        for (int i = 0; i < _playerManager._totalLevels; i++)
        {
            _ghostLevels.Add(new GhostList());
        }
    }

    public void SaveGhostData(int level)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GhostList));
        FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/GhostTimes/Level" + level + ".xml", FileMode.Create);
        serializer.Serialize(stream, _ghostLevels[level]);
        stream.Close();
        print("File Saved");
    }

    public void LoadGhostData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GhostList));
        FileStream stream = new FileStream(Application.dataPath + "/StreamingAssets/GhostTimes/Level" + _playerManager._levelIndex + ".xml", FileMode.Open);
        _ghostLevels[_playerManager._levelIndex] = serializer.Deserialize(stream) as GhostList;
        stream.Close();
        print("File Loaded");
    }
}
[System.Serializable]
public class GhostList
{
    public List<Vector3> _framePos = new List<Vector3>();
    public List<Vector3> _frameRot = new List<Vector3>();
}
 