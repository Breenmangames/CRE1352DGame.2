using System.Xml.Schema;
using UnityEngine;


public class Move 
{


    public MoveSet Base { get; set; }

    public int PP { get; set; }

    public Move(MoveSet bBase)
    {
         Base= bBase;
        PP = bBase.PP;
    }

}
