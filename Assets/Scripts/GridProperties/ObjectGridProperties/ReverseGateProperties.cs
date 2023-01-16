using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseGateProperties : BasicGateProperties
{
    public override bool ShouldClose()
    {
        return IsActivated;
    }
}
