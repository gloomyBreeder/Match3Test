using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer _render;
    private Vector2[] _adjacentDirections = new Vector2[] 
    {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };
    private bool _matchFound = false;
    [HideInInspector]
    public bool HasMatch;
    void Start()
    {
        _render = GetComponent<SpriteRenderer>();
    }
    public void SwapSprite(Tile swiped)
    {
        SpriteRenderer render2 = swiped.GetComponent<SpriteRenderer>();

        if (_render.sprite == render2.sprite)
        { 
            return;
        }

        Sprite tempSprite = render2.sprite;
        render2.sprite = _render.sprite;
        _render.sprite = tempSprite;
    }

    //find gameobject near to tile according to vector direction
    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D rayHit = Physics2D.Raycast(transform.position, castDir);
        if (rayHit.collider != null)
        {
            return rayHit.collider.gameObject;
        }
        return null;
    }

    //find only nearby tiles
    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < _adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(_adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    public bool IsNearTo(Tile swiped)
    {
        return GetAllAdjacentTiles().Contains(swiped.gameObject);
    }

    private List<GameObject> FindMatch(Vector2 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, castDir);
        
        while (raycastHit.collider != null && raycastHit.collider.GetComponent<SpriteRenderer>().sprite == _render.sprite)
        {
            matchingTiles.Add(raycastHit.collider.gameObject);
            raycastHit = Physics2D.Raycast(raycastHit.collider.transform.position, castDir);
        }
        return matchingTiles;
    }

    private void ClearMatch(Vector2[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }
        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            _matchFound = true;
        }
    }

    public void ClearAllMatches()
    {
        if (_render.sprite == null)
            return;

        ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
        ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });

        if (_matchFound)
        {
            HasMatch = _matchFound;
            _render.sprite = null;
            _matchFound = false;
            Match3Manager.instance.FindNullTiles();
        }

    }
}
