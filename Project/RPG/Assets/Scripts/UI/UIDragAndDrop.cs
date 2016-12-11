using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDragAndDrop : MonoBehaviour
{
    private UIManager uiManager = null;
    private PlayerSlotData playerSlotData = null;
    private EquipmentHandler equipHandler = null;
    private UIStore uistore = null;
    private UICharater uiCharacter = null;
    private UIInquirePopup uiInquirePopup = null;
    private UICopyPopup uiCopyPopup = null;
    private UIDivisionPopup uidivPopup = null;
    public UISlotInfo uiSlotInfo = null;
    private UIClick uiClick = null;

    private UIRoot root = null;
    private Transform parent = null;
    private GameObject draggedObject = null; // 드래그중인 icon
    private GameObject tempDraggingPanel = null; // 드래그중인 panel

    public bool isDragging = false; // 드래그중인지

    void Awake()
    {
        uiManager = UIManager.Instance;
        playerSlotData = PlayerSlotData.Instance;

        equipHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<EquipmentHandler>();
        uiSlotInfo = GetComponent<UISlotInfo>();
        uiClick = GetComponent<UIClick>();
        parent = transform.parent;
        root = NGUITools.FindInParents<UIRoot>(parent);
    }

    void Start()
    {
        uiInquirePopup = uiManager.popupSettings.inquirePopup.GetComponent<UIInquirePopup>();
        uistore = uiManager.windowSettings.storeObj.GetComponent<UIStore>();
        uiCharacter = uiManager.windowSettings.characterPanel.gameObject.GetComponent<UICharater>();
        uidivPopup = uiManager.popupSettings.itemDivisionPopup.GetComponent<UIDivisionPopup>();
        uiCopyPopup = uiManager.popupSettings.copyPopup.GetComponent<UICopyPopup>();
    }

    void OnPress(bool isPress)
    {
        // TODO : 아직 버그가..
        if (!isPress && !isDragging && Application.platform == RuntimePlatform.Android)
        {
            uiClick.SetSkillDescription();
            uiClick.CheckSlotType();
        }
    }

    void OnDragStart()
    {
        // 이미 드래그래서 분리중이면 새로운 드래그 못하게.
        if (uiManager.popupSettings.itemDivisionPopup.activeSelf)
        {
            return;
        }

        // 눌린 곳에 아이템이 없으면 리턴
        if (uiSlotInfo.slotSettings.uiIcon.mainTexture == null)
        {
            return;
        }

        // 드래그 하려는 슬롯에 아이템이 없으면 리턴
        if (!uiSlotInfo.isExist)
        {
            return;
        }

        isDragging = true; // 드래그 시작
        Debug.Log(uiSlotInfo.name + " : " + uiSlotInfo.slotIndex);
        uiSlotInfo.slotSettings.uiIcon.alpha = 0.5f; // 드래그시작하면 원래있던건 반투명하게
        uiManager.tempIcon = uiSlotInfo.slotSettings.uiIcon;

        // icon 복사본 생성 - 마우스 따라 다닐거
        GameObject icon = CreateTemporary();
        draggedObject = icon;

        NGUITools.MarkParentAsChanged(this.gameObject);

        uiManager.SetSound(uiSlotInfo.slotInfo); // 드래그시작시 사운드 출력
    }

    void OnDrag(Vector2 delta)
    {
        // OnDragStart 에서 드래그 시작 안하면 리턴
        // 이 조건이 없으면 아무대나 클릭후 드래그가 된다?
        if (!isDragging)
        {
            return;
        }

        OnDragDropMove(delta * root.pixelSizeAdjustment);
    }

    void OnDragDropMove(Vector2 delta)
    {
        if (draggedObject != null)
        {
            draggedObject.transform.localPosition += (Vector3)delta;
        }
    }

    void OnDragEnd()
    {
        // 드래그 끝날때
        if (draggedObject == null)
        {
            return;
        }
        StopDragging(UICamera.hoveredObject); // 타겟 오브젝트.
    }

    void StopDragging(GameObject targetObj)
    {
        if (isDragging)
        {
            isDragging = false;
            OnDragDropRelease(targetObj);
        }
    }

    void OnDragDropRelease(GameObject targetObj)
    {
        // 드래그중에 복사한 임시 오브젝트 제거
        DestoryTemporary();
        
        // 빈 곳에 버릴겨우 targetObj는 UIRoot로 잡힘
        // 타겟이 없으면 인벤토리인경우 경고메시지(파괴할지 말지)
        // 단축키이면 제거
        if (string.Compare(targetObj.name, "UI Root") == 0)
        {
            switch (uiSlotInfo.slotType)
            {
                case TypeData.SlotType.인벤토리:
                    {
                        // 제거할지 안할지 팝업창.
                        uiManager.popupSettings.inquirePopup.SetActive(true);
                        uiInquirePopup.SetMessage(uiSlotInfo, "아이템을 버리겠습니까?");
                    }
                    break;

                case TypeData.SlotType.단축키:
                    {
                        uiSlotInfo.isCoolTime = false;
                        uiSlotInfo.coolTimer = 0f;
                        playerSlotData.RemoveSlotData(uiSlotInfo);
                    }
                    break;
            }
            uiSlotInfo.slotSettings.uiIcon.alpha = 1f;
            uiSlotInfo.ReSetting();
            return;
        }

        // 슬롯이 아닌경우 - BG인경우 Icon이 없으니깐.. - 이래 허접하게.. 음..
        if (!targetObj.transform.FindChild("Icon"))
        {
            uiSlotInfo.slotSettings.uiIcon.alpha = 1f;
            return;
        }

        UISlotInfo targetInfo = targetObj.GetComponent<UISlotInfo>(); // 타겟 정보

        SetCoolTimeInfo(targetInfo);

        CheckSlotInfo(targetInfo);

        uiSlotInfo.slotSettings.uiIcon.alpha = 1f;
    }

    private void SetCoolTimeInfo(UISlotInfo targetInfo)
    {
        float tempCoolTimer = 0f;
        bool isTempCoolTime = false;

        switch (uiSlotInfo.slotType)
        {
            case TypeData.SlotType.인벤토리:
                {
                    if (targetInfo.slotType == TypeData.SlotType.단축키)
                    {
                        targetInfo.coolTimer = uiSlotInfo.coolTimer;
                        targetInfo.isCoolTime = uiSlotInfo.isCoolTime;
                        return;
                    }
                }
                break;

            case TypeData.SlotType.단축키:
                {
                    if (targetInfo.slotType != TypeData.SlotType.단축키)
                    {
                        uiSlotInfo.coolTimer = 0f;
                        uiSlotInfo.isCoolTime = false;
                        return;
                    }
                }
                break;

            case TypeData.SlotType.스킬리스트:
                {
                    if (targetInfo.slotType == TypeData.SlotType.단축키)
                    {
                        // 단축슬롯에 같은 스킬이 있다면 그 스킬의 쿨타임정보를 가져옴
                        foreach (KeyValuePair<int, UISlotInfo> shortCut in uiManager.shortCuts)
                        {
                            if (shortCut.Value.slotInfo.skillIndex == uiSlotInfo.slotInfo.skillIndex)
                            {
                                targetInfo.coolTimer = shortCut.Value.coolTimer;
                                targetInfo.isCoolTime = shortCut.Value.isCoolTime;
                                break;
                            }
                        }
                        return;
                    }
                    else { return; }
                }

            case TypeData.SlotType.캐릭터:
            case TypeData.SlotType.창고:
            case TypeData.SlotType.상점리스트:
            case TypeData.SlotType.판매:
            case TypeData.SlotType.구매:
                return;
        }

        tempCoolTimer = uiSlotInfo.coolTimer;
        uiSlotInfo.coolTimer = targetInfo.coolTimer;
        targetInfo.coolTimer = tempCoolTimer;

        isTempCoolTime = uiSlotInfo.isCoolTime;
        uiSlotInfo.isCoolTime = targetInfo.isCoolTime;
        targetInfo.isCoolTime = isTempCoolTime;
    }

    // (인벤,창고)인경우 놓았을떄 분리 창은 같은 슬롯타입에는 안뜸 
    private void CheckSlotInfo(UISlotInfo targetInfo)
    {
        Debug.Log(uiSlotInfo.slotType);
        // 같은 아이템타입이면서 같은 아이템인덱스 : isItemIndex = true;
        // 같은 아이템타입이면서 다른 아이템인덱스 : isItemType = true, isItemIndex = false;
        // 다른 아이템타입이면                    : isItemType = false;
        bool isItemType = uiSlotInfo.slotInfo.itemType == targetInfo.slotInfo.itemType ? true : false;
        bool isItemIndex = isItemType & (uiSlotInfo.slotInfo.itemIndex == targetInfo.slotInfo.itemIndex ? true : false);

        // 스킬 -> 스킬 X
        // 상점 -> 상점 X
        // 인벤토리 -> 인벤토리 (교환) 
        //      빈 타겟 : 교체
        //      타겟있음
        //              장비인경우 : 교환
        //              같은 아이템타입, 같은 아이템인덱스 : 합치기
        //              같은 아이템타입, 다른 아이템인덱스 : 교환
        //              다른 아이템타입 : 교환
        // 창고 -> 창고 (교환)
        //      타겟있음 : 교환
        //      빈 타겟 : 교체
        // 단축키 -> 단축키
        //      타겟있음 : 교환
        //      빈 타겟 : 교체
        if (uiSlotInfo.slotType == targetInfo.slotType)
        {
            // 상점 활성화중에는 같은 슬롯끼리 드래그앤드롭 안됨
            if (uistore.gameObject.activeSelf) { return; }

            // 현재 슬롯 인덱스랑 타겟 슬롯 인덱스가 같으면 
            if (uiSlotInfo.slotIndex == targetInfo.slotIndex) { return; }
            
            switch (uiSlotInfo.slotType)
            {
                case TypeData.SlotType.인벤토리:
                    {
                        if (!targetInfo.isExist) // 빈 타겟이면
                        {
                            playerSlotData.ChangSlotData(uiSlotInfo, targetInfo);
                        }
                        else
                        {
                            // 장비이면 : 교환  - 다른 아이템타입 / 다른 인덱스(다른타입이면서) - 소모품,퀘스템
                            if (!isItemType || !isItemIndex || (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비))
                            {
                                playerSlotData.SwapSlotData(uiSlotInfo, targetInfo);
                            }
                            // 같은 아이템타입이면서 같은 아이템인덱스 : 합치기 - 소모품, 퀘스트템
                            else if (isItemIndex)
                            {
                                playerSlotData.CombineSlotData(uiSlotInfo, targetInfo, uiSlotInfo.QUANTITY_MAX);
                            }
                            else
                            {
                                Debug.Log("오류");
                            }
                        }
                    }
                    break;

                case TypeData.SlotType.단축키:
                case TypeData.SlotType.창고:
                    {
                        // 타겟이 없으면 교체
                        if (!targetInfo.isExist)
                        {
                            playerSlotData.ChangSlotData(uiSlotInfo, targetInfo);
                        }
                        // 타겟이 있으면 교환
                        else
                        {
                            playerSlotData.SwapSlotData(uiSlotInfo, targetInfo);
                        }

                        // TODO : 임시. 종료직전에 저장하는걸로 바꾸까?
                        if (uiSlotInfo.slotType == TypeData.SlotType.단축키)
                        {
                            //Network_Slot.Instance.RequestSaveSlot(TypeData.SlotType.단축키);
                        }
                    }
                    break;
            } // end swicth
        } // end if
        // 인벤토리 -> 단축창 (인벤은 그대로 단축키에는 - 복사)
        //            창고 ( 소모품, 퀘템인경우 : 같은 아이템타입, 같은 아이템인덱스 - 분리 창 On - 분리창 수량 만큼 수량검사후 합치고, 나머지는 현슬롯에 남김, 다옴기는거면 현슬롯 제거)
        //                 (                    같은 아이템타입, 다른 아이템인덱스 - 분리 창 On - 분리창 수량 만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
        //                 (                    다른 아이템타입 - 분리 창 On - 분리 창에 적은 수량만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
        //                 ( 장비 인경우 : 빈곳에 추가 - 현재 슬롯 제거)
        // 창고 -> 인벤토리 ( 소모품, 퀘템인경우 : 같은 아이템타입, 같은 아이템인덱스 - 분리 창 On - 분리창 수량 만큼 수량검사후 합치고, 나머지는 현슬롯에 남김, 다옴기는거면 현슬롯 제거)
        //                 (                    같은 아이템타입, 다른 아이템인덱스 - 분리 창 On - 분리창 수량 만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
        //                 (                    다른 아이템타입 - 분리 창 On - 분리 창에 적은 수량만큼 순차대로 빈곳에 추가후 나머지는 현재슬롯에 남김 - 다 옴기는거면 현재슬롯은 제거)
        //                 ( 장비 인경우 : 빈곳에 추가 - 현재 슬롯 제거)
        // 단축창 -> 제거
        // 스킬 -> 단축창 ( 타겟이 존재하면 교체 / 타겟이 없으면 복사 )
        // < 상점 내용은 상점스크립트로 연결 - 정산이 완료되면 주인공 데이터 변화된다 >
        // 인벤 > 판매목록(상점) ( 인벤은 수량 변화, 판매목록 : 분리창 On - 분리 수량 만큼 - 이미있으면 합치기 - 없으면 빈곳에 ) - 상점 정산클릭시 데이터 처리
        // 상점리스트 > 구매 ( 분리창On - 수량만큼 추가 / 똑같은게 있으면 합치고 다르면 빈곳에 )
        // 구매 (분리창On - 수량 감소 / 0개면 제거 )
        // 판매 (분리창On - 수량 감소,0개- 인벤에 복구 )
        // 인벤 > 케릭 / 케릭 > 인벤
        else
        {
            // 케릭슬롯에 아이템 존재하면 교환 / 없으면 교체
            if ((uiSlotInfo.slotType == TypeData.SlotType.인벤토리) && (targetInfo.slotType == TypeData.SlotType.캐릭터))
            {
                if (!uiCharacter.SetSlotInfo(uiSlotInfo, targetInfo))
                {
                    return;
                }
            }
            else if ((uiSlotInfo.slotType == TypeData.SlotType.인벤토리) && (targetInfo.slotType == TypeData.SlotType.단축키))
            {
                playerSlotData.CopySlotData(uiSlotInfo, targetInfo);
            }
            else if ((uiSlotInfo.slotType == TypeData.SlotType.인벤토리) && (targetInfo.slotType == TypeData.SlotType.판매))
            {
                // 상점스크립트로
                uiSlotInfo.slotSettings.uiIcon.alpha = 1f;
                if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비)
                {
                    uistore.CopySlotInfo(uiSlotInfo, targetInfo.slotType, 1);
                    if (!uistore.changInvenIndexs.Contains(uiSlotInfo.slotIndex))
                    {
                        uistore.changInvenIndexs.Add(uiSlotInfo.slotIndex);
                    }

                    uiSlotInfo.slotInfo.quantity = 0;
                    uiSlotInfo.isExist = false;
                    uiSlotInfo.StoreReSetting();

                    return;
                }
                // 팝업창 On
                uiManager.popupSettings.copyPopup.SetActive(true);
                uiCopyPopup.DragAndDropInfo(uiSlotInfo, targetInfo);
                return;
            }
            else
            {
                switch (uiSlotInfo.slotType)
                {
                    // 케릭>인벤 빈 곳에 추가
                    case TypeData.SlotType.캐릭터:
                        {
                            if (targetInfo.slotType == TypeData.SlotType.인벤토리)
                            {
                                if (uiSlotInfo.slotIndex == 1)
                                {
                                    equipHandler.SetWeapon(null, false);
                                    equipHandler.SetSubWeapon(null, false);
                                }

                                // 장비 수량 1
                                playerSlotData.AddSlotData(targetInfo.slotType, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, 1);
                                playerSlotData.RemoveSlotData(uiSlotInfo);
                                uiCharacter.GetComponent<UICharater>().ChangPlayerStat();
                            }
                        }
                        break;

                    case TypeData.SlotType.인벤토리:
                    case TypeData.SlotType.창고:
                        {
                            if (targetInfo.slotType == TypeData.SlotType.구매) { return; }

                            if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비)
                            {
                                playerSlotData.AddSlotData(targetInfo.slotType, uiSlotInfo.slotInfo.itemType, uiSlotInfo.slotInfo.itemIndex, 1);
                                playerSlotData.RemoveSlotData(uiSlotInfo);
                            }
                            // 소모품, 퀘스트템일경우
                            else
                            {
                                uiManager.popupSettings.itemDivisionPopup.SetActive(true);
                                uidivPopup.DragAndDropInfo(uiSlotInfo, targetInfo);

                                return;
                            }
                        }
                        break;

                    case TypeData.SlotType.단축키:
                        {
                            playerSlotData.RemoveSlotData(uiSlotInfo);
                        }
                        break;

                    case TypeData.SlotType.상점리스트:
                        {
                            if (targetInfo.slotType == TypeData.SlotType.구매)
                            {
                                if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비)
                                {
                                    uistore.CopySlotInfo(uiSlotInfo, targetInfo.slotType, 1);
                                    return;
                                }
                                // 팝업창 On
                                uiManager.popupSettings.copyPopup.SetActive(true);
                                uiCopyPopup.DragAndDropInfo(uiSlotInfo, targetInfo);
                                return;
                            }
                        }
                        break;

                    case TypeData.SlotType.스킬리스트:
                        {
                            if (targetInfo.slotType == TypeData.SlotType.단축키)
                            {
                                // 타겟이 존재 하면 교체
                                if (targetInfo.isExist)
                                {
                                    playerSlotData.ChangSlotData(uiSlotInfo, targetInfo);
                                }
                                // 타겟이 없으면 복사
                                else
                                {
                                    playerSlotData.CopySlotData(uiSlotInfo, targetInfo);
                                }
                            }
                        }
                        break;

                    case TypeData.SlotType.구매:
                    case TypeData.SlotType.판매:
                        {
                            if (targetInfo.slotType == TypeData.SlotType.단축키) { return; }

                            if (uiSlotInfo.slotInfo.itemType == TypeData.ItemType.장비)
                            {
                                uistore.CopySlotInfo(uiSlotInfo, TypeData.SlotType.없음, 1);
                                return;
                            }
                            // 팝업창 On
                            uiManager.popupSettings.copyPopup.SetActive(true);
                            uiCopyPopup.DragAndDropInfo(uiSlotInfo, null);
                            return;
                        }
                        break;
                }
            }
        }

        // 슬롯 재설정
        uiSlotInfo.ReSetting();
        targetInfo.ReSetting();
    }

    // 아이템을 드래그 할때 복제
    private GameObject CreateTemporary()
    {
        GameObject icon = Instantiate(uiSlotInfo.slotSettings.uiIcon.gameObject) as GameObject;
        icon.layer = gameObject.layer;

        // 복제한 아이콘에 TweenColor가 있으면 제거
        TweenColor cTween = icon.GetComponent<TweenColor>();
        if (cTween != null)
        {
            cTween.enabled = false;
            NGUITools.Destroy(cTween);
        }

        // 복제한 아이콘에 UITexture가 있으면 Color를 White로
        UITexture tex = icon.GetComponent<UITexture>();
        if (tex != null)
        {
            tex.color = Color.white;
        }

        if (UIDragDropRoot.root != null)
        {
            icon.transform.parent = UIDragDropRoot.root;
        }
        else
        {
            GameObject panelObj = new GameObject("_TemporaryPanel");
            panelObj.layer = gameObject.layer;
            panelObj.transform.parent = NGUITools.GetRoot(gameObject).transform;
            panelObj.transform.localScale = Vector3.one;
            panelObj.transform.localRotation = Quaternion.identity;
            panelObj.transform.localPosition = Vector3.one;

            tempDraggingPanel = panelObj;
            uiManager.tempDraggingPanel = panelObj;

            UIPanel panel = panelObj.AddComponent<UIPanel>();
            panel.depth = UIPanel.nextUnusedDepth;
            icon.transform.parent = panelObj.transform;
        }

        // 현재 마우스 위치에로 설정
        icon.transform.position = NGUITools.FindCameraForLayer(gameObject.layer).ScreenToWorldPoint(Input.mousePosition);
        icon.transform.rotation = transform.rotation;
        icon.transform.localScale = transform.localScale;

        return icon;
    }

    private void DestoryTemporary()
    {
        // 드래그중 / UI모드 해제시 - 복사한 임시 오브젝트 제거
        if (tempDraggingPanel != null)
        {
            NGUITools.Destroy(tempDraggingPanel);
        }
        else if (draggedObject != null)
        {
            Debug.Log("mDraggedObject");
            NGUITools.Destroy(draggedObject);
        }
    }
}