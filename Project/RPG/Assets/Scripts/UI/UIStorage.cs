using UnityEngine;
using System.Collections;

public class UIStorage : MonoBehaviour
{
    private PlayerInfoData playerInfoData = null;
    public UILabel storageGold = null;

    private bool isDeposit = false;

    void Awake()
    {
        playerInfoData = PlayerInfoData.Instance;
        storageGold = transform.FindChild("Gold").FindChild("Amount").GetComponent<UILabel>();
    }

    void OnEnable()
    {
        SetStorageGold();
    }

    public void SetStorageGold()
    {
        storageGold.text = playerInfoData.infoData.storageGold.ToString();
    }

    public void DepositBtn()
    {
        isDeposit = true;

        GameObject divObj = UIManager.Instance.popupSettings.goldDivisionPopup;
        divObj.SetActive(true);
        divObj.GetComponent<UIDivisionPopup>().DepositAndWithdraw(isDeposit);
    }

    public void WithdrawBtn()
    {
        isDeposit = false;

        GameObject divObj = UIManager.Instance.popupSettings.goldDivisionPopup;
        divObj.SetActive(true);
        divObj.GetComponent<UIDivisionPopup>().DepositAndWithdraw(isDeposit);
    }

    public void CloseWindows()
    {
        gameObject.SetActive(false);
    }
}
