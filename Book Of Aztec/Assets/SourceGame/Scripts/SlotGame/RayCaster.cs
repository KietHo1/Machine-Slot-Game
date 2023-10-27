using UnityEngine;

public class RayCaster : MonoBehaviour
{
    public int ID { get; set; } // for calcs

    public Symbol GetSymbol()
    {
        Collider2D hit = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y));
        if (hit) { return hit.GetComponent<Symbol>(); }
        else return null;
    }

}