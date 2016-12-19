using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(MonsterState))]
[RequireComponent(typeof(MonsterRange))]
public class MonsterInfoData : MonoBehaviour
{
    // TODO : 몬스터 스폰될때 몬스터 정보을 이 스크립트에 저장 및 수치계산 관리
    public  MonsterData.MonsterInfo     monsterInfo     = null; // 몬스터 정보
    private MonsterMovement             monsterMovemnet = null; // 몬스터 동작
    private MonsterState                monsterState    = null; // 몬스터 상태
    private PlayerEffect                playerEffect    = null; // 주인공 이펙트(어그로)
    private ItemManager                 itemManager     = null; // 아이템 매니저(드랍아이템)

    public Dictionary<int, MonsterData.MonsterSkillInfo> monsterSkillInfos; // 몬스터 스킬 정보

    // 드랍 아이템 정보
    public struct DropItem
    {
        public TypeData.ItemType itemType;
        public int itemIndex;
        public int quantity;
    }

    public DropItem     dropItem;

    public Dictionary<int, DropItem> dropItems = null;  // 드랍아이템 정보

    private Transform   monsterT    = null;     // 몬스터
    public  Transform   parentT     = null;     // 몬스터 스폰위치

    public float        currentHP   = 0f;       // 몬스터 현재 HP
    public bool         isDeath     = false;    // 몬스터 죽었는지 여부

    void Awake()
    {
        monsterSkillInfos   = new Dictionary<int, MonsterData.MonsterSkillInfo>();
        dropItems           = new Dictionary<int, DropItem>();
        monsterT            = transform;
        monsterMovemnet     = GetComponent<MonsterMovement>();
        monsterState        = GetComponent<MonsterState>();
    }

    void Start()
    {
        itemManager     = ItemManager.Instance;

        playerEffect    = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerEffect>();
        parentT         = transform.parent;
    }

    // 리젠되서 활성화가 되면
    void OnEnable()
    {
        currentHP = monsterInfo.hp;
        monsterState.nextMode = TypeData.MODE.평화;
        monsterState.nextState = TypeData.MonsterState.대기;
    }

    void Update()
    {
        if (isDeath)
        {
            itemManager.CreateDropItem(monsterT, monsterInfo, dropItems);

            // 주인공 골드 습득 및 인벤골드표시 갱신
            PlayerInfoData.Instance.infoData.gold += monsterInfo.gold;
            UIManager.Instance.SetGoldLabel(false);
            // 주인공 경험치 습득
            if (PlayerInfoData.Instance.SetExp(monsterInfo.exp))
            {
                playerEffect.CheckActiveEffect(playerEffect.effectSettings.levelup, true);
            }

            // 상태 변화(죽음)
            monsterState.nextState = TypeData.MonsterState.죽음;
            // 애니메이션 설정
            monsterMovemnet.animator.SetTrigger(monsterMovemnet.animationSettings.isDeathTrigger);
            StartCoroutine(monsterMovemnet.Death(5f));
            
            // HP 비활성화
            ResetHpBar();

            // 레이어 바꿔서 주인공 타겟에서 제외. (스폰될때 Monster로 레이어 다시 변경)
            gameObject.layer = LayerMask.NameToLayer("Default");
            isDeath = false;
        }
    }

    // 드랍 아이템 정보
    public void CheckDropItem()
    {
        Debug.Log("type : " + monsterInfo.itemType + " index : " + monsterInfo.itemIndex + " quantity : " + monsterInfo.quantity);

        string[] itemTypes = monsterInfo.itemType.Split(',');
        string[] itemIndex = monsterInfo.itemIndex.Split(',');
        string[] quantity = monsterInfo.quantity.Split(',');

        // 몬스터 드랍아이템 정보
        for (int i = 0; i < itemTypes.Length; i++)
        {
            dropItem.itemType = (TypeData.ItemType)int.Parse(itemTypes[i]);
            dropItem.itemIndex = int.Parse(itemIndex[i]);

            int tempQuantity = 1;

            if (i < quantity.Length)
            {
                tempQuantity = int.Parse(quantity[i]);
            }

            dropItem.quantity = tempQuantity;
            dropItems.Add(i, dropItem);
        }
    }

    // 현재 체력 설정
    public void SetCurrentHP(float hp)
    {
        if (hp < 0)
        {
            hp /= monsterInfo.def;
        }

        currentHP += hp;

        if (currentHP <= 0)
        {
            isDeath = true;
        }
    }

    // 체력바 처음상태로 설정
    private void ResetHpBar()
    {
        switch ((TypeData.MonsterType)monsterInfo.monsterType)
        {
            case TypeData.MonsterType.일반:
            case TypeData.MonsterType.정예:
                {
                    if (UIManager.Instance.mobHpBarObj.activeSelf)
                    {
                        UIManager.Instance.mobHpBarObj.SetActive(false);
                    }
                }
                break;

            case TypeData.MonsterType.보스:
                {
                    if (UIManager.Instance.bossHpBarObj.activeSelf)
                    {
                        UIManager.Instance.bossHpBarObj.SetActive(false);
                    }
                }
                break;
        }
    }

    // 평화상태로 돌아가면 위치 체력, 체력바 처음상태로 설정
    public void Reset(bool isDie)
    {
        if (!isDie)
        {
            ResetHpBar();
        }
        monsterT.localPosition = Vector3.zero;
        currentHP = monsterInfo.hp;
    }
}