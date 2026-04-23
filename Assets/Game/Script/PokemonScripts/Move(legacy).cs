using System.Xml.Schema;
using UnityEngine;


public class Move 
{


    public MoveSet Base { get; set; }

    public int PP { get; set; }

    public Move(MoveSet pBase)
    {
         Base= pBase;
        PP = pBase.PP;
    }

}
