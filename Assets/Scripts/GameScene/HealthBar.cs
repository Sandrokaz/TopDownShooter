
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider Slider;
    [SerializeField] private EntityBase entityBase;
    [SerializeField] private Image image;



    private void Start()
    {
        Slider.maxValue = entityBase.Health;
        Slider.value = entityBase.Health;
    }
    public void SetHealth(float health)
    {

        Slider.value = health;

    }
    // image.color = Color.Lerp(low, High, Slider.normalizedValue);


}