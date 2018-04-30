using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Service : IBuildingModule
{
    public Building Building { get; protected set; }
    public BuildingPrototype Prototype { get { return Building.Prototype; } }

    public StorageWithRequirements InputStorage { get; protected set; }

    public Character ServicedCharacter { get; protected set; }

    Character reservation;
    float reservationTimer;

    float serviceDuration;

    public string NeedFulfilled { get; protected set; }
    float NeedFulfillmentPerSecond;

    public bool HidesCharacter { get { return Prototype.HidesCharacter; } }

    bool haltedForDeconstruction;

    public bool Halted { get { return haltedForDeconstruction; } }

    public Service(Building building)
    {
        Building = building;

        InputStorage = new StorageWithRequirements(building, Prototype.ConsumedResources);

        NeedFulfilled = Prototype.NeedFulfilled;
        NeedFulfillmentPerSecond = Prototype.NeedFulfillmentPerSecond;

        serviceDuration = Prototype.ServiceDuration;
    }

    public void UpdateService(float deltaTime)
    {
        if (ServicedCharacter != null)
        {
            serviceDuration -= deltaTime;

            ServicedCharacter.Needs[NeedFulfilled] -= NeedFulfillmentPerSecond * deltaTime;
            if (ServicedCharacter.Needs[NeedFulfilled] < 0f) ServicedCharacter.Needs[NeedFulfilled] = 0f;

            if (ServicedCharacter.Needs[NeedFulfilled] <= 0f && serviceDuration <= 0f)
            {
                ServicedCharacter.ServiceEnded();
                serviceDuration = Prototype.ServiceDuration;
                ServicedCharacter = null;
                reservation = null;
            }
        }
        else
        {
            reservationTimer -= deltaTime;
            if (reservationTimer <= 0f)
            {
                reservation = null;
            }
        }
    }

    bool Consume()
    {
        if (InputStorage.AreRequirementsMet)
        {
            InputStorage = new StorageWithRequirements(Building, Prototype.ConsumedResources);
            GameManager.Instance.World.UnregisterResources(Prototype.ConsumedResources);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool StartService(Character character)
    {
        if ((reservation == null || reservation == character) && Consume())
        {
            ServicedCharacter = character;
            ServicedCharacter.StartUsingService();
            serviceDuration = Prototype.ServiceDuration;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanReserveService(Character character)
    {
        return (haltedForDeconstruction == false
                && (reservation == character || reservation == null) 
                && ServicedCharacter == null);
    }

    public bool ReserveService(Character character)
    {
        if (CanReserveService(character))
        {
            reservation = character;
            reservationTimer = 5f;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RenewServiceReservation(Character character)
    {
        if (reservation == character)
        {
            reservationTimer = 5f;
        }
    }

    public void StartDeconstructionPreparation()
    {
        if (ServicedCharacter != null)
        {
            ServicedCharacter.InterruptActivity();
            ServicedCharacter = null;
        }
        haltedForDeconstruction = true;
        reservation = null;
        serviceDuration = Prototype.ServiceDuration;
        InputStorage.StartDeconstructionPreparation();
    }

    public void CancelDeconstructionPreparation()
    {
        InputStorage.CancelDeconstructionPreparation();
        haltedForDeconstruction = false;
    }

    public bool IsPreparingForDeconstruction()
    {
        return haltedForDeconstruction;
    }

    public bool IsReadyForDeconstruction()
    {
        return (ServicedCharacter == null
                && InputStorage.IsReadyForDeconstruction());
    }

    public float GetServicePercentage()
    {
        return ((Prototype.ServiceDuration - serviceDuration) / Prototype.ServiceDuration);
    }

    public void SetHalt(bool halt)
    {
        return;
    }

    public Tile GetAccessTile(bool second = false)
    {
        return Building.GetAccessTile(second);
    }

    public Rotation GetAccessTileRotation(bool second = false)
    {
        return Building.GetAccessTileRotation(second);
    }

    public string DEBUG_GetSelectionText()
    {
        string s = "";

        s += "Korzystająca postać: ";
        if (ServicedCharacter != null)
        {
            s += ServicedCharacter.Name + "\n";
            s += "Pozostały czas produkcji: ";
            s += serviceDuration + "\n";
        }
        s += "\n";
        s += "Rezerwacja: ";
        if (reservation != null)
        {
            s += reservation.Name + "\n";
        }

        s += InputStorage.DEBUG_GetSelectionText();
        return s;
    }
}
