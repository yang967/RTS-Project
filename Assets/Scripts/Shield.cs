using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    float rate;
    bool set;
    int type;
    int hp;
    float time;
    float start;
    GameObject parent;

    private void Awake()
    {
        rate = 1;
        set = false;
        type = -1;
        hp = 0;
        time = -2;
    }

    private void FixedUpdate()
    {
        if (time == -2)
            return;
        if (Time.time - start > time)
            Destroy();
        if(parent)
            transform.position = parent.transform.position;
    }

    public void Set(int type, int hp, float rate, float size, GameObject parent)
    {
        this.type = type;
        this.rate = rate;
        this.hp = hp;
        transform.localScale = new Vector3(size * 2, size * 2 < 30 ? size * 2 : transform.localScale.y, size * 2);
        set = true;
        start = Time.time;
        this.parent = parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!set)
            return;
        Bullet b;
        if(other.TryGetComponent<Bullet>(out b))
        {
            switch(type)
            {
                case 0:
                    int current_damage = b.getDamage();
                    b.SetDamage(rate);
                    hp -= current_damage - b.getDamage();
                    break;
                case 1:
                    b.hit();
                    hp -= b.getDamage();
                    break;
            }

            if (hp <= 0)
                Destroy();
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

}
