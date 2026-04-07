using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    public void GotoRegister()
    {
        // Chuyển scene
        SceneManager.LoadScene("Register");
    }

    public void OnLoginClick()
    {
        var email = emailInput.text;
        var password = passwordInput.text;

        var account = new Account
        {
            Email = email,
            Password = password
        };

        // Chuyển đối tượng thành dạng chuỗi
        var json = JsonUtility.ToJson(account);
        Debug.Log(json);
        StartCoroutine(OnLogin(json));
    }

    IEnumerator OnLogin(string json)
    {
        var url = "http://localhost:5297/api/login";
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
                Debug.Log("Đăng nhập thành công");
                // Chuyển trang Unity
                SceneManager.LoadScene("Lab6");
            }
            else
            {
                Debug.Log("Đăng nhập thất bại");
            }
        }
    }
}
