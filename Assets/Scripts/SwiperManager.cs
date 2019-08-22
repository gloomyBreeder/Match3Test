using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwiperManager : BasicManager<SwiperManager>
{
    private Tile _selected;
    [SerializeField]
    private Camera _cam;

    private GameObject GetClickedGameObject()
    {
        RaycastHit2D rayHit = Physics2D.GetRayIntersection(_cam.ScreenPointToRay(Input.mousePosition));
        if (rayHit.collider != null)
        {
            return rayHit.collider.gameObject;
        }
        return null;
    }
    void Update()
    {
        if (Match3Manager.instance.IsShifting)
            return;

        // when we click/swipe on tile
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clicked = GetClickedGameObject();
            if (clicked == null)
                return;

            // if we try to swipe tile and haven't selected another one before
            if (_selected == null)
            {
                _selected = clicked.GetComponent<Tile>();
            }
            // if we clicked another tile before
            else
            {
                DoMatch(clicked);
            }
            
        }

        // when we swiped tile on another tile
        if (Input.GetMouseButtonUp(0))
        {
            GameObject clicked = GetClickedGameObject();
            if (clicked == null)
                return;

            if (_selected != null && clicked == _selected.gameObject)
                return;

            DoMatch(clicked);
        }

    }

    void DoMatch(GameObject swiped)
    {
        Tile tile = swiped.GetComponent<Tile>();
        if (_selected != null && _selected.IsNearTo(tile))
        {
            _selected.SwapSprite(tile);
            _selected.ClearAllMatches();
            tile.ClearAllMatches();

            //if no match => swipe tiles like before
            if (!_selected.HasMatch && !tile.HasMatch)
                tile.SwapSprite(_selected);

            _selected.HasMatch = false;
            tile.HasMatch = false;

        }
        _selected = null;
    }
}
