using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SelectionCanvas : MonoBehaviour
{
    private Image m_SelectionSprite;

    private Vector2 m_SelectionStart;
    
    private float m_ScreenDistanceToSelect = 0.02f;

    private Plane m_GroundPlane = new Plane(Vector3.zero, Vector3.right, Vector3.forward);

    private List<Selectable> m_SelectedAgents = new List<Selectable>();
    private List<int> m_SelectedLeaders = new List<int>();

    private void Start()
    {
        m_SelectionSprite = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (var oldSelection in m_SelectedAgents)
                oldSelection.Deselect();

            m_SelectedAgents.Clear();

            UpdateSelectedLeaders();
        }

        if(Input.GetMouseButtonDown(0))
        {
            m_SelectionStart = Input.mousePosition;
            m_SelectionSprite.enabled = true;
            var zeroRect = new Rect(0, 0, 0, 0);
            m_SelectionSprite.rectTransform.sizeDelta = Vector2.zero;
        }
        else if(Input.GetMouseButton(0))
        {
            AlignSelectionWithMousePosition();
        }
        else if(Input.GetMouseButtonUp(0))
        {
            m_SelectionSprite.enabled = false;

            if (WasDraggingMouse())
            {
                foreach (var oldSelection in m_SelectedAgents)
                    oldSelection.Deselect();

                m_SelectedAgents = GetObjectsInSelectedFrustum();
                foreach (var newSelection in m_SelectedAgents)
                    newSelection.Select();

                UpdateSelectedLeaders();                
            }
            else
            {
                var worldPos = GetWorldMousePosition();
                if(worldPos.HasValue)
                {
                    foreach (var leader in m_SelectedAgents)
                        leader.GetComponent<LeaderComponent>().Move(worldPos.Value);
                }
            }
        }
    }

    private bool WasDraggingMouse()
    {
        var delta = (Vector2)Input.mousePosition - m_SelectionStart;
        if (Math.Abs(delta.x) / Screen.width > m_ScreenDistanceToSelect)
            return true;
        if (Math.Abs(delta.y) / Screen.height > m_ScreenDistanceToSelect)
            return true;
        return false;
    }

    private List<Selectable> GetObjectsInSelectedFrustum()
    {
        List<Selectable> objectsList = new List<Selectable>();

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        foreach (var agents in GameObject.FindObjectsOfType<Selectable>())
        {
            if (GeometryUtility.TestPlanesAABB(planes, agents.GetComponent<Collider>().bounds))
            {
                if(IsWithinSelectionBounds(agents.transform.position))
                    objectsList.Add(agents);
            }
        }
        
        return objectsList;
    }

    private Vector3? GetWorldMousePosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float rayDist;
        if (m_GroundPlane.Raycast(ray, out rayDist))
            return ray.GetPoint(rayDist);
        return null;
    }

    private void AlignSelectionWithMousePosition()
    {
        var startX = Math.Min(Input.mousePosition.x, m_SelectionStart.x);
        var endX = Math.Max(Input.mousePosition.x, m_SelectionStart.x);
        var startY = Math.Min(Input.mousePosition.y, m_SelectionStart.y);
        var endY = Math.Max(Input.mousePosition.y, m_SelectionStart.y);
        m_SelectionSprite.rectTransform.anchoredPosition = new Vector2(startX, startY);
        m_SelectionSprite.rectTransform.sizeDelta = new Vector2(endX - startX, endY - startY);
    }

    private bool IsWithinSelectionBounds(Vector3 worldPostion)
    {
        Vector2 screenSpacePos = Camera.main.WorldToScreenPoint(worldPostion);
        var startX = Math.Min(Input.mousePosition.x, m_SelectionStart.x);
        var endX = Math.Max(Input.mousePosition.x, m_SelectionStart.x);
        var startY = Math.Min(Input.mousePosition.y, m_SelectionStart.y);
        var endY = Math.Max(Input.mousePosition.y, m_SelectionStart.y);

        return screenSpacePos.x >= startX && screenSpacePos.x <= endX && screenSpacePos.y >= startY && screenSpacePos.y <= endY;
    }

    private void UpdateSelectedLeaders()
    {
        m_SelectedLeaders.Clear();
        foreach(var agent in m_SelectedAgents)
        {
            var leader = agent.GetComponent<LeaderComponent>();
            m_SelectedLeaders.Add(leader.ID);
        }

        GameManager.Instance.SelectedLeaders = m_SelectedLeaders;
    }
}
