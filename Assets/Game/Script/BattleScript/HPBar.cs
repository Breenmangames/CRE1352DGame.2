using UnityEngine;

public class HPBar : MonoBehaviour
{

    [SerializeField] GameObject health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public void SetHP(float hpNormalised)
    {
        health.transform.localScale = new Vector3(hpNormalised / 100f, 1f);
    }
}
