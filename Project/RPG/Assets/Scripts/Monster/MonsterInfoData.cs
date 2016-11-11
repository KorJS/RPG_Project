using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MonsterMovement))]
[RequireComponent(typeof(MonsterState))]
[RequireComponent(typeof(MonsterRange))]
public class MonsterInfoData : MonoBehaviour
{
    // TODO : 몬스터 스폰될때 몬스터 정보을 이 스크립트에 저장 및 수치계산 관리
    public MonsterData.MonsterInfo monsterInfo;
    public List<MonsterData.MonsterSkillInfo> monsterSkillInfos;
    private MonsterMovement monsterMovemnet = null;
    private MonsterState monsterState = null;
    private ItemManager itemManager = null;

    public struct DropItem
    {
        public TypeData.ItemType itemType;
        public int itemIndex;
        public int quantity;
    }

    public DropItem dropItem;

    public Dictionary<int, DropItem> dropItems = null;

    private Transform monsterT;

    public float currentHP = 0f;
    public bool isDeath = false;

    void Awake()
    {
        dropItems = new Dictionary<int, DropItem>();
        monsterSkillInfos = new List<MonsterData.MonsterSkillInfo>();
        monsterT = transform;
        monsterMovemnet = GetComponent<MonsterMovement>();
        monsterState = GetComponent<MonsterState>();
    }

    void Start()
    {
        itemManager = ItemManager.Instance;
    }

    void OnEnable()
    {
        currentHP = monsterInfo.hp;
    }

    void Update()
    {
        Debug.Log("monsterSkillInfos : " + monsterSkillInfos.Count);
        if (isDeath)
        {
            itemManager.CreateDropItem(monsterT, monsterInfo, dropItems);

            // 주인공 경험치 습득
            PlayerInfoData.Instance.SetExp(monsterInfo.exp);

            // 상태 변화(죽음)
            monsterState.nextState = TypeData.MonsterState.죽음;
            // 애니메이션 설정
            monsterMovemnet.animator.SetTrigger(monsterMovemnet.animationSettings.isDeathTrigger);
            StartCoroutine(monsterMovemnet.Death(10f));
            
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