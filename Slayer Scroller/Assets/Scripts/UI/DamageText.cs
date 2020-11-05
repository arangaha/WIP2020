using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DamageText : MonoBehaviour
{
    float opacity = 1;
    float xforce = 0;
    float yforce = 0;
  [SerializeField]  TextMeshProUGUI text;

    void Start()
    {
        StartCoroutine(Decay());
    }

    /// <summary>
    /// initialize the text
    /// </summary>
    /// <param name="number">positive if taking damage, negative if healing</param>
    public void InitializeText(float number, bool bleed)
    {
        if (number != 0)
        {
            text = GetComponent<TextMeshProUGUI>();

            if(bleed)
            {
                text.color = Color.red;
                text.text = "" + (int)number;
                text.fontSize = 18;
            }
            else if (number > 0)
            {
                text.color = Color.white;
                text.text = "" + (int)number;
                //Debug.Log("ouch");
            }
            else
            {
                text.color = Color.green;
                text.text = "" + (int)-number;
                text.fontSize = 18;
               // Debug.Log("heal");
            }
            xforce = Random.Range(-2, 2);
            yforce = Random.Range(-2, 2);
            GetComponent<Rigidbody2D>().velocity =  (new Vector2(xforce, yforce));
        }


    }
     IEnumerator Decay()
    {
        float r = text.color.r;
        float g = text.color.g;
        float b = text.color.b;
        while (true)
        {
            opacity -= 0.03f;
            text.color = new Color(r, g, b, opacity);
            if (opacity <= 0)
            {
                Destroy(gameObject);
            }
            yield return new WaitForSeconds(0.01f);

        }
    }
}
