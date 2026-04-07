using System;
using UnityEngine;

[Serializable]
public class Character
{
    public int Id;
    public int Account_id;
    public string? Name;
    public int Level;
    public int Exp;
    public DateTime Created_at;
    public DateTime Updated_at;
}
