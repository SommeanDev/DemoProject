using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System;
using UnityEngine.UI;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference databaseReference;

    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField ageInputField;
    [SerializeField] private Button submitButton;
    [SerializeField] private PushNotificationHandler pushNotificationHandler;
    
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log("Firebase Initialized");
        });

        submitButton.onClick.AddListener(OnSubmit);
    }

    private void OnSubmit()
    {
        string name = nameInputField.text;
        string ageText = ageInputField.text;
        string deviceToken = pushNotificationHandler.DeviceToken;
        
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(ageText))
        {
            Debug.LogError("Name or age is empty!");
            return;
        }

        if (!int.TryParse(ageText, out int age))
        {
            Debug.LogError("Invalid age input!");
            return;
        }

        AddUser(name, age, deviceToken);
    }

    public void AddUser(string name, int age, string deviceToken)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        UserData user = new UserData(name, age, deviceToken);
        string json = JsonUtility.ToJson(user);

        databaseReference.Child("users").Child(deviceId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User data successfully added to Firebase.");
            }
            else
            {
                Debug.LogError("Failed to add user data: " + task.Exception);
            }
        });
    }
}

[Serializable]
public class UserData
{
    public string name;
    public int age;
    public string deviceToken;
    
    public UserData(string name, int age, string deviceToken)
    {
        this.name = name;
        this.age = age;
        this.deviceToken = deviceToken;
    }
}
