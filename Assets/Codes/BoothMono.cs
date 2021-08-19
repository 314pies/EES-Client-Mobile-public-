using EES.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoothMono : MonoBehaviour {

    public GameObject BoothDetails;
    public GameObject Mask;
    [Header("Exhibition")]
    public Text Title;
    public Text Position;

    private int index;
    private Booth booth;

    public void Initialize(int index, Booth booth, GameObject boothDetails)
    {
        StartCoroutine(CloseMask());
        this.index = index;
        this.booth = booth;
        Title.text = booth.DisplayName;
        Position.text = booth.BoothPosition;
        BoothDetails = boothDetails;
    }

    IEnumerator CloseMask()
    {
        yield return new WaitForSeconds(0.2f);
        Mask.SetActive(false);
    }

    public void OnBoothClicked()
    {
        if (BoothDetails.GetComponent<BoothInfo>().SetBoothInfo(booth))
            Utilities.Transition(BoothDetails);
        else
            PopUp.Singleton.ShowError("攤位資訊設定錯誤", false);
    }
}
