using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Button loadGame;
    public event Action<string> newGame;
    string folderName;
    private void Awake()
    {
        if(Directory.GetDirectories(Application.persistentDataPath + "/saves").Length == 0)
        {
            loadGame.interactable = false;
        }
    }
    public void ManageWorld()
    {
        Transform t = transform.GetChild(2);
        t.gameObject.SetActive(false);
        t.GetChild(0).GetComponent<TMP_InputField>().text = "";
    }
    public void CreateWorld(bool overwrite)
    {
        string[] dirs = Directory.GetDirectories(Application.persistentDataPath);
        if (!Directory.GetDirectories(Application.persistentDataPath).Contains(Application.persistentDataPath + "/saves"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves");
        }
        folderName = transform.GetChild(2).GetChild(0).GetComponent<TMP_InputField>().text;
        if (folderName != null)
        {
            List<string> dir = Directory.GetDirectories(Application.persistentDataPath + "/saves").ToList();
            if (overwrite || !dir.Contains(Application.persistentDataPath + "/saves/" + folderName))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/saves/" + folderName);
                newGame?.Invoke(folderName);
            }
            else
            {
                transform.GetChild(2).GetChild(3).gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("set Value");
        }
    }
}
