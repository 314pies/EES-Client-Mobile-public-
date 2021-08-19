using EES.ClientAPIs.ClientModules;
using EES.Utilities;
using System.Collections;
using System.Collections.Generic;

public class Booth
{

    private int boothId = Utilities.INVALID;
    private string boothPosition;
    private int exhibitionId = Utilities.INVALID;
    private int holderId = Utilities.INVALID;
    private string holderName;
    private string displayName;
    private List<string> boothTag;
    private int popularity;

    #region Property
    public int BoothId
    {
        get { return boothId; }
    }
    public string BoothPosition
    {
        get { return boothPosition; }
    }
    public int ExhibitionId
    {
        get { return exhibitionId; }
    }
    public int HolderId
    {
        get { return holderId; }
    }
    public string HolderName
    {
        get { return holderName; }
    }
    public string DisplayName
    {
        get { return displayName; }
    }
    public int Popularity
    {
        get { return popularity; }
    }
    #endregion

    private bool initialized = false;

    public Booth(int boothId, string boothPosition, int exhibitionId, int holderId, string holderName,
        string displayName, List<string> boothTag, int popularity)
    {
        if (boothId != Utilities.INVALID && exhibitionId != Utilities.INVALID && holderId != Utilities.INVALID)
        {
            this.boothId = boothId;
            this.boothPosition = boothPosition;
            this.exhibitionId = exhibitionId;
            this.holderId = holderId;
            this.holderName = holderName;
            this.displayName = displayName;
            if (boothTag == null)
                this.boothTag = new List<string>();
            else
                this.boothTag = boothTag;
            initialized = true;
        }
    }

    public static Booth Convert(EESBooth eesBooth)
    {
        if (eesBooth.boothId != Utilities.INVALID &&
            eesBooth.exhibitionId != Utilities.INVALID &&
            eesBooth.holderId != Utilities.INVALID)
        { 
            return new Booth(eesBooth.boothId, eesBooth.boothPosition, eesBooth.exhibitionId, eesBooth.holderId,
                eesBooth.holderName, eesBooth.displayName, eesBooth.boothTag, eesBooth.popularity);
        }
        else
        {
            return null;
        }    
    }
}
