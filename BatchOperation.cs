using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Reflection;

[ExecuteInEditMode] // SendMessageでエラーが出ないように
public class BatchOperation : MonoBehaviour
{
    List<GameObject> list;
    public string scriptName;

    public string propertyName;
    public string value;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SearchObjects()
    {
        list = new List<GameObject>();
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            list.Add(this.gameObject.transform.GetChild(i).gameObject);
        }
    }

    public void SetActiveScript()
    {
        SearchObjects();

        Type type = Type.GetType(scriptName);
        for (int i = 0; i < list.Count; i++)
        {
            ((MonoBehaviour)list[i].GetComponentInChildren(type)).enabled = true;
        }
    }

    public void SetDisactiveScript()
    {
        SearchObjects();

        Type type = Type.GetType(scriptName);
        for (int i = 0; i < list.Count; i++)
        {
            ((MonoBehaviour)list[i].GetComponentInChildren(type)).enabled = false;
        }
    }

    public void SetActiveVariable()
    {
        SearchObjects();
        Type type = Type.GetType(scriptName);
        FieldInfo field = type.GetField(propertyName);

        object obj;
        switch (value)
        {
            case "true":
                obj = true;
                break;
            case "false":
                obj = false;
                break;

            default:
                obj = "";
                break;
        }

        for (int i = 0; i < list.Count; i++)
        {
            field.SetValue(list[i].GetComponentInChildren(type),obj);
        }
    }

}
