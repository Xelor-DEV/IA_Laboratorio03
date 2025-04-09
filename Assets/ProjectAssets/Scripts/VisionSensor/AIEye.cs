using UnityEngine;
public class DataViewBase
{
    #region RangeView


    [Header("----- RangeView -----")]
    [Range(0, 180)]
    public float angle = 30f;
    public float height = 1.0f;
    public float distance = 0f;
    public Color meshSightIn = Color.red;
    public Color meshSightOut = Color.red;
    public Mesh mesh;

    [Header("----- Owner ----- ")]
    public Transform Owner;


    #endregion
    [Header("----- DrawGizmo ----- ")]
    public bool IsDrawGizmo = false;

    [Header("----- LayerMask ----- ")]
    public LayerMask Scanlayers;
    public DataViewBase()
    { }
    public virtual bool IsInSight(Transform enemy)
    {
        return false;
    }
    public void CreateMesh()
    {
        mesh = CreateWedgeMesh();
    }
    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();
        int segments = 10;
        int numTriangles = (segments * 6) + 6;
        int numVertices = numTriangles * 3;
        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        Vector3 bottomCenter = Vector3.down * height;
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance + Vector3.down * height;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance + Vector3.down * height;

        Vector3 topCenter = Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * (2 * height);
        Vector3 topRight = bottomRight + Vector3.up * (2 * height);

        int vert = 0;

        // left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;

        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        // right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;

        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; ++i)
        {
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance + Vector3.down * height;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance + Vector3.down * height;

            topRight = bottomRight + Vector3.up * (2 * height);
            topLeft = bottomLeft + Vector3.up * (2 * height);

            // far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;

            // top 
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            // bottom 
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;

            currentAngle += deltaAngle;
        }

        for (int i = 0; i < numVertices; ++i)
        {
            triangles[i] = i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    public virtual void OnDrawGizmos()
    {

    }
}

#region View
[System.Serializable]
public class DataView : DataViewBase
{

    public LayerMask occlusionlayers;

    [Header("InsideObject")]
    public bool InsideObject = false;

    [Header("InSight")]
    public bool InSight = false;
    Transform enemy;
    public DataView()
    { }

    public override bool IsInSight(Transform AimOffset)
    {
        this.InSight = false;
        enemy = null;
        if (AimOffset == null) return this.InSight;

        Vector3 origin = this.Owner.position;
        Vector3 dest = AimOffset.position;
        Vector3 direcction = dest - origin;

        if (direcction.magnitude > distance + 2)
            return InSight;



        if (dest.y < -(this.height + this.Owner.position.y) || dest.y > (this.height + this.Owner.position.y))
        {
            return this.InSight;
        }

        direcction.y = 0;
        float deltaAngle = Vector3.Angle(direcction.normalized, this.Owner.forward);

        if (deltaAngle > this.angle)
        {
            return this.InSight;
        }

        //if (this.IsDrawGizmo) 
        //  Debug.DrawLine(origin, dest, Color.red, 2f);

        if (this.InsideObject && Physics.Linecast(origin, dest, this.occlusionlayers))
        {
            return this.InSight;
        }


        enemy = AimOffset;

        this.InSight = true;
        return this.InSight;
    }
    public override void OnDrawGizmos()
    {
        if (!this.IsDrawGizmo) return;

        if (this.mesh != null && this.Owner != null)
        {
            if (this.InSight)
                Gizmos.color = this.meshSightIn;
            else
                Gizmos.color = this.meshSightOut;

            Gizmos.DrawMesh(this.mesh, this.Owner.position, this.Owner.rotation);

            Gizmos.DrawLine(this.Owner.position, this.Owner.position + this.Owner.forward * this.distance);
            Gizmos.DrawWireSphere(this.Owner.position + this.Owner.forward * this.distance, 1f);

        }
        //if (enemy != null)
        //{
        //    Gizmos.DrawLine(this.Owner.position, enemy.position);
        //}
    }
}
#endregion

public class AIEye : MonoBehaviour
{
    public DataView dataView = new DataView();
    public Health ViewToy = new Health();
    float[] arrayRate = new float[100];
    float FrameRate = 0;
    int index = 0;
    public float MinRate;
    public float MaxRate;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < arrayRate.Length; i++)
        {
            arrayRate[i] = Random.Range(MinRate, MaxRate);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (FrameRate > arrayRate[index])
        {

            FrameRate = 0;
            Scan();
            index++;
            index = index % arrayRate.Length;
        }

        FrameRate += Time.deltaTime;



    }
    void Scan()
    {
        ViewToy = null;

        Collider[] toys = Physics.OverlapSphere(transform.position, dataView.distance, dataView.Scanlayers);
        float min_dist = float.MaxValue;
        for (int i = 0; (i < toys.Length); i++)
        {

            GameObject toy = toys[i].gameObject;

            Debug.Log("name: " + toy.name);

            Health health = toy.GetComponent<Health>();
            if (health != null && health.AimOffSet != null && dataView.IsInSight(health.AimOffSet))
            {
                float dist = (transform.position - health.transform.position).magnitude;
                if (min_dist > dist)
                {
                    ViewToy = health;

                    min_dist = dist;

                }
            }

        }


        if (ViewToy == null)
            dataView.InSight = false;
    }
    private void OnValidate()
    {
        dataView.CreateMesh();
    }
    private void OnDrawGizmos()
    {
        dataView.OnDrawGizmos();
    }
}