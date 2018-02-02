using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// Kolejne węzły w sekwencji są wykonywane, aż natrafimy na błąd/porażkę/uruchomienie.
// Sekwencja zwraca sukces, jeśli wszystkie dzieci zwróciły sukces.
// UWAGA: sekwencja jest za każdym wywołaniem zaczynana od nowa
public class BT_Sequence : BT_CompositeNode
{
    public override BT_Result Tick(BT_AgentMemory am)
	{
        for (int i = 0; i < Children.Count; i++)
        {
            BT_Result result = Children[i].Tick(am);

            if (result != BT_Result.SUCCESS)
            {
                return result;
            }
        }

        return BT_Result.SUCCESS;        
    }
}
