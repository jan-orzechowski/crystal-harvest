using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_FindWorkplace : BT_Node 
{
    public override BT_Result Tick(BT_AgentMemory am)
    {
        if (am.Workplace != null
            && am.Character.IsTileMarkedAsInaccessible(
               am.Workplace.GetAccessTile(am.UseWorkplaceSecondAccessTile)) == false)
        {
            Debug.Log("mamy już dostępne miejsce pracy");
            return BT_Result.SUCCESS;
        }

        if (am.PotentialWorkplace == null)
        {
            Debug.Log("Szukanie - nowy kandydat");

            am.Workplace = null;
            am.UseWorkplaceSecondAccessTile = false;
            
            am.PotentialWorkplace = GameManager.Instance.World.GetAvailableWorkplace(am.Character);

            if (am.PotentialWorkplace == null) return BT_Result.FAILURE;

            //if (am.Character.IsTileMarkedAsInaccessible(am.PotentialWorkplace.GetAccessTile(false))
            //    && am.Character.IsTileMarkedAsInaccessible(am.PotentialWorkplace.GetAccessTile(true)) == false)
            //    am.UseWorkplaceSecondAccessTile = true;
            
            am.Character.SetNewDestination(am.PotentialWorkplace.GetAccessTile(am.UseWorkplaceSecondAccessTile), false);

            return BT_Result.RUNNING;
        }
        else
        {
            Tile accessTile = am.PotentialWorkplace.GetAccessTile(am.UseWorkplaceSecondAccessTile);
            
            if (am.Character.IsTileMarkedAsInaccessible(accessTile))
            {
                Debug.Log("pole niedostępne");
                am.PotentialWorkplace = null;
                return BT_Result.RUNNING;
            }

            if (am.Character.DestinationTile != accessTile)
            {
                Debug.Log("Szukanie - błędne pole");
                am.PotentialWorkplace = null;
                return BT_Result.RUNNING;
            }

            Debug.Log("szukanie trwa");
            
            if (am.Character.CurrentTile == accessTile)
            {
                am.Workplace = am.PotentialWorkplace;
                am.PotentialWorkplace = null;
                Debug.Log("Znaleziono miejsce pracy: " + am.Workplace.Building.Name);
                return BT_Result.SUCCESS;
            }

            if (am.Character.Path != null && am.Character.Path.IsReady)
            {
                if (am.Character.Path.IsImpossible)
                {
                    Debug.Log("Szukanie - ścieżka niemożliwa");
                    am.PotentialWorkplace = null;
                    return BT_Result.RUNNING;
                }
                else
                {
                    am.Workplace = am.PotentialWorkplace;
                    am.PotentialWorkplace = null;
                    Debug.Log("Znaleziono miejsce pracy: " + am.Workplace.Building.Name);                    
                    return BT_Result.SUCCESS;
                }
            }
            else
            {
                return BT_Result.RUNNING;
            }
        }
    }
}
