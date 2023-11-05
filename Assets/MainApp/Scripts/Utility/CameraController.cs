using UnityEngine;
public class CameraController : MonoBehaviour
{
    /// regular speed
    [Tooltip("Normal movement speed")]
    public float moveSpeed = 5f;
    /// regular speed
    [Tooltip("Movement radius")]
    public float moveRadius = 4f;

    /// <summary>
    /// mouse sensitivity 
    /// </summary>
    [Tooltip("mouse sensitivity")]
    public float camSens = 0.25f;

    /// <summary>
    /// kind of in the middle of the screen, rather than at the top (play)
    /// </summary>
    private Vector3 lastMouse = new Vector3(255, 255, 255);

    Vector3 lastPosition;
    private float rotYAxis, rotXAxis;

    void Start()
    {
        //current angles of camera (make sure it's looking at what you want
        //to rotate around)
        rotYAxis = transform.eulerAngles.y;
        rotXAxis = transform.eulerAngles.x;

        xAngle = 0.0f;
        yAngle = 0.0f;
        this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
    }
    void Update()
    {
#if UNITY_WEBGL
        if(BaseScreenTopMenuV2.Instance.isCameraRotating)
            DragScreenOnWeb();
            MoveCamera();
#elif UNITY_EDITOR
        //RotateCamera();
        RotateCameraWithArrowKeys();
#endif
        if(BaseScreenTopMenuV2.Instance.isCameraRotating)
            DragScreen();
    }

    void RotateCameraWithArrowKeys()
    {
        //get your inputs
        rotYAxis += Input.GetAxis(("Horizontal")) * Time.deltaTime * 40;
        rotXAxis -= Input.GetAxis("Vertical") * Time.deltaTime * 40;

        //clamp the angle
        rotXAxis = ClampAngle(rotXAxis, -90, 90);

        // convert it to quaternions
        Quaternion toRotation = Quaternion.Euler(rotXAxis, rotYAxis, 0);
        Quaternion rotation = toRotation;

        //and apply!
        transform.rotation = rotation;
    }

    void RotateCamera()
    {
        //calculate mouse view direction
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse = Input.mousePosition;
        //Mouse camera angle done.
    }

    void MoveCamera()
    {
        //Keyboard commands
        Vector3 p = GetBaseInput();
        if (p != Vector3.zero)
        {
            lastPosition = transform.position;
            transform.Translate(GetBaseInput());
        }
    }

    private Vector3 GetBaseInput()
    {
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.UpArrow))
        {
            p_Velocity = new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            p_Velocity = new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            p_Velocity = new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            p_Velocity = new Vector3(1, 0, 0);
        }
        p_Velocity = p_Velocity * moveSpeed * Time.deltaTime;

        Vector3 newLocation = transform.position + p_Velocity;
        float distance = Vector3.Distance(newLocation, Vector3.zero);

        if (distance >= moveRadius)
        {
            transform.position = Vector3.ClampMagnitude(lastPosition, moveRadius - 0.1f);
            p_Velocity = Vector3.zero;
        }
        return p_Velocity;
    }

    //clamp angle from before
    public float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    private Vector3 firstpoint; //change type on Vector3
    private Vector3 secondpoint;
    private float xAngle = 0.0f; //angle for axes x for rotation
    private float yAngle = 0.0f;
    private float xAngTemp = 0.0f; //temp variable for angle
    private float yAngTemp = 0.0f;

    public void DragScreen()
    {
        //Check count touches
        if (Input.touchCount > 0)
        {
            //Touch began, save position
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                firstpoint = Input.GetTouch(0).position;
                xAngTemp = xAngle;
                yAngTemp = yAngle;
            }
            //Move finger by screen
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                secondpoint = Input.GetTouch(0).position;
                //Mainly, about rotate camera. For example, for Screen.width rotate on 180 degree
                xAngle = xAngTemp - (secondpoint.x - firstpoint.x) * 180 / Screen.width;
                yAngle = yAngTemp + (secondpoint.y - firstpoint.y) * 90 / Screen.height;
                //Rotate camera
                this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
            }
        }
    }

    public void DragScreenOnWeb()
    {
        //Touch began, save position
        if (Input.GetMouseButtonDown(0))
        {
            firstpoint = Input.mousePosition;
            xAngTemp = xAngle;
            yAngTemp = yAngle;
        }
        //Move finger by screen
        if (Input.GetMouseButton(0))
        {
            secondpoint = Input.mousePosition;
            //Mainly, about rotate camera. For example, for Screen.width rotate on 180 degree
            xAngle = xAngTemp - (secondpoint.x - firstpoint.x) * 180 / Screen.width;
            yAngle = yAngTemp + (secondpoint.y - firstpoint.y) * 90 / Screen.height;
            //Rotate camera
            this.transform.rotation = Quaternion.Euler(yAngle, xAngle, 0.0f);
        }
    }
}