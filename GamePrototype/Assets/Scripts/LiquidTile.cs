using UnityEngine;
using System.Collections;

public class LiquidTile : MonoBehaviour {

    Element element;

    void Start()
    {
        SetElement(Element.Ice);
    }

    public void SetElement(Element elt)
    {
        element = elt;
        GetComponent<SpriteRenderer>().sprite = GameManager.S.liquidTileSprites[(int)element];
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Enemy")
        { 
            Actor actor = col.GetComponent<Actor>();
            switch (element)
            {
                case Element.Fire:
                    actor.Burn(1);
                    break;

                case Element.Ice:
                    actor.Freeze(1);
                    break;
            }
            actor.Slow();
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Enemy")
        {
            Actor actor = col.GetComponent<Actor>();

            actor.UnSlow();
        }
    }
}
