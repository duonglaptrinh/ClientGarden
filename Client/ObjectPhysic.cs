using UnityEngine;

public class ObjectPhysic : MonoBehaviour
{
    public static int SENARIO_OBJECT_PHYSIC_LAYER = 19;

    public void Start()
    {
        SetLayerRecursively(this.gameObject);
    }

    void SetLayerRecursively(GameObject obj)
    {
        obj.layer = SENARIO_OBJECT_PHYSIC_LAYER;

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject);
        }
    }

    public void AddRigidBody()
    {
        if (!GetComponent<Rigidbody>())
        {
            this.gameObject.AddComponent<Rigidbody>();

            Rigidbody rb = GetComponent<Rigidbody>();

            //set default value
            rb.useGravity = false;
            rb.mass = 1;
            rb.drag = 10;
            rb.angularDrag = 1;
        }
    }
}
