using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    // Khai báo biến lưu giá trị 3 trường thông tin đăng ký tài khoản
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;
    public TMP_InputField nameInput;

    public void GotoLogin()
    {
        // Chuyển scene
        SceneManager.LoadScene("Login");
    }

    public void OnRegisterClick()
    {
        var email = emailInput.text;
        var password = passwordInput.text;
        var name = nameInput.text;

        var account = new Account
        {
            Email = email,
            Password = password,
            Name = name
        };

        // Chuyển đối tượng thành dạng chuỗi
        var json = JsonUtility.ToJson(account);
        Debug.Log(json);
        StartCoroutine(Post(json));
    }

    IEnumerator Post(string json)
    {
        var url = "http://localhost:5297/api/register";
        var request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        // Kiểm tra dl trả về
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error);
            Debug.Log("RESPONSE: " + request.downloadHandler.text); // 👈 thêm dòng này
        }
        else
        {
            // Chuyển từ json sang object
            var response = JsonUtility.FromJson<Response>(request.downloadHandler.text);

            Debug.Log(request.downloadHandler.text);
            Debug.Log(response.Nofitication);

            if (response.Issuccess)
            {
                //Debug.Log(response.data.email);
                //Debug.Log(response.data.userName);
                //Debug.Log(response.data.createAt);

                // Chuyển trang Unity
                SceneManager.LoadScene("Login");
            }
            else
            {
                Debug.Log("Lỗi đăng ký tài khoản");
            }
        }
    }
}
