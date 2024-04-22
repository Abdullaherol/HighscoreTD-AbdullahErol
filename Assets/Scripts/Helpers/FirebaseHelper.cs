using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System;
using UnityEngine;

public class FirebaseHelper
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    public FirebaseHelper()
    {
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void SignInAnonymously(Action<FirebaseUser> onSignInSuccess)
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Anonymous sign-in failed.");
            }
            else
            {
                FirebaseUser user = task.Result.User;
                Debug.LogFormat("User signed in successfully: {0}", user.UserId);
                onSignInSuccess?.Invoke(user);
            }
        });
    }

    public void SaveUserData(string userId, string jsonData, Action onSuccess, Action<string> onFailure)
    {
        dbRef.Child("userSaves").Child(userId).SetRawJsonValueAsync(jsonData).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error saving user data: " + task.Exception);
                onFailure?.Invoke("Failed to save user data");
            }
            else
            {
                Debug.Log("User data saved successfully.");
                onSuccess?.Invoke();
            }
        });
    }

    public void GetUserSaveData(string userId, Action<string> onDataReceived, Action onFailure)
    {
        dbRef.Child("userSaves").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Error retrieving user data: " + task.Exception);
                onFailure?.Invoke();
            }
            else
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists && snapshot.Value != null)
                {
                    string jsonData = snapshot.GetRawJsonValue();
                    onDataReceived(jsonData);
                }
                else
                {
                    Debug.Log("No user save found.");
                    onFailure?.Invoke();
                }
            }
        });
    }

    public void CheckUserSaveExists(string userId, Action<bool> onResult)
    {
        dbRef.Child("userSaves").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking user save: " + task.Exception);
                onResult(false);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                onResult(snapshot.Exists && snapshot.Value != null);
            }
        });
    }
}
