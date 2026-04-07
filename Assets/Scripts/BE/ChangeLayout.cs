using UnityEditor.Overlays;
using UnityEngine;

public class ChangeLayout : MonoBehaviour
{
    public GameObject Account;
    public GameObject Character;

    void Start()
    {
        Character.SetActive(false);
    }

    public void ShowPanel1()
    {
        Account.SetActive(true);
        Character.SetActive(false);
    }

    public void ShowPanel2()
    {
        Account.SetActive(false);
        Character.SetActive(true);
    }

    public void TogglePanels()
    {
        bool isAccountActive = Account.activeSelf;

        Account.SetActive(!isAccountActive);
        Character.SetActive(isAccountActive);
    }
}
