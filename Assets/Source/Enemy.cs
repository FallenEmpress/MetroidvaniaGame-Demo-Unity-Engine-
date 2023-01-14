using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character, IDamageable
{

    

    public virtual void ApplyDamage(float amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth <= 0)
        {
            
            gameObject.GetComponent<EnemyAI>().enabled = false;
            Die();
            

        }
    }

   


}
