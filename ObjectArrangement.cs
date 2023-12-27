using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectArrangement : MonoBehaviour
{
    public List<GameObject> objects;

    public Canvas canvas;
    public string fileName;
    List<List<double>> csvList;
    List<string> csvListName;
    List<List<string>> csvFiles;


    double keyCount;
    const double keyDownTime = 1;
    public int currentSenarioNumber;
    double textCount;
    const double textDisplayTime = 4;

    GameObject ovrCamera;


    /// <summary>
    /// Does not attach some Data Recorder Stript in this object.
    /// </summary>
    public GameObject dataRecorder;
    DataRecorder dr;

    public class ObjectState
    {
        public Vector3 position;
        public Vector3 rotation;

        public ObjectState()
        {
            position = new Vector3();
            rotation = new Vector3();
        }

        public PedestrianContollerVariable pcv;
    }

    public class PedestrianContollerVariable
    {
        public Vector3 triggerPoint;
        public float velocity;
        public float lookAheadDistance;
        public List<Vector3> path;
    }

    public enum Mode
    {
        PM,
        VR,
    }

    public Mode mode;

    List<ObjectState> states;
    NetworkCharacterControl ncc;
    public GameObject networkCharacterControl;
    public List<GameObject> cameraObjects;

    // Start is called before the first frame update
    void Start()
    {
        OpenSetting(fileName);
        dr = dataRecorder.GetComponent<DataRecorder>();

        ncc = networkCharacterControl.GetComponent<NetworkCharacterControl>();

        ovrCamera = GameObject.Find("OVRCameraRig");
    }



    void PM_Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            keyCount += Time.deltaTime;
            if (keyCount >= keyDownTime)
            {
                keyCount = 0;
                currentSenarioNumber--;
                if (currentSenarioNumber < 0)
                    currentSenarioNumber = 0;

                EnableCanvas(csvFiles[currentSenarioNumber][1]);
                OpenArrangement(csvFiles[currentSenarioNumber][1]);
                textCount = textDisplayTime;
            }
        }
        else if (Input.GetKey(KeyCode.L))
        {
            keyCount += Time.deltaTime;
            if (keyCount >= keyDownTime)
            {
                keyCount = 0;
                currentSenarioNumber++;
                if (currentSenarioNumber >= csvFiles.Count)
                    currentSenarioNumber = csvFiles.Count - 1;

                EnableCanvas(csvFiles[currentSenarioNumber][1]);
                OpenArrangement(csvFiles[currentSenarioNumber][1]);
                textCount = textDisplayTime;
            }
        }
        else
        {
            keyCount = 0;
        }
    }

    float hight;

    void VR_Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            keyCount += Time.deltaTime;
            if (keyCount >= keyDownTime)
            {
                keyCount = 0;
                currentSenarioNumber--;
                if (currentSenarioNumber < 1)
                    currentSenarioNumber = 1;

                EnableCanvas(csvFiles[currentSenarioNumber][1]);
                //OpenArrangement(csvFiles[currentSenarioNumber][1]);
                ncc.FileOpen(csvFiles[0][1] + @"\" + csvFiles[currentSenarioNumber][1]);

                if(ovrCamera != null)
                {
                    hight = ovrCamera.transform.position.y;
                    ovrCamera.transform.parent = cameraObjects[int.Parse(csvFiles[currentSenarioNumber][2])].transform;
                    ovrCamera.transform.localPosition = new Vector3(0, hight, 0);
                    ovrCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
                }

                //for (int i = 0; i < cameraObjects.Count; i++)
                //{
                //    cameraObjects[i].SetActive(false);
                //}
                //cameraObjects[int.Parse(csvFiles[currentSenarioNumber][2])].SetActive(true);
                textCount = textDisplayTime;
            }
        }
        else if (Input.GetKey(KeyCode.L))
        {
            keyCount += Time.deltaTime;
            if (keyCount >= keyDownTime)
            {
                keyCount = 0;
                currentSenarioNumber++;
                if (currentSenarioNumber >= csvFiles.Count)
                    currentSenarioNumber = csvFiles.Count - 1;

                EnableCanvas(csvFiles[currentSenarioNumber][1]);
                //OpenArrangement(csvFiles[currentSenarioNumber][1]);
                ncc.FileOpen(csvFiles[0][1] + @"\" + csvFiles[currentSenarioNumber][1]);

                if (ovrCamera != null)
                {
                    hight = ovrCamera.transform.position.y;
                    ovrCamera.transform.parent = cameraObjects[int.Parse(csvFiles[currentSenarioNumber][2])].transform;
                    ovrCamera.transform.localPosition = new Vector3(0, hight, 0);
                    ovrCamera.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                //for (int i = 0; i < cameraObjects.Count; i++)
                //{
                //    cameraObjects[i].SetActive(false);
                //}
                //cameraObjects[int.Parse(csvFiles[currentSenarioNumber][2])].SetActive(true);
                textCount = textDisplayTime;
            }
        }
        else
        {
            keyCount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mode == Mode.PM)
        {
            PM_Update();
        }
        else if(mode == Mode.VR)
        {
            VR_Update();
        }

        if (textCount > 0)
        {
            textCount -= Time.deltaTime;
            if (textCount <= 0)
            {
                textCount = 0;
                DisableCanvas();
            }
        }

    }

    void EnableCanvas(int count)
    {
        var text = canvas.gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
        text.text = "試行：" + count;
        canvas.gameObject.SetActive(true);
    }

    void EnableCanvas(string str)
    {
        var text = canvas.gameObject.GetComponentInChildren<UnityEngine.UI.Text>();
        text.text = str;
        textCount = textDisplayTime;
        canvas.gameObject.SetActive(true);
    }

    void DisableCanvas()
    {
        canvas.gameObject.SetActive(false);
    }

    System.IO.StreamReader OpenCSV(string file)
    {
        System.IO.StreamReader sr;
        try
        {
            sr = new System.IO.StreamReader(Application.dataPath + @"\" + file);
        }
        catch
        {
            return null;
        }
        return sr;
    }

    void OpenSetting(string file)
    {
        System.IO.StreamReader sr = OpenCSV(file);

        string str = sr.ReadLine();
        string[] strs;
        csvFiles = new List<List<string>>();

        while (!sr.EndOfStream)
        {
            csvFiles.Add(new List<string>());
            str = sr.ReadLine();
            strs = str.Split(',');

            for (int i = 0; i < strs.Length; i++)
            {
                csvFiles[csvFiles.Count - 1].Add(strs[i]);
            }
        }


    }

    void OpenArrangement(string file)
    {
        dr.CloseFile();

        string[] strs = file.Split('.');

        dr.OpenFile(strs[0]);

        System.IO.StreamReader sr = OpenCSV(file);
        
        string str = sr.ReadLine();
        csvList = new List<List<double>>();

        int count = 0;
        while (!sr.EndOfStream)
        {
            csvList.Add(new List<double>());
            str = sr.ReadLine();
            strs = str.Split(',');
            foreach (var item in strs)
            {
                if(item != "")
                    csvList[count].Add(double.Parse(item));
            }
            count++;
        }

        states = new List<ObjectState>();

        for (int i = 0; i < csvList.Count; i++)
        {
            states.Add(new ObjectState());

            states[i].position = new Vector3((float)csvList[i][1], (float)csvList[i][2], (float)csvList[i][3]);
            states[i].rotation = new Vector3((float)csvList[i][4], (float)csvList[i][5], (float)csvList[i][6]);

            if(csvList[i].Count > 7)
            {
                states[i].pcv = new PedestrianContollerVariable();
                states[i].pcv.triggerPoint = new Vector3((float)csvList[i][7], (float)csvList[i][8], (float)csvList[i][9]);
                states[i].pcv.velocity = (float)csvList[i][10];
                states[i].pcv.lookAheadDistance = (float)csvList[i][11];
                states[i].pcv.path = new List<Vector3>();
                count = 12;
                while(csvList[i].Count >= count+2)
                {
                    states[i].pcv.path.Add(new Vector3((float)csvList[i][count], (float)csvList[i][count+1], (float)csvList[i][count+2]));
                    count += 3;
                }
            }
        }

        for (int i = 0; i < states.Count; i++)
        {
            objects[i].transform.position = states[i].position;
            objects[i].transform.eulerAngles = states[i].rotation;
            if(states[i].pcv != null)
            {
                SetPedestrianController(ref states[i].pcv, objects[i]);
            }
        }
    }

    void SetPedestrianController(ref PedestrianContollerVariable pcv,GameObject obj)
    {
        PedestrianController pc;
        try
        {
            pc = obj.GetComponent<PedestrianController>();
        }
        catch
        {
            return;
        }

        //pc.triggerPoint = pcv.triggerPoint;
        pc.velocity = pcv.velocity;
        pc.lookAheadDistance = pcv.lookAheadDistance;
        pc.path = pcv.path;
        pc.SettingPath();
    }

}
