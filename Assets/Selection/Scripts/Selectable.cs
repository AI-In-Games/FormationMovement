using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer m_SelectionSprite;


    private void Start()
    {
        Deselect();
    }

    public void Select()
    {
        m_SelectionSprite.enabled = true;
    }

    public void Deselect()
    {
        m_SelectionSprite.enabled = false;
    }
}
