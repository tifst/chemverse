using UnityEngine;

public class ReactorTank : MonoBehaviour
{
    [Header("Product Info")]
    public string productName = "";     // contoh: H2O, NaCl
    public string productCategory = ""; // contoh: Gas, Fluid, Solid
    public int quantity = 0;

    public void Clear()
    {
        productName = "";
        productCategory = "";
        quantity = 0;
    }
}