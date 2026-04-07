using System;
using UnityEngine;

[Serializable]
public class Account
{
    public int Id;
    public string Email;
    public string Password;
    public string Password_Salt;
    public string Name;
    public DateTime Created_at;
    public DateTime Updated_at;
}
