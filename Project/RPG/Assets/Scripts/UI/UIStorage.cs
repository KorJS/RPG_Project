using UnityEngine;
using System.Collections;

public class UIStorage : MonoBehaviour
{
    private PlayerInfoData  playerInfoData  = null; // 주인공 정보
    private UIPanel         storagePanel    = null; // 창고 판넬
    public  UILabel         storageGold     = null; // 창고 보관금

    private bool isDeposit = false;

    void Awake()
    {
        playerInfoData  = PlayerInfoData.Instance;
        storagePanel    = GetComponent<UIPanel>();
        storageGold     = transform.FindChild("Gold").FindChild("Amount").GetComponent<UILabel>();
    }

    void OnEnable()
    {
        SetStorageGold();
    }

    // 보관금 설정
    public void SetStorageGold()
    {
        storageGold.text = playerInfoData.infoData.storageGold.ToString();
    }

    // 입금
    public void DepositBtn()
    {
        isDeposit = true;

        GameObject divObj = UIManager.Instance.popupSettings.goldDivisionPopup;
        divObj.SetActive(true);
        divObj.GetComponent<UIDivisionPopup>().DepositAndWithdraw(isDeposit);
    }

    // 출금
    public void WithdrawBtn()
    {
        isDeposit = false;

        GameObject divObj = UIManager.Instance.popupSettings.goldDivisionPopup;
        divObj.SetActive(true);
        divObj.GetComponent<UIDivisionPopup>().DepositAndWithdraw(isDeposit);
    }

    public void CloseWindows()
    {
        storagePanel.alpha = 0f;
    }
}
