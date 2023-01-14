using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillBar;
    public float health;


    public void LoseHealth(int value)
    {
        if (health <= 0)
            return;
        health -= value;
        
        fillBar.fillAmount = health / 10;
        
        if (health <= 0)
        {
           
            FindObjectOfType<Player>().DiePlayer();     
        }
        
    }
   
}
