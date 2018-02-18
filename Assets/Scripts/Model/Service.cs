using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Service
{
    public Building Building { get; protected set; }
    BuildingPrototype prototype;

    public StorageToFill InputStorage;

    public Character ServicedCharacter { get; protected set; }

    Character reservation;
    float reservationTimer;

    float serviceDuration;

    public string NeedFulfilled { get; protected set; }
    float NeedFulfillmentPerSecond;

    public Service(Building building, BuildingPrototype prototype)
    {
        Building = building;
        this.prototype = prototype;

        InputStorage = new StorageToFill(building, prototype.ConsumedResources);

        NeedFulfilled = prototype.NeedFulfilled;
        NeedFulfillmentPerSecond = prototype.NeedFulfillmentPerSecond;
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
        if (InputStorage.IsFilled)
        {
            InputStorage = new StorageToFill(Building, prototype.ConsumedResources);
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
            ServicedCharacter.UsingService = true;
            serviceDuration = prototype.ServiceDuration;
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanReserveService(Character character)
    {
        return ((reservation == character || reservation == null) && ServicedCharacter == null);
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
        if(reservation == character)
        {
            reservationTimer = 5f;
        }
    }

    public Tile GetAccessTile()
    {
        return Building.AccessTile;
    }

    public Rotation GetAccessTileRotation()
    {
        return Building.AccessTileRotation;
    }

    public string GetSelectionText()
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

        s += InputStorage.GetSelectionText();
        return s;
    }
}
