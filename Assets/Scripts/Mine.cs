using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    int group;
    int damage;
    int armorPenetration;
    int[] damageType;

    // Start is called before the first frame update
    void Start()
    {
        group = -1;
        damage = -1;
        armorPenetration = -1;
        damageType = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setProperty(int group, int damage, int armorPenetration, int[] damageType)
    {
        if (this.group != -1 || damage != -1 || armorPenetration != -1 || damageType != null)
            return;
        this.group = group;
        this.damage = damage;
        this.armorPenetration = armorPenetration;
        this.damageType = new int[4];
        damageType.CopyTo(this.damageType, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != 3)
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isFlyUnit())
            return;
        if (other.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getGroup() != group)
            detonate();
    }

    void detonate()
    {
        if (group == -1)
        {
            Destroy(gameObject, 1);
            throw new System.NullReferenceException("Mine group is not set");
        }
        if (damage == -1)
            damage = 0;
        if (armorPenetration == -1)
            armorPenetration = 0;
        if (damageType == null)
            damageType = new int[] { 0, 0, 0, 0 };
        transform.GetChild(1).GetComponent<Detonate>().detonate(damage, armorPenetration, damageType);
        Destroy(gameObject);
    }
}
