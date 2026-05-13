using UnityEngine;
using UnityEngine.UI;

public class SpriteNumberDisplay : MonoBehaviour
{
    [Header("Sprite set 0-9")]
    [SerializeField] private Sprite[] digitSprites;

    [Header("Digits objects on stage")]
    [SerializeField] private Image[] digitSlots;

    public void UpdateDisplay(int number)
    {
        int maxNumber = (int)Mathf.Pow(10, digitSlots.Length) - 1;
        number = Mathf.Clamp(number, 0, maxNumber);

        for (int i = 0; i < digitSlots.Length; i++)
        {
            int digit = number % 10;

            digitSlots[digitSlots.Length - 1 - i].sprite = digitSprites[digit];

            number /= 10;
        }
    }

    public void SetColor(Color newColor)
    {
        foreach (Image slot in digitSlots)
        {
            if (slot != null)
            {
                slot.color = newColor;
            }
        }
    }
}