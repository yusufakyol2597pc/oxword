using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAds : CPopUp
{
    public override void Open()
    {
        gameObject.SetActive(true);
    }

    public override void Close()
    {
        gameObject.SetActive(false);
    }

    public void BuyRemoveAds()
    {
        Logger.Log("Ads removed.");
    }
}
