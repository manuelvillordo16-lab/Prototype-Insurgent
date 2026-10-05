using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] int startingpoint;
    [SerializeField] Transform[] points;
    private int i;
   
    void Start()
    {
        transform.position = points[startingpoint].position;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        other.transform.SetParent(transform);
    }

    private void OnTriggerExit(Collider other)
    {
        other.transform.SetParent(null);
    }


}
