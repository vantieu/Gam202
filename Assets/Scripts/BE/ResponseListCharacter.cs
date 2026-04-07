using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

[Serializable]
public class ResponseListCharacter
{
    public bool Issuccess;
    public string Nofitication;
    public List<Character> Data;
}
