using UnityEngine;
public enum Entity 
{ 
    Child, 
    Toy 
}
public class EntityIdentity : MonoBehaviour
{
    public Entity Entity;
    public Transform AimOffSet;
}