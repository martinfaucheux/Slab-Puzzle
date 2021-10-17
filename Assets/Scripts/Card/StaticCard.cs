using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCard : Card
{
    public override bool CheckActivation()
    {
        return true;
    }

    public override void Start()
    {
        base.Start();
    }
}
