using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject player;

    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerMoneyText;
    // Start is called before the first frame update
    void Start()
    {
        playerHealthText.text = "";
        playerMoneyText.text = "";
        playerHealthText.color= Color.red;
        playerMoneyText.color=Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthText.text="HP: "+player.GetComponent<PlayerController>().GetHealth().ToString();
        playerMoneyText.text = "Money: " +player.GetComponent<PlayerController>().GetMoney().ToString();
    }
}
