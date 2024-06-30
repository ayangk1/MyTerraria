using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    public TextMeshProUGUI healthText;
    public GameObject healthBar;

    public void OnHealthChange(Character character)
    {
        healthText.text = "生命:" + character.currHealth + "/" + character.maxHealth;
        if (character.currHealth <= 200f)
        {
            healthBar.transform.GetChild(0).gameObject.SetActive(true);
            healthBar.transform.GetChild(1).gameObject.SetActive(false);
            if (character.currHealth % 20 != 0)
                healthBar.transform.GetChild(0).GetChild(0).localScale = Vector3.one * (character.currHealth % 20) / 20;
            else
                healthBar.transform.GetChild(0).GetChild(0).localScale = Vector3.one;
            for (int i = 0; i < 10; i++)
            {
                if (i < Mathf.CeilToInt(character.currHealth / 20))
                    healthBar.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
                else
                    healthBar.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
            }
        }
        else if (character.currHealth > 200f && character.currHealth <= 400f)
        {
            healthBar.transform.GetChild(0).gameObject.SetActive(true);
            healthBar.transform.GetChild(1).gameObject.SetActive(true);
            if (character.currHealth % 20 != 0)
                healthBar.transform.GetChild(1).GetChild(0).localScale = Vector3.one * (character.currHealth % 20) / 20;
            else
                healthBar.transform.GetChild(1).GetChild(0).localScale = Vector3.one;
            for (int i = 0; i < 10; i++)
            {
                if (i < Mathf.CeilToInt(character.currHealth / 20) - 10)
                    healthBar.transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
                else
                    healthBar.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
