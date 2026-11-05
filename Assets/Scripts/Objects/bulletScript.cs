using UnityEngine;

public class bulletScript : MonoBehaviour
{

    [SerializeField] float bulletspeed;
    [SerializeField] int bullettime;

    void Start()
    {
        Destroy(gameObject, bullettime);
    }


    void Update()
    {
        transform.Translate(Vector3.forward * bulletspeed * Time.deltaTime);
    }
}
