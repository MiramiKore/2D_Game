using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 200f;                //����������� ��������
    [SerializeField] private float soulCoefficient = 39.2f;         //���������� ����
    [SerializeField] private float physicalDmgResistance = 0.35f;   //���������� �������������
    [SerializeField] private float soulDmgResistance = 0.44f;       //������������� ����

    PlayerAttack playerAttack;

    private float currentHealth;    //������� �������� ���������
    private float currentSoul;      //������� ����������� ����

    private void Start()
    {
        currentHealth = maxHealth + (maxHealth * soulCoefficient);  //������� �������� ��������� = ����������� �������� + (����������� �������� * ���������� ����)
        currentSoul = soulCoefficient;                              //������� ����������� ���� = ����������� ����
    }
    //���� �� ����
    public void TakeSoulDamage(int damage)
    {
        currentSoul -= (soulDmgResistance * damage);                //�� ������� ����������� ���� �������� ������������� ���� * �� ����
        currentHealth = maxHealth + (maxHealth * currentSoul);      //������� �������� ����� ������������� �������� * �� ������� ����������� ����
    }
    //���� �� ��������
    public void TakeDamage(int damage)
    {
        currentHealth -= (damage * physicalDmgResistance);

        if (currentHealth <= 0)  //���� ������� �������� ������ ��� ����� ���� - ���� �������
        {
            Die();
        }
    }
    //������ �����
    void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
