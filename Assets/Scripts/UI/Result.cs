using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Result : MonoBehaviour {

    public Image[] images = new Image[3];
    public Text[] texts = new Text[3];

    [HideInInspector]
    public int score = 0;

    public void ShowIt(bool showIt)
    {
        transform.GetComponent<Image>().enabled = showIt;

        for (int i = 0; i < texts.Length; i++)
        {
            try
            {
                images[i].enabled = showIt;
                texts[i].enabled = showIt;
            }
            catch (System.Exception)
            {
                texts[i].enabled = showIt;
            }
        }

        if (showIt)
        {
            if (score < 0) score = 0;

            texts[1].text = score.ToString();
            texts[1].enabled = false;
            StartCoroutine(FillStar(0, (double)(score/900.0)));
        }
    }

    IEnumerator FillStar(int index, double amount)
    {
        for (double i = 0.00; i <= amount; i += 0.01)
        {
            images[index].fillAmount = (float)(i);

            if (images[index].fillAmount >= 1)
            {
                transform.GetComponent<AudioSource>().Play();
                break;
            }

            yield return null;
        }

        index++;
        amount = amount - 1;

        if (index == 3 || amount <= 0)
        {
            texts[1].enabled = true;
            yield break;
        }
        else
        {
            StartCoroutine(FillStar(index, amount));
        }

        yield break;
    }
}
