using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 200f;                //максиальное здоровье
    [SerializeField] private float soulCoefficient = 39.2f;         //коэффицент души
    [SerializeField] private float physicalDmgResistance = 0.35f;   //физическое сопротивление
    [SerializeField] private float soulDmgResistance = 0.44f;       //сопротивление души

    PlayerAttack playerAttack;

    private float currentHealth;    //текущие здоровье персонажа
    private float currentSoul;      //текущая целостность души

    private void Start()
    {
        currentHealth = maxHealth + (maxHealth * soulCoefficient);  //текущие здоровье персонажа = максиальное здоровье + (максиальное здоровье * коэффицент души)
        currentSoul = soulCoefficient;                              //текущая целостность души = коэффиценту души
    }
    //Урон по душе
    public void TakeSoulDamage(int damage)
    {
        currentSoul -= (soulDmgResistance * damage);                //из текущей целостности души вычитаем сопротивление души * на урон
        currentHealth = maxHealth + (maxHealth * currentSoul);      //текущие здоровье равно максимальному здоровью * на текущую целостность души
    }
    //Урон по здоровью
    public void TakeDamage(int damage)
    {
        currentHealth -= (damage * physicalDmgResistance);

        if (currentHealth <= 0)  //если текущие здоровье меньше или равно нулю - юнит умирает
        {
            Die();
        }
    }
    //Смерть юнита
    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
