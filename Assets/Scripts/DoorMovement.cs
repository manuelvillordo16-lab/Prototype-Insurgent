using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    // luego intentar hacer herencia con PlatformMovement
    [SerializeField] float speed;
    [SerializeField] int startingpoint;
    [SerializeField] Transform[] startpoint;
    [SerializeField] Transform[] endpoint;
    private int i;
    private bool key = false;
    
    void Start()
    {
        transform.position = startpoint[startingpoint].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (key == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, endpoint[i].position, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startpoint[startingpoint].position, speed * Time.deltaTime);
        }
    }
   

    public void OpenDoor()
    {
        key = true;
    }

    public void CloseDoor()
    {
        key = false;
    }
}
