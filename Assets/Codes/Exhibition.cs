using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exhibition
{

    private int exhibitionId = Utilities.INVALID;
    private int organizerId = Utilities.INVALID;
    private string displayName;
    private string location;
    private DateTime startDate;
    private DateTime endDate;
    private string description;
    private int popularity = Utilities.INVALID;

    #region Property
    public int ExhibitionId
    {
        get
        {
            return exhibitionId;
        }
        set
        {
            setExhibtionId(value);
        }
    }
    public int OrganizerId
    {
        get
        {
            return organizerId;
        }
        set
        {
            if (!value.Equals(Utilities.INVALID))
                organizerId = value;
        }
    }
    public string DisplayName { get { return displayName; } set { if (value.Length > 0) displayName = value; } }
    public string Location { get { return location; } set { if (value.Length > 0) location = value; } }
    public DateTime StartDate { get { return startDate; } set { if (value <= endDate) startDate = value; } }
    public DateTime EndDate { get { return endDate; } set { if (value >= endDate) startDate = value; } }
    public string Description { get { return description; } set { if (value.Length > 0) description = value; } }
    public int Popularity
    {
        get
        {
            if (popularity < 0)
                return 0;
            return popularity;
        }
        set
        {
            if (popularity >= 0)
                popularity = value;
        }
    }
    #endregion

    private bool initialized = false;

    public Exhibition(int exhibitionId, int organizerId, string displayName, string location,
        DateTime startDate, DateTime endDate, string description, int popularity)
    {
        if (exhibitionId != Utilities.INVALID && organizerId != Utilities.INVALID && displayName.Length > 0)
        {
            this.exhibitionId = exhibitionId;
        }
        else
        {
            this.exhibitionId = Utilities.INVALID;
        }
        this.organizerId = organizerId;
        this.displayName = displayName;
        this.location = location;
        this.startDate = startDate;
        this.endDate = endDate;
        this.description = description;
        this.popularity = popularity;
    }

    public void setExhibtionId(int exhibitionId)
    {
        if (!initialized && exhibitionId != Utilities.INVALID)
        {
            this.exhibitionId = exhibitionId;
            initialized = true;
        }
    }

    public bool IsValid()
    {
        if (exhibitionId == Utilities.INVALID || organizerId == Utilities.INVALID || displayName.Length <= 0)
            return false;
        return true;
    }

    public static Exhibition Convert(EESExhibition eesExhibition)
    {
        if (eesExhibition.exhibitionId != Utilities.INVALID &&
            eesExhibition.organizerId != Utilities.INVALID &&
            eesExhibition.displayName.Length > 0)
        {
            return new Exhibition(eesExhibition.exhibitionId, eesExhibition.organizerId, eesExhibition.displayName,
                eesExhibition.location, eesExhibition.startDate, eesExhibition.endDate, eesExhibition.description,
                eesExhibition.popularity);
        }
        else
        {
            return null;
        }
    }
}
