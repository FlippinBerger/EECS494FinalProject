using UnityEngine;
using System.Collections;

public struct AttackHitInfo
{
    public AttackHitInfo(int d, float kV, float kD, GameObject go)
    {
        damage = d;
        knockbackVelocity = kV;
        knockbackDuration = kD;
        element = Element.None;
        elementalPower = 1;
        source = go;
    }
    public AttackHitInfo(int d, float kV, float kD, Element elt, int ep, GameObject go)
    {
        damage = d;
        knockbackVelocity = kV;
        knockbackDuration = kD;
        element = elt;
        elementalPower = ep;
        source = go;
    }
    public int damage;
    public float knockbackVelocity;
    public float knockbackDuration;
    public Element element;
    public int elementalPower;
    public GameObject source;
}

public abstract class Weapon : MonoBehaviour {
    public string weaponName;
    public Element element;
    public bool isSpell = false; // isSpell determines whether the weapon gets bound
                                 // to the right trigger or the left trigger
                                 // (defensive abilities are called "spells" for now)
    public int manaCost = 0;
    public int damagePerLevel = 1;
    public int minDamage = 1;
    public int maxDamage = 3;
    public float minKnockbackVelocity = 3f;
    public float maxKnockbackVelocity = 5f;
    public float minKnockbackDuration = 0.1f;
    public float maxKnockbackDuration = 0.5f;
    public float cooldown = 1f; // the cooldown between attacks
    public float chargeTime = 1f;
    public Sprite icon; // the icon that represents this weapon

    protected Player owner; // the player associated with this weapon
    protected AttackHitInfo hitInfo;
    int upgradeLevel = 1;
    
    // Use this for initialization
    protected virtual void Start () {
        // this.parentPlayer = this.transform.parent.gameObject.GetComponent<Player>(); // set the parent player
        // if (parentPlayer == null) print("parent player null in " + gameObject + " Start()");
        // ResetAttack();
    }

    public virtual void SetOwner(Player player)
    {
        ResetAttack();
        if (player == null)
        {
            transform.parent = null;
            return;
        }
        owner = player;
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0, 0.8f);
    }

    public virtual void ResetAttack()
    {
        gameObject.SetActive(false);
    }

    public void SetElement(Element elt)
    {
        element = elt;
        GetComponent<SpriteRenderer>().color = GameManager.S.elementColors[(int)elt];
    }

    public void Upgrade()
    {
        owner.EnqueueFloatingText("Upgrade!", Color.green);
        ++upgradeLevel;
        switch (upgradeLevel)
        {
            case 2:
                UpgradeLevel2();
                break;
            case 3:
                UpgradeLevel3();
                break;
            case 4:
                UpgradeLevel4();
                break;
            default:
                UpgradePast4();
                break;
        }

        owner.UpdateWeaponIcon(this);
    }

    virtual protected void UpgradeLevel2()
    {
        minDamage += damagePerLevel;
        maxDamage += damagePerLevel;
    }
    virtual protected void UpgradeLevel3()
    {
        minDamage += damagePerLevel;
        maxDamage += damagePerLevel;
    }
    virtual protected void UpgradeLevel4()
    {
        minDamage += damagePerLevel;
        maxDamage += damagePerLevel;
    }
    virtual protected void UpgradePast4()
    {
        minDamage += damagePerLevel;
        maxDamage += damagePerLevel;
    }

    public int GetUpgradeLevel()
    {
        return upgradeLevel;
    }

    abstract public void Fire(float attackPower);

    virtual protected AttackHitInfo DetermineHitStrength(float attackPower)
    {
        if (owner == null) print("parent player null in " + gameObject + " DetermneHitIfnos");
        int damage = minDamage + (int)((maxDamage - minDamage) * attackPower);
        float knockbackVelocity = minKnockbackVelocity + ((maxKnockbackVelocity - minKnockbackVelocity) * attackPower);
        float knockbackDuration = minKnockbackDuration + ((maxKnockbackDuration - minKnockbackDuration) * attackPower);
        return new AttackHitInfo(damage, knockbackVelocity, knockbackDuration, element, owner.elementalLevel, owner.gameObject);
    }
}
