using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conditions {

    public string Name { get; set; }

    public string Description { get; set; }
    
    public string StartMsg { get; set; } 

    public Action<Pokemon> OnStart { get; set; }

    public Func<Pokemon, bool> OnBeforeMove { get; set; }

    public Action<Pokemon> OnAfterTurn { get; set;}

    public ConditionID ID { get; set; }
    
}
