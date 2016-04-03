using UnityEngine;
using System.Collections;

public class LiquidTile : MonoBehaviour {

    Element element;
    int frozenState = 0;
    int numSpritesPerBiome = 3;
    SpriteRenderer sprenderer;

    public void SetElement(Element elt)
    {
        element = elt;
        sprenderer = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Enemy")
        {
            Actor actor = col.GetComponent<Actor>();
            if (actor.element == Element.Ice)
            {
                FreezeOver();
            }
            else
            {
                Crack();
                if (frozenState == 0)
                {
                    actor.slipping = false;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Enemy")
        { 
            Actor actor = col.GetComponent<Actor>();
            if (frozenState == 0) // if unfrozen
            {
                switch (element)
                {
                    case Element.Fire:
                        actor.Burn(1);
                        break;

                    case Element.Ice:
                        actor.Freeze(0.5f);
                        break;
                }
                if (actor.element != element) actor.Slow();
            }
            else // if frozen (partially or fully)
            {
                switch (element)
                {
                    case Element.Fire:
                        break;

                    case Element.Ice:
                        //reduce mobility
                        actor.slipping = true;
                        break;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" || col.tag == "Enemy")
        {
            Actor actor = col.GetComponent<Actor>();
            actor.UnSlow();
            // actor.slipping = false;
        }
    }

    public void FreezeOver()
    {
        frozenState = 2;
        UpdateSprite();
    }

    void Crack()
    {
        if (frozenState > 0)
        {
            frozenState--;
        }
        UpdateSprite();
    }

    void UpdateSprite()
    {
        sprenderer.sprite = GameManager.S.liquidTileSprites[(int)element * numSpritesPerBiome + frozenState];
    }
}
