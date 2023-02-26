using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    GameObject Target;
    Vector3 target;
    int damage;
    bool destroy = false;
    int[] antiType = null;
    int armorpenetration;
    Effect effect = null;

    // Update is called once per frame
    void Update()
    {
        if (destroy)
            return;
        Vector3 pos = transform.position;
        if (Mathf.Abs(pos.x - target.x) < 0.5f && Mathf.Abs(pos.z - target.z) < 0.5f && gameObject.name.Equals("BioMissle(Clone)"))
            Detonate();
        else if (pos.y < target.y && gameObject.name.Equals("BioMissle(Clone)"))
            Detonate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destroy)
            return;
        Detonate detonate;
        if(Target != null && other.gameObject.Equals(Target))
        {
            if (transform.GetChild(0).TryGetComponent<Detonate>(out detonate))
            {
                if (effect != null && effect.getEffectType() != -1)
                    detonate.setEffect(effect);
                detonate.detonate(damage, armorpenetration, antiType);
            }
            else
                Target.transform.GetChild(0).GetComponent<UnitLoad>().setHitPoint(damage * 10 + 2, armorpenetration, antiType);
            hit();
            //Detonate();
        }
        else if(other.gameObject.Equals(Terrain.activeTerrain.gameObject))
        {
            /*if (transform.GetChild(0).TryGetComponent<Detonate>(out detonate))
            {
                if (effect != null && effect.getEffectType() != -1)
                    detonate.setEffect(effect);
                detonate.detonate(damage, armorpenetration, antiType);
            }
            Destroy(transform.GetChild(0).transform.gameObject);
            Destroy(gameObject, 3f);
            destroy = true;
            gameObject.GetComponent<ProjectileMovement>().Set();
            if (transform.childCount == 3)
            {
                transform.GetChild(2).gameObject.SetActive(true);
            }*/
            Detonate();
        }
    }

    public void hit()
    { 
        ParticleSystem ps;
            if (transform.GetChild(0).TryGetComponent<ParticleSystem>(out ps))
                ps.Stop();
            else if (transform.GetChild(0).childCount > 0)
            {
                foreach (Transform tr in transform.GetChild(0))
                    if (tr.TryGetComponent<ParticleSystem>(out ps))
                        ps.Stop();
            }
            else
                Destroy(transform.GetChild(0).transform.gameObject);    
            Destroy(gameObject, 3f);
            Destroy(transform.GetComponent<Collider>());
            Destroy(transform.GetComponent<Rigidbody>());
            destroy = true;
            gameObject.GetComponent<ProjectileMovement>().Set();
            if (transform.childCount >= 3)
            {
                transform.GetChild(2).gameObject.SetActive(true);
            }
    }

    void Detonate()
    {
        Detonate detonate;
        if (transform.GetChild(0).TryGetComponent<Detonate>(out detonate))
        {
            if (effect != null && effect.getEffectType() != -1)
                detonate.setEffect(effect);
            detonate.detonate(damage, armorpenetration, antiType);
        }
        Destroy(transform.GetChild(0).transform.gameObject);
        Destroy(gameObject, 3f);
        Destroy(transform.GetComponent<Collider>());
        Destroy(transform.GetComponent<Rigidbody>());
        destroy = true;
        gameObject.GetComponent<ProjectileMovement>().Set();
        if (transform.childCount >= 3)
        {
            transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void Attack(GameObject target, GameObject from, int damage, int armorpenetration, float accuracy, int[] antitype)
    {
        if (gameObject.GetComponent<MissleMovement>() == null)
            transform.eulerAngles = from.transform.eulerAngles;
        else
            transform.eulerAngles = from.transform.eulerAngles + new Vector3(Random.Range(0, 90), Random.Range(-90, 90), 0);
        this.armorpenetration = armorpenetration;
        antiType = antitype;
        Target = target;
        this.damage = damage;
        ProjectileMovement movement = gameObject.GetComponent<ProjectileMovement>();
        movement.setTarget(Target, accuracy);
        this.target = movement.getTarget();
        Destroy(gameObject, 10f);
    }

    public void Attack(Vector3 target, GameObject from, int damage, int armorpenetration, float accuracy, int[] antitype)
    {
        if (gameObject.GetComponent<MissleMovement>() == null)
            transform.eulerAngles = from.transform.eulerAngles;
        else
            transform.eulerAngles = from.transform.eulerAngles + new Vector3(Random.Range(0, 90), Random.Range(-90, 90), 0);
        this.armorpenetration = armorpenetration;
        antiType = antitype;
        this.target = target;
        this.damage = damage;
        ProjectileMovement movement = gameObject.GetComponent<ProjectileMovement>();
        movement.setTarget(target, accuracy);
        this.target = movement.getTarget();
        Destroy(gameObject, 10f);
    }

    public void setEffect(Effect e)
    {
        if (effect != null)
            return;
        effect = new Effect(e);
    }

    public Effect getEffect()
    {
        return effect;
    }

    public void SetDamage(float rate)
    {
        damage = (int)(damage * rate);
    }

    public int getDamage()
    {
        return damage;
    }
}
