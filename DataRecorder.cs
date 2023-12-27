using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class DataRecorder : MonoBehaviour {

    public string saveDir;
    public List<EditorTargetList> trackingTarget;

    bool isOpen = false;

    [System.Serializable]
    public class EditorTargetList
    {
        public GameObject obj;
        public bool isWritePosition = true;
        public bool isWriteVel = true;
    }

    System.IO.StreamWriter sw;

    [System.Serializable]
    public class EditorDataList
    {
        public string label;
        public string name;
        public int number;
        public FieldInfo info;
        public object obj;

        public EditorDataList()
        {
            name = "";
            number = 0;
            info = null;
            obj = null;
        }
    }

    [HideInInspector, SerializeField]
    public List<EditorDataList> list = new List<EditorDataList>();

    bool isEnableDir = true;

    public int saveDataSize;

    string dir;
    float time;

    // (change for vel; )const float deg2rad = Mathf.PI / 180;

    object writeData;


    // Use this for initialization
    void Start()
    {

        OpenFile();
    }

    public void OpenFile(string dir,string file_name)
    {

        if (!System.IO.Directory.Exists(dir))
        {
            Debug.LogWarning("DataRecoderのデータ保存場所が見つかりません\n今回のデータは保存されません！");
            isEnableDir = false;
            return;
        }

        isOpen = true;

        if (file_name == "")
        {
            dir += @"\data_";
        }
        else
        {
            dir += @"\" + file_name + @"_data_";
        }

        dir +=
            System.DateTime.Now.Month.ToString("00") +
            System.DateTime.Now.Day.ToString("00") +
            "_" +
            System.DateTime.Now.Hour.ToString("00") +
            System.DateTime.Now.Minute.ToString("00") +
            System.DateTime.Now.Second.ToString("00") +
            @".csv";
        sw = new System.IO.StreamWriter(dir, false, System.Text.Encoding.Default);
        time = 0;

        string[] strs;
        try
        {
            //search FieldInfo
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].name != "None")
                {
                    MonoBehaviour[] components = trackingTarget[list[i].number].obj.GetComponents<MonoBehaviour>();
                    FieldInfo f;
                    strs = list[i].name.Split(':');
                    foreach (var component in components)
                    {
                        if (component.GetType().Name == strs[0])
                        {
                            f = component.GetType().GetField(strs[1]);
                            if (f != null)
                            {
                                list[i].info = f;
                                list[i].obj = component;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    list[i].info = null;
                }
            }
        }
        catch
        { }

        try
        {
            //write header
            sw.Write("time");
            for (int i = 0; i < trackingTarget.Count; i++)
            {
                if (trackingTarget[i].isWritePosition)
                {
                    sw.Write("," + i + "_x_pos_" + trackingTarget[i].obj.name);
                    sw.Write("," + i + "_y_pos_" + trackingTarget[i].obj.name);
                    sw.Write("," + i + "_z_pos_" + trackingTarget[i].obj.name);
                }
                if (trackingTarget[i].isWritePosition)
                {
                    sw.Write("," + i + "_roll_" + trackingTarget[i].obj.name);
                    sw.Write("," + i + "_yaw_" + trackingTarget[i].obj.name);
                    sw.Write("," + i + "_pitch_" + trackingTarget[i].obj.name);
                }
            }
        }
        catch { }

        string temp;
        int cCount;

        try
        {
            for (int i = 0; i < list.Count; i++)
            {
                temp = list[i].info.GetValue(list[i].obj).ToString();

                cCount = temp.Length - temp.Replace(",", "").Length;

                if(cCount == 0)
                {
                    sw.Write("," + list[i].name.Split(':')[1] + "_" + ((MonoBehaviour)list[i].obj).name);
                }
                else
                {
                    for (int n = 0; n < cCount + 1; n++)
                    {
                        sw.Write("," + list[i].name.Split(':')[1] + "_" + ((MonoBehaviour)list[i].obj).name + "_" + n);
                    }
                }
            }
        }
        catch { }

        sw.WriteLine();
    }

    public void OpenFile(string file_name)
    {
        //open save file
        if (saveDir == "")
        {
            dir = @"C:";
        }
        else
        {
            dir = saveDir;
        }
        OpenFile(dir, file_name);
    }

    public void OpenFile()
    {
        //open save file
        if (saveDir == "")
        {
            dir = @"C:";
        }
        else
        {
            dir = saveDir;
        }
        OpenFile(dir,"");
    }

    public void CloseFile()
    {
        if (isOpen)
        {
            isOpen = false;
            if (sw != null)
                sw.Close();
        }
    }

    private void OnApplicationQuit()
    {

        CloseFile();
    }

    //Update is called once per frame
    //void Update()
    //{

    //}

    float ChangeDomain(float degree)
    {
        return degree > 180 ? degree * deg2rad - Mathf.PI * 2 : degree * deg2rad;
    }

    char [] brackets = {'(',')'};

    private void FixedUpdate()
    {
        if (!isEnableDir)
            return;

        if (!isOpen)
            return;

        sw.Write(time);
        try
        {
            for (int i = 0; i < trackingTarget.Count; i++)
            {
                if (trackingTarget[i].isWritePosition)
                {
                    sw.Write("," + trackingTarget[i].obj.transform.position.x);
                    sw.Write("," + trackingTarget[i].obj.transform.position.y);
                    sw.Write("," + trackingTarget[i].obj.transform.position.z);
                }
                if (trackingTarget[i].isWritePosition)
                {
                    sw.Write("," + ChangeDomain(trackingTarget[i].obj.transform.rotation.eulerAngles.x));
                    sw.Write("," + ChangeDomain(trackingTarget[i].obj.transform.rotation.eulerAngles.y));
                    sw.Write("," + ChangeDomain(trackingTarget[i].obj.transform.rotation.eulerAngles.z));
                }
            }
        }
        catch { }

        try
        {
            for (int i = 0; i < list.Count; i++)
            {
                writeData = list[i].info.GetValue(list[i].obj);
                if(writeData is Vector3)
                {
                    sw.Write("," + ((Vector3)writeData).ToString("F6").Trim(brackets));
                }
                else
                {
                    sw.Write("," + writeData.ToString());
                }

            }
        }
        catch { }

        sw.WriteLine();

        time += Time.fixedDeltaTime;
    }
}
