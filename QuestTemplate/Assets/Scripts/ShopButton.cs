using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour
{
    private bool owned = false;

    public GameObject model;
    public int price = 0;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    // Start is called before the first frame update
    void Start()
    {
        // Create the 3D player object to show above button
        var newObj = Instantiate(model);
        newObj.transform.SetParent(buyButton.transform);
        newObj.transform.localScale = new Vector3(50, 50, 50);
        newObj.transform.position = transform.position;

        // Set Player Object inactive and disable physics
        var playerController = newObj.GetComponent<PlayerController>();
        playerController.enabled = false;

        var rb = newObj.GetComponent<Rigidbody>();
        rb.useGravity = false;

        priceText.text = "Price: $" + price.ToString();
    }

}
