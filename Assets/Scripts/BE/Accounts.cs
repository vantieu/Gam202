
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Accounts : MonoBehaviour
{
    public TMP_InputField emailInput;

    public void GetAllAccounts()
    {
        StartCoroutine(GetAll());
    }

    IEnumerator GetAll()
    {
        var url = "http://localhost:5297/api/allaccount";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                ResponseListAccount responseListAccount = JsonUtility.FromJson<ResponseListAccount>(request.downloadHandler.text);

                Debug.Log(request.downloadHandler.text);
                Debug.Log(responseListAccount.Nofitication);

                if (responseListAccount.Issuccess)
                {
                    foreach (var acc in responseListAccount.Data)
                    {
                        Debug.Log($"Username: {acc.Name}, Email: {acc.Email}.");
                    }
                }
                else
                    Debug.Log("Lỗi không lấy ds tài khoản");
            }
        }
    }

    public void GetAccountByEmail()
    {
        StartCoroutine(GetByEmail());
    }

    IEnumerator GetByEmail()
    {
        var email = emailInput.text;
        var url = $"http://localhost:5297/api/getaccountbyemail/{email}";

        Debug.Log(url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            // Kiểm tra dl trả về

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Response response = JsonUtility.FromJson<Response>(request.downloadHandler.text);

                Debug.Log(request.downloadHandler.text);
                Debug.Log(response.Nofitication);

                if (response.Issuccess)
                {
                    Debug.Log("Email: " + response.Data.Email);
                }
                else
                    Debug.Log("Lỗi không lấy ds tài khoản");
            }
        }
    }
}
