using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IButton
{
    public bool IsPressed { get; set; }
    public bool IsActivated();
}
