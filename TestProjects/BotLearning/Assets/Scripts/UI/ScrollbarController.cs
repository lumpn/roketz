using UnityEngine;
using UnityEngine.UI;

public sealed class ScrollbarController : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private FloatObject value;
    [SerializeField] private FloatObject size;

    void Update()
    {
        scrollbar.value = value.Fraction;
        scrollbar.size = size.Fraction;
    }
}
