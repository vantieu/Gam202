using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Characters : MonoBehaviour
{
    public TMP_InputField idInput;
    public TMP_InputField nameInput;

    public void GetAllCharacter()
    {
        StartCoroutine(GetAll());
    }
    IEnumerator GetAll()
    {
        var url = "http://localhost:5297/api/allcharacter";

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
                ResponseListCharacter response = JsonUtility.FromJson<ResponseListCharacter>(request.downloadHandler.text);

                Debug.Log(request.downloadHandler.text);
                Debug.Log(response.Nofitication);

                if (response.Issuccess)
                {
                    foreach (var cha in response.Data)
                    {
                        Debug.Log($"Id: {cha.Id},Username: {cha.Name}.");
                    }
                }
                else
                    Debug.Log("Lỗi không lấy ds nhân vật");
            }
        }
    }

    public void GetCharacterById()
    {
        StartCoroutine(GetById());
    }
    IEnumerator GetById()
    {
        var id = idInput.text;
        var url = $"http://localhost:5297/api/GetCharacterById/{id}";

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
                    Debug.Log("Id: " + response.Data.Id);
                    Debug.Log("Name: " + response.Data.Name);
                }
                else
                    Debug.Log("Lỗi không lấy ds tài khoản");
            }
        }
    }

    public void GetCharacterByName()
    {
        StartCoroutine(GetByName());
    }
    IEnumerator GetByName()
    {
        var name = nameInput.text;
        var url = $"http://localhost:5297/api/GetCharacterByName/{name}";

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
                    Debug.Log("Id: " + response.Data.Id);
                    Debug.Log("Name: " + response.Data.Name);
                }
                else
                    Debug.Log("Lỗi không lấy ds tài khoản");
            }
        }
    }
}
