using UnityEngine;
using System.Collections.Generic;

public enum FormationType
{
    None,
    Testudo,
    Orb,
    Wedge
}

public class GameManager : MonoBehaviour
{
    [Header("Steering Behaviors")]
    public float AgentSpeed = 10f;
    public float AgentSlowingDistance = 2f;
    public float AgentDistance = 1f;
    public int TestudoWidth = 10;

    [Header("Selection")]
    public SpriteRenderer m_SelectionSprite;
    private Transform m_SelectionTransform;

    public float m_SelectionScale = 3f;

    public Transform m_SelectionPlaneTransform;

    private Plane m_SelectionPlane;

    public float m_ClickTime = 0.1f;

    private static GameManager instance;
    public static GameManager Instance {  get { return instance; } }

    [HideInInspector]
    public List<int> SelectedLeaders = new List<int>();

    [HideInInspector]
    public FormationType SelectedFormationType = FormationType.None;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("There is already a GameManager in your scene!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        m_SelectionPlane = new Plane(m_SelectionPlaneTransform.position, m_SelectionPlaneTransform.forward, m_SelectionPlaneTransform.right);
        m_SelectionTransform = m_SelectionSprite.GetComponent<Transform>();
        ToggleSelection(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectedFormationType = FormationType.Testudo;
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            SelectedFormationType = FormationType.Orb;
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            SelectedFormationType = FormationType.Wedge;
        else
            SelectedFormationType = FormationType.None;
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    //public bool IsSelecting { get; private set; }

    //public Vector3 SelectionStart { get; private set; }
    //public Vector3 CurrentSelection { get; private set; }

    //private void UpdateSelection()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        float dist;
    //        if (m_SelectionPlane.Raycast(ray, out dist))
    //        {
    //            SelectionStart = ray.GetPoint(dist);

    //            UpdateSelectionArea(SelectionStart, SelectionStart);
    //            GameManager.Instance.ToggleSelection(true);
    //        }
    //    }
    //    else if (Input.GetMouseButton(0))
    //    {
    //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        float dist;
    //        if (m_SelectionPlane.Raycast(ray, out dist))
    //        {
    //            CurrentSelection = ray.GetPoint(dist);

    //            UpdateSelectionArea(SelectionStart, CurrentSelection);
    //        }
    //    }
    //    else if (Input.GetMouseButtonUp(0))
    //    {
    //        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        float dist;
    //        if (m_SelectionPlane.Raycast(ray, out dist))
    //        {
    //            ToggleSelection(false);
    //        }
    //    }
    //}

    public void ToggleSelection(bool toggle)
    {
        m_SelectionSprite.enabled = toggle;
    }

    public void UpdateSelectionArea(Vector3 pointA, Vector3 pointB)
    {
        var maxX = Mathf.Max(pointA.x, pointB.x);
        var minX = Mathf.Min(pointA.x, pointB.x);

        var maxZ = Mathf.Max(pointA.z, pointB.z);
        var minZ= Mathf.Min(pointA.z, pointB.z);

        var diffX = maxX - minX;
        var diffY = 10f; // TODO
        var diffZ = maxZ - minZ;

        var midX = (diffX) / 2f;
        var midY = m_SelectionPlaneTransform.position.y;
        var midZ = (diffZ) / 2f;
                
        var position = new Vector3(minX + midX, midY, minZ + midZ);

        m_SelectionTransform.position = position;
        m_SelectionTransform.localScale = Vector3.one * m_SelectionScale;
        m_SelectionSprite.size = new Vector2(diffX / m_SelectionScale, diffZ / m_SelectionScale);
    }

    public void GetBounds(Vector3 pointA, Vector3 pointB, out Vector3 center, out Vector3 size)
    {
        var maxX = Mathf.Max(pointA.x, pointB.x);
        var minX = Mathf.Min(pointA.x, pointB.x);

        var maxZ = Mathf.Max(pointA.z, pointB.z);
        var minZ = Mathf.Min(pointA.z, pointB.z);

        var diffX = maxX - minX;
        var diffY = 10f; // TODO
        var diffZ = maxZ - minZ;

        var midX = (diffX) / 2f;
        var midY = m_SelectionPlaneTransform.position.y;
        var midZ = (diffZ) / 2f;

        center = new Vector3(minX + midX, midY, minZ + midZ);
        size = new Vector3(diffX, diffY, diffZ);
    }
}
